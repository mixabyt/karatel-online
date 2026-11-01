// Patches the 2012 Kopatel Online web client (Assembly-CSharp*.dll) so it works with the local server:
//  * every kopatel-online.ru URL is rebuilt at runtime from the address the .unity3d was loaded from;
//  * Photon connects over TCP to <that host>:4530 instead of the dead Photon Cloud app;
//  * code the fan build cut out (room creation, joining, player spawn, camera, interpolation...) is
//    rebuilt from Fixes/KopFix.cs and copied into the original classes;
//  * "photonView.isMine" checks the fan build had replaced with constants are restored;
//  * coin purchases are signed with our own RSA key instead of the original developer's.
// Usage: KopPatcher <dir with original dlls> <output dir> <keys.json> [Unity WebPlayer folder]
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

if (args.Length < 3)
{
    Console.Error.WriteLine("usage: KopPatcher <inDir> <outDir> <keys.json> [UnityWebPlayerDir]");
    return 2;
}
string inDir = Path.GetFullPath(args[0]), outDir = Path.GetFullPath(args[1]), keysPath = Path.GetFullPath(args[2]);
string webPlayer = args.Length > 3 ? args[3]
    : Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE") ?? "", "AppData", "LocalLow", "Unity", "WebPlayer");
string monoLib = Path.Combine(webPlayer, "mono", "Stable3.x.x", "Data", "lib");
string unityLib = Path.Combine(webPlayer, "player", "Stable3.x.x", "Data", "lib");
Check(File.Exists(Path.Combine(monoLib, "mscorlib.dll")) && File.Exists(Path.Combine(unityLib, "UnityEngine.dll")),
      $"Unity Web Player libraries not found under {webPlayer} (install the Web Player or pass its folder)");
Directory.CreateDirectory(outDir);
string work = Path.Combine(Path.GetTempPath(), "koppatch-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(work);

const int PhotonPort = 4530;

var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(inDir);
AssemblyDefinition Read(string path, IMetadataImporterProvider importer = null) =>
    AssemblyDefinition.ReadAssembly(new MemoryStream(File.ReadAllBytes(path)),
        new ReaderParameters { AssemblyResolver = resolver, ReadingMode = ReadingMode.Immediate, MetadataImporterProvider = importer });

try
{
    // ---------- 1. a publicized copy of the game assemblies, only to compile Fixes/KopFix.cs against ----------
    string refDir = Path.Combine(work, "ref");
    Directory.CreateDirectory(refDir);
    foreach (var name in new[] { "Assembly-CSharp.dll", "Assembly-CSharp-firstpass.dll" })
    {
        var asm = Read(Path.Combine(inDir, name));
        foreach (var t in asm.MainModule.GetTypes())
        {
            if (t.IsNested) t.IsNestedPublic = true; else t.IsPublic = true;
            foreach (var f in t.Fields) f.IsPublic = true;
            foreach (var m in t.Methods) m.IsPublic = true;
        }
        asm.Write(Path.Combine(refDir, name));
    }
    string fixDll = Path.Combine(work, "KopFix.dll");
    CompileFixes(Path.Combine(AppContext.BaseDirectory, "Fixes", "KopFix.cs"), fixDll, refDir);

    // ---------- 2. Assembly-CSharp-firstpass: KopNet helpers + Photon over TCP to our host ----------
    var firstpass = Read(Path.Combine(inDir, "Assembly-CSharp-firstpass.dll"));
    PatchFirstpass(firstpass.MainModule);
    firstpass.Write(Path.Combine(outDir, "Assembly-CSharp-firstpass.dll"));

    // ---------- 3. Assembly-CSharp ----------
    var game = Read(Path.Combine(inDir, "Assembly-CSharp.dll"), new ByNameImporterProvider());
    var gm = game.MainModule;
    var keys = PatchStringsAndKeys(gm);
    RestoreFanConstants(gm);
    TransplantFixes(gm, fixDll, refDir);
    AddPrefixes(gm);
    RedirectBattleChecks(gm);
    game.Write(Path.Combine(outDir, "Assembly-CSharp.dll"));

    Directory.CreateDirectory(Path.GetDirectoryName(keysPath));
    File.WriteAllText(keysPath, JsonSerializer.Serialize(keys, new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine($"keys written to {keysPath}");
    return 0;
}
finally
{
    try { Directory.Delete(work, true); } catch (IOException) { }
}

// =====================================================================================================

void CompileFixes(string source, string output, string refDir)
{
    string dotnetRoot = Path.GetFullPath(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "..", "..", ".."));
    string csc = Directory.GetDirectories(Path.Combine(dotnetRoot, "sdk"))
        .Select(d => Path.Combine(d, "Roslyn", "bincore", "csc.dll"))
        .Where(File.Exists)
        .OrderByDescending(p => Version.TryParse(Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(p)))).Split('-')[0], out var v) ? v : new Version())
        .FirstOrDefault();
    Check(csc != null, "C# compiler (csc.dll) not found in the .NET SDK");
    var refs = new[]
    {
        Path.Combine(monoLib, "mscorlib.dll"), Path.Combine(monoLib, "System.dll"), Path.Combine(monoLib, "System.Core.dll"),
        Path.Combine(unityLib, "UnityEngine.dll"), Path.Combine(refDir, "Assembly-CSharp.dll"),
        Path.Combine(refDir, "Assembly-CSharp-firstpass.dll"), Path.Combine(inDir, "Photon3Unity3D.dll"),
    };
    var psi = new ProcessStartInfo(Path.Combine(dotnetRoot, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "dotnet.exe" : "dotnet"))
    { RedirectStandardOutput = true, RedirectStandardError = true };
    foreach (var a in new[] { csc, "-nologo", "-noconfig", "-nostdlib", "-target:library", "-langversion:7.3",
                              "-optimize+", "-debug-", "-deterministic", "-out:" + output })
        psi.ArgumentList.Add(a);
    foreach (var r in refs) psi.ArgumentList.Add("-r:" + r);
    psi.ArgumentList.Add(source);
    using var p = Process.Start(psi);
    string outText = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
    p.WaitForExit();
    if (p.ExitCode != 0)
        throw new InvalidOperationException("compiling KopFix.cs failed:\n" + outText);
    Console.WriteLine("KopFix.cs compiled");
}

void PatchFirstpass(ModuleDefinition fm)
{
    var fts = fm.TypeSystem;
    var unityRef = fm.AssemblyReferences.First(a => a.Name == "UnityEngine");
    var getAbsoluteUrl = new MethodReference("get_absoluteURL", fts.String,
        new TypeReference("UnityEngine", "Application", fm, unityRef));
    var indexOfChar = StringMethod(fm, "IndexOf", fts.Int32, true, fts.Char);
    var indexOfString = StringMethod(fm, "IndexOf", fts.Int32, true, fts.String);
    var lastIndexOfChar = StringMethod(fm, "LastIndexOf", fts.Int32, true, fts.Char);
    var substring1 = StringMethod(fm, "Substring", fts.String, true, fts.Int32);
    var substring2 = StringMethod(fm, "Substring", fts.String, true, fts.Int32, fts.Int32);

    var kopNet = new TypeDefinition("", "KopNet",
        TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, fts.Object);
    fm.Types.Add(kopNet);

    // KopNet.Base(): "http://host:port/dir/" = the URL of the loaded .unity3d without query and file name.
    var baseMethod = new MethodDefinition("Base", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, fts.String);
    kopNet.Methods.Add(baseMethod);
    {
        var body = baseMethod.Body;
        body.InitLocals = true;
        body.Variables.Add(new VariableDefinition(fts.String));
        body.Variables.Add(new VariableDefinition(fts.Int32));
        var il = body.GetILProcessor();
        var noQuery = il.Create(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Call, getAbsoluteUrl);
        il.Emit(OpCodes.Stloc_0);
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ldc_I4, (int)'?');
        il.Emit(OpCodes.Callvirt, indexOfChar);
        il.Emit(OpCodes.Stloc_1);
        il.Emit(OpCodes.Ldloc_1);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Blt, noQuery);
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Ldloc_1);
        il.Emit(OpCodes.Callvirt, substring2);
        il.Emit(OpCodes.Stloc_0);
        il.Append(noQuery);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ldc_I4, (int)'/');
        il.Emit(OpCodes.Callvirt, lastIndexOfChar);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Callvirt, substring2);
        il.Emit(OpCodes.Ret);
    }

    // KopNet.Host(): just the host name out of Base().
    var hostMethod = new MethodDefinition("Host", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, fts.String);
    kopNet.Methods.Add(hostMethod);
    {
        var body = hostMethod.Body;
        body.InitLocals = true;
        body.Variables.Add(new VariableDefinition(fts.String));
        body.Variables.Add(new VariableDefinition(fts.Int32));
        var il = body.GetILProcessor();
        il.Emit(OpCodes.Call, baseMethod);
        il.Emit(OpCodes.Stloc_0);
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ldstr, "//");
        il.Emit(OpCodes.Callvirt, indexOfString);
        il.Emit(OpCodes.Ldc_I4_2);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Callvirt, substring1);
        il.Emit(OpCodes.Stloc_0);
        foreach (char stop in new[] { '/', ':' })
        {
            var next = il.Create(OpCodes.Nop);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldc_I4, (int)stop);
            il.Emit(OpCodes.Callvirt, indexOfChar);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Blt, next);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, substring2);
            il.Emit(OpCodes.Stloc_0);
            il.Append(next);
        }
        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ret);
    }

    var photonNetwork = fm.GetType("PhotonNetwork");
    int tcpPatched = 0;
    foreach (var m in photonNetwork.Methods.Where(m => m.HasBody))
    {
        foreach (var ins in m.Body.Instructions)
        {
            if (ins.OpCode == OpCodes.Newobj && ins.Operand is MethodReference ctor && ctor.DeclaringType.Name == "NetworkingPeer"
                && ins.Previous.OpCode == OpCodes.Ldc_I4_0)
            {
                ins.Previous.OpCode = OpCodes.Ldc_I4_1; // ConnectionProtocol.Udp -> Tcp
                tcpPatched++;
            }
        }
    }
    Check(tcpPatched == 1, $"ConnectionProtocol patch sites: {tcpPatched}");

    // NetworkingPeer.requestSecurity = true makes the client start a Diffie-Hellman key exchange and
    // send OpAuthenticate encrypted; the local server speaks plain Photon, so switch it off.
    int securityPatched = 0;
    foreach (var ctor in fm.GetType("NetworkingPeer").Methods.Where(m => m.IsConstructor && m.HasBody))
    {
        foreach (var ins in ctor.Body.Instructions)
        {
            if (ins.OpCode == OpCodes.Stfld && ins.Operand is FieldReference f && f.Name == "requestSecurity"
                && ins.Previous.OpCode == OpCodes.Ldc_I4_1)
            {
                ins.Previous.OpCode = OpCodes.Ldc_I4_0;
                securityPatched++;
            }
        }
    }
    Check(securityPatched == 1, $"requestSecurity patch sites: {securityPatched}");

    var connectUsingSettings = photonNetwork.Methods.Single(m => m.Name == "ConnectUsingSettings" && m.Parameters.Count == 1);
    var connect4 = photonNetwork.Methods.Single(m => m.Name == "Connect" && m.Parameters.Count == 4);
    var cbody = connectUsingSettings.Body;
    cbody.Instructions.Clear();
    cbody.Variables.Clear();
    cbody.ExceptionHandlers.Clear();
    var cil = cbody.GetILProcessor();
    cil.Emit(OpCodes.Call, hostMethod);
    cil.Emit(OpCodes.Ldc_I4, PhotonPort);
    cil.Emit(OpCodes.Ldstr, "Master");
    cil.Emit(OpCodes.Ldarg_0);
    cil.Emit(OpCodes.Call, connect4);
    cil.Emit(OpCodes.Ret);
    Console.WriteLine($"firstpass: KopNet added, Photon -> TCP <page host>:{PhotonPort}, no encryption handshake");
}

Dictionary<string, object> PatchStringsAndKeys(ModuleDefinition gm)
{
    var gts = gm.TypeSystem;
    var firstpassRef = gm.AssemblyReferences.First(a => a.Name == "Assembly-CSharp-firstpass");
    var baseRef = new MethodReference("Base", gts.String, new TypeReference("", "KopNet", gm, firstpassRef));
    var concat2 = StringMethod(gm, "Concat", gts.String, false, gts.String, gts.String);
    string[] urlPrefixes =
    {
        "https://kopatel-online.ru/2012/", "http://kopatel-online.ru/2012/", "//kopatel-online.ru/2012/",
        "https://kopatel-online.ru/", "http://kopatel-online.ru/",
    };

    // The key pair of the previous build is kept, so a rebuilt client works with a server that is
    // already running (it read keys.json at start). A new pair only without one.
    byte[] modulus, exponent, privateExponent;
    var old = File.Exists(keysPath) ? JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(keysPath)) : null;
    if (old != null && old.ContainsKey("rsa_n") && old.ContainsKey("rsa_e") && old.ContainsKey("rsa_d"))
    {
        modulus = Convert.FromHexString(old["rsa_n"]);
        exponent = Convert.FromHexString(old["rsa_e"]);
        privateExponent = Convert.FromHexString(old["rsa_d"]);
        Console.WriteLine($"RSA key kept from {keysPath}");
    }
    else
    {
        using var rsa = RSA.Create(1024);
        var rsaParams = rsa.ExportParameters(true);
        (modulus, exponent, privateExponent) = (rsaParams.Modulus, rsaParams.Exponent, rsaParams.D);
    }
    string rsaXml = $"<RSAKeyValue><Modulus>{Convert.ToBase64String(modulus)}</Modulus>"
                  + $"<Exponent>{Convert.ToBase64String(exponent)}</Exponent></RSAKeyValue>";

    int urlCount = 0, rsaCount = 0;
    string secretCipher = null, secretPassword = null;
    foreach (var (type, method) in AllMethods(gm))
    {
        var body = method.Body;
        body.SimplifyMacros();
        var il = body.GetILProcessor();
        foreach (var ins in body.Instructions.ToList())
        {
            if (ins.OpCode != OpCodes.Ldstr) continue;
            var s = (string)ins.Operand;
            if (type.Name == "VkontakteAPI" && method.Name == "GetSecret")
            {
                if (secretCipher == null) secretCipher = s; else secretPassword ??= s;
                continue;
            }
            if (s.StartsWith("<RSAKeyValue>"))
            {
                ins.Operand = rsaXml;
                rsaCount++;
                continue;
            }
            var prefix = urlPrefixes.FirstOrDefault(s.StartsWith);
            if (prefix == null) continue;
            ins.OpCode = OpCodes.Call;
            ins.Operand = baseRef;
            var ldSuffix = il.Create(OpCodes.Ldstr, s.Substring(prefix.Length));
            il.InsertAfter(ins, ldSuffix);
            il.InsertAfter(ldSuffix, il.Create(OpCodes.Call, concat2));
            urlCount++;
        }
        body.OptimizeMacros();
    }
    Check(urlCount >= 8, $"URL patch sites: {urlCount}");
    Check(rsaCount >= 1, $"RSA key sites: {rsaCount}");
    Check(secretCipher != null && secretPassword != null, "GetSecret strings not found");
    Console.WriteLine($"Assembly-CSharp: {urlCount} URLs rebased, RSA key replaced");
    return new Dictionary<string, object>
    {
        ["secret"] = DecryptSecret(secretCipher, secretPassword),
        ["rsa_n"] = Convert.ToHexString(modulus),
        ["rsa_e"] = Convert.ToHexString(exponent),
        ["rsa_d"] = Convert.ToHexString(privateExponent),
    };
}

// The fan build overwrote 5-byte calls with same-sized constants:
//   photonView.isMine           -> pop; ldc.i4.1; nop; nop; nop
//   MainPlayer != null          -> pop; pop; ldc.i4.0; nop; nop      (PlayerNetwork.Start)
//   if (pos.y < 0) respawn       -> "br" that leaves two floats on the stack (WorldGameObject.Update,
//                                  rejected by the player's IL verifier, so Update never ran)
void RestoreFanConstants(ModuleDefinition gm)
{
    var firstpassRef = gm.AssemblyReferences.First(a => a.Name == "Assembly-CSharp-firstpass");
    var unityRef = gm.AssemblyReferences.First(a => a.Name == "UnityEngine");
    var isMine = new MethodReference("get_isMine", gm.TypeSystem.Boolean, new TypeReference("", "PhotonView", gm, firstpassRef)) { HasThis = true };
    var unityObject = new TypeReference("UnityEngine", "Object", gm, unityRef);
    var objNotEqual = new MethodReference("op_Inequality", gm.TypeSystem.Boolean, unityObject);
    objNotEqual.Parameters.Add(new ParameterDefinition(unityObject));
    objNotEqual.Parameters.Add(new ParameterDefinition(unityObject));

    int isMineCount = 0, notNullCount = 0, updateCount = 0;
    foreach (var (type, method) in AllMethods(gm))
    {
        var body = method.Body;
        body.SimplifyMacros();
        var list = body.Instructions;
        for (int i = 0; i + 5 < list.Count; i++)
        {
            if ((list[i].OpCode == OpCodes.Call || list[i].OpCode == OpCodes.Callvirt)
                && list[i].Operand is MethodReference mr && mr.Name == "get_photonView"
                && list[i + 1].OpCode == OpCodes.Pop && IsLdcInt(list[i + 2])
                && list[i + 3].OpCode == OpCodes.Nop && list[i + 4].OpCode == OpCodes.Nop && list[i + 5].OpCode == OpCodes.Nop)
            {
                list[i + 1].OpCode = OpCodes.Callvirt;
                list[i + 1].Operand = isMine;
                list[i + 2].OpCode = OpCodes.Nop;
                list[i + 2].Operand = null;
                isMineCount++;
            }
            if (type.Name == "PlayerNetwork" && method.Name == "Start"
                && list[i].OpCode == OpCodes.Ldnull && list[i + 1].OpCode == OpCodes.Pop && list[i + 2].OpCode == OpCodes.Pop
                && IsLdcInt(list[i + 3]) && list[i + 4].OpCode == OpCodes.Nop && list[i + 5].OpCode == OpCodes.Nop)
            {
                list[i + 1].OpCode = OpCodes.Call;
                list[i + 1].Operand = objNotEqual;
                foreach (var k in new[] { i + 2, i + 3 })
                {
                    list[k].OpCode = OpCodes.Nop;
                    list[k].Operand = null;
                }
                notNullCount++;
            }
        }
        if (type.Name == "WorldGameObject" && method.Name == "Update")
        {
            foreach (var ins in list)
            {
                if (ins.OpCode == OpCodes.Ldc_R4 && (float)ins.Operand == 0f && ins.Next.OpCode == OpCodes.Br
                    && ins.Previous.Operand is FieldReference f && f.Name == "y")
                {
                    ins.Next.OpCode = OpCodes.Bge_Un;
                    updateCount++;
                }
            }
        }
        body.OptimizeMacros();
    }
    Check(isMineCount >= 15, $"isMine restore sites: {isMineCount}");
    Check(notNullCount == 1, $"PlayerNetwork MainPlayer check sites: {notNullCount}");
    Check(updateCount == 1, $"WorldGameObject.Update branch sites: {updateCount}");
    Console.WriteLine($"restored {isMineCount} photonView.isMine checks, PlayerNetwork MainPlayer check, Update fall-respawn branch");
}

void TransplantFixes(ModuleDefinition gm, string fixDll, string refDir)
{
    var fixResolver = new DefaultAssemblyResolver();
    foreach (var d in new[] { refDir, monoLib, unityLib, inDir }) fixResolver.AddSearchDirectory(d);
    var fix = AssemblyDefinition.ReadAssembly(fixDll, new ReaderParameters { AssemblyResolver = fixResolver, ReadingMode = ReadingMode.Immediate });
    var kop = fix.MainModule.GetType("KopFix");
    var sources = kop.Methods.Where(m => m.IsStatic && m.Name.Contains("__")).ToList();
    Check(sources.Count == kop.Methods.Count(m => !m.IsConstructor), "every KopFix method must be named Type__Method");

    // First create/find all targets so that calls between KopFix methods can be redirected.
    var targets = new Dictionary<MethodDefinition, MethodDefinition>();
    foreach (var src in sources)
    {
        var parts = src.Name.Split(new[] { "__" }, 2, StringSplitOptions.None);
        var type = gm.GetType(parts[0]) ?? throw new InvalidOperationException($"no type {parts[0]} for {src.Name}");
        var arity = src.Parameters.Count - 1;
        var existing = type.Methods.Where(m => m.Name == parts[1] && !m.IsStatic && m.Parameters.Count == arity).ToList();
        Check(existing.Count <= 1, $"{src.Name} is ambiguous");
        targets[src] = existing.FirstOrDefault();
    }
    var fields = new Dictionary<string, FieldDefinition>();
    var importer = new Importer(gm, mr =>
    {
        var def = sources.FirstOrDefault(s => s.Name == mr.Name);
        return def == null ? null : targets[def];
    }, fr => fields.GetValueOrDefault(fr.Name));
    // Static KopFix fields named Type__Field become public static fields of Type (no initializers:
    // KopFix's static constructor is not copied).
    Check(!kop.Methods.Any(m => m.IsConstructor && m.IsStatic), "KopFix fields may not have initializers");
    foreach (var f in kop.Fields)
    {
        var parts = f.Name.Split(new[] { "__" }, 2, StringSplitOptions.None);
        Check(f.IsStatic && parts.Length == 2, $"KopFix field {f.Name} must be static and named Type__Field");
        var added = new FieldDefinition(parts[1], FieldAttributes.Public | FieldAttributes.Static, importer.Type(f.FieldType));
        gm.GetType(parts[0]).Fields.Add(added);
        fields[f.Name] = added;
    }
    foreach (var src in sources)
    {
        if (targets[src] != null) continue;
        var parts = src.Name.Split(new[] { "__" }, 2, StringSplitOptions.None);
        // Public: other classes call them (Level.Start -> WorldGameObject.Kop_RegisterPlayer), and the web
        // player enforces member access.
        var added = new MethodDefinition(parts[1], MethodAttributes.Public | MethodAttributes.HideBySig, importer.Type(src.ReturnType));
        foreach (var p in src.Parameters.Skip(1))
            added.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, importer.Type(p.ParameterType)));
        gm.GetType(parts[0]).Methods.Add(added);
        targets[src] = added;
    }
    foreach (var src in sources)
    {
        var dst = targets[src];
        Check(dst.ReturnType.FullName == importer.Type(src.ReturnType).FullName, $"{src.Name}: return type differs");
        Transplant(src, dst, importer);
        CheckAccess(dst);
        Console.WriteLine($"  rebuilt {dst.DeclaringType.Name}.{dst.Name}");
    }
}

// KopFix.cs compiles against a publicized copy, but the web player enforces the real visibility:
// code copied into a class may only use private members of that same class (or its nested types).
void CheckAccess(MethodDefinition dst)
{
    bool Inside(TypeDefinition t)
    {
        for (; t != null; t = t.DeclaringType)
            if (t == dst.DeclaringType) return true;
        return false;
    }
    foreach (var ins in dst.Body.Instructions)
    {
        switch (ins.Operand)
        {
            case MethodDefinition md when !Inside(md.DeclaringType) && (md.IsPrivate || md.IsFamily):
                Check(false, $"{dst.DeclaringType.Name}.{dst.Name} calls inaccessible {md.DeclaringType.Name}.{md.Name}");
                break;
            case FieldDefinition fd when !Inside(fd.DeclaringType) && (fd.IsPrivate || fd.IsFamily):
                Check(false, $"{dst.DeclaringType.Name}.{dst.Name} uses inaccessible {fd.DeclaringType.Name}.{fd.Name}");
                break;
        }
    }
}

void Transplant(MethodDefinition src, MethodDefinition dst, Importer importer)
{
    // The web player's verifier rejects "constrained." on enums ("Object not compatible with
    // constrained type"); old Unity compilers boxed instead, and so must KopFix.cs.
    Check(src.Body.Instructions.All(i => i.OpCode != OpCodes.Constrained),
          $"{src.Name} uses a 'constrained.' call; box the value type explicitly in KopFix.cs");
    src.Body.SimplifyMacros();
    var body = dst.Body;
    body.Instructions.Clear();
    body.Variables.Clear();
    body.ExceptionHandlers.Clear();
    body.InitLocals = src.Body.InitLocals;
    foreach (var v in src.Body.Variables)
        body.Variables.Add(new VariableDefinition(importer.Type(v.VariableType)));
    var map = new Dictionary<Instruction, Instruction>();
    foreach (var ins in src.Body.Instructions)
    {
        var copy = Instruction.Create(OpCodes.Nop);
        copy.OpCode = ins.OpCode;
        copy.Operand = ins.Operand switch
        {
            ParameterDefinition p => p.Index == 0 ? body.ThisParameter : dst.Parameters[p.Index - 1],
            VariableDefinition v => body.Variables[v.Index],
            MethodReference m => importer.Method(m),
            FieldReference f => importer.Field(f),
            TypeReference t => importer.Type(t),
            Instruction or Instruction[] => null,
            var other => other,
        };
        map[ins] = copy;
        body.Instructions.Add(copy);
    }
    foreach (var ins in src.Body.Instructions)
    {
        if (ins.Operand is Instruction target) map[ins].Operand = map[target];
        else if (ins.Operand is Instruction[] targets) map[ins].Operand = targets.Select(x => map[x]).ToArray();
    }
    foreach (var h in src.Body.ExceptionHandlers)
    {
        body.ExceptionHandlers.Add(new ExceptionHandler(h.HandlerType)
        {
            TryStart = map[h.TryStart], TryEnd = h.TryEnd == null ? null : map[h.TryEnd],
            HandlerStart = map[h.HandlerStart], HandlerEnd = h.HandlerEnd == null ? null : map[h.HandlerEnd],
            FilterStart = h.FilterStart == null ? null : map[h.FilterStart],
            CatchType = h.CatchType == null ? null : importer.Type(h.CatchType),
        });
    }
    body.OptimizeMacros();
}

void AddPrefixes(ModuleDefinition gm)
{
    // OnCreatedRoom: the fan build blanked its start; run the rebuilt part first.
    var wgo = gm.GetType("WorldGameObject");
    var onCreated = wgo.Methods.Single(m => m.Name == "OnCreatedRoom");
    var kopOnCreated = wgo.Methods.Single(m => m.Name == "Kop_OnCreatedRoom");
    var il = onCreated.Body.GetILProcessor();
    var first = onCreated.Body.Instructions[0];
    il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
    il.InsertBefore(first, il.Create(OpCodes.Call, kopOnCreated));

    // ResourceLoader: the sound bundle is not loaded any more (and not available), so playing a sound
    // indexed an empty array and threw. Return early instead.
    var loader = gm.GetType("ResourceLoader");
    var loaded = loader.Fields.Single(f => f.Name == "SoundsBundleLoaded");
    foreach (var name in new[] { "GetClip", "PlayEffectAtPoint" })
    {
        var m = loader.Methods.Single(x => x.Name == name);
        var mil = m.Body.GetILProcessor();
        var start = m.Body.Instructions[0];
        mil.InsertBefore(start, mil.Create(OpCodes.Ldarg_0));
        mil.InsertBefore(start, mil.Create(OpCodes.Ldfld, loaded));
        mil.InsertBefore(start, mil.Create(OpCodes.Brtrue, start));
        if (m.ReturnType.MetadataType != MetadataType.Void)
            mil.InsertBefore(start, mil.Create(OpCodes.Ldnull));
        mil.InsertBefore(start, mil.Create(OpCodes.Ret));
    }
    Console.WriteLine("prefixes: OnCreatedRoom, ResourceLoader sound guards");
}

// Igor.enableGuns is also true on a building map while the player holds the weapons (KopFix). Blocks
// that lost their support only fall off in the battle modes, and every client decides that for
// itself, so this check must not depend on one player's weapons.
void RedirectBattleChecks(ModuleDefinition gm)
{
    var battleGuns = gm.GetType("Igor").Methods.Single(m => m.Name == "Kop_BattleGuns");
    var verify = gm.GetType("BlocksAvulsion").Methods.Single(m => m.Name == "VerifyAvulsionAroundBlocks");
    int count = 0;
    foreach (var ins in verify.Body.Instructions)
    {
        if ((ins.OpCode == OpCodes.Call || ins.OpCode == OpCodes.Callvirt)
            && ins.Operand is MethodReference mr && mr.Name == "get_enableGuns")
        {
            ins.Operand = battleGuns;
            count++;
        }
    }
    Check(count == 1, $"BlocksAvulsion enableGuns sites: {count}");
    Console.WriteLine("BlocksAvulsion: battle modes only");
}

static bool IsLdcInt(Instruction i) =>
    i.OpCode == OpCodes.Ldc_I4 || i.OpCode == OpCodes.Ldc_I4_0 || i.OpCode == OpCodes.Ldc_I4_1 || i.OpCode == OpCodes.Ldc_I4_S;

static IEnumerable<(TypeDefinition, MethodDefinition)> AllMethods(ModuleDefinition m) =>
    m.GetTypes().SelectMany(t => t.Methods.Where(x => x.HasBody).Select(x => (t, x)));

static MethodReference StringMethod(ModuleDefinition m, string name, TypeReference ret, bool instance, params TypeReference[] ps)
{
    var mr = new MethodReference(name, ret, m.TypeSystem.String) { HasThis = instance };
    foreach (var p in ps) mr.Parameters.Add(new ParameterDefinition(p));
    return mr;
}

static void Check(bool ok, string what)
{
    if (!ok) throw new InvalidOperationException("Unexpected client build: " + what);
}

// Same as the client's Crypto.DHJKVS: PBKDF2-SHA1 key, AES-256-CBC, IV stored length-prefixed in front.
static string DecryptSecret(string cipherText, string password)
{
    var kdf = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes("o6806642kbM7c5"));
    var data = Convert.FromBase64String(cipherText);
    int ivLen = BitConverter.ToInt32(data, 0);
    var iv = data.AsSpan(4, ivLen).ToArray();
    using var aes = Aes.Create();
    aes.Key = kdf.GetBytes(32);
    return Encoding.UTF8.GetString(aes.DecryptCbc(data.AsSpan(4 + ivLen), iv, PaddingMode.PKCS7));
}

/// Imports references from KopFix.dll (compiled against publicized copies) into Assembly-CSharp:
/// Assembly-CSharp types/members become the real definitions, other assemblies are matched by name.
class Importer
{
    private readonly ModuleDefinition _dst;
    private readonly Func<MethodReference, MethodDefinition> _kopMethod;
    private readonly Func<FieldReference, FieldDefinition> _kopField;

    public Importer(ModuleDefinition dst, Func<MethodReference, MethodDefinition> kopMethod,
                    Func<FieldReference, FieldDefinition> kopField)
    {
        _dst = dst;
        _kopMethod = kopMethod;
        _kopField = kopField;
    }

    private static string ScopeName(TypeReference t)
    {
        while (t.DeclaringType != null) t = t.DeclaringType;
        return t.Scope switch
        {
            AssemblyNameReference a => a.Name,
            ModuleDefinition m => m.Assembly.Name.Name,
            _ => t.Scope?.Name,
        };
    }

    public TypeReference Type(TypeReference t)
    {
        switch (t)
        {
            case GenericInstanceType g:
                var gi = new GenericInstanceType(Type(g.ElementType));
                foreach (var a in g.GenericArguments) gi.GenericArguments.Add(Type(a));
                return gi;
            case ArrayType a:
                return a.IsVector ? new ArrayType(Type(a.ElementType)) : new ArrayType(Type(a.ElementType), a.Rank);
            case ByReferenceType b:
                return new ByReferenceType(Type(b.ElementType));
            case PointerType p:
                return new PointerType(Type(p.ElementType));
            case GenericParameter gp:
                return gp;
            case TypeSpecification spec:
                throw new NotSupportedException("type " + spec.FullName);
        }
        if (ScopeName(t) == "Assembly-CSharp")
            return _dst.GetType(t.FullName) ?? throw new InvalidOperationException("no type " + t.FullName);
        if (ScopeName(t) == "KopFix")
            throw new InvalidOperationException("KopFix may not use its own types: " + t.FullName);
        return _dst.ImportReference(t);
    }

    public MethodReference Method(MethodReference m)
    {
        if (m is GenericInstanceMethod gim)
        {
            var inst = new GenericInstanceMethod(Method(gim.ElementMethod));
            foreach (var a in gim.GenericArguments) inst.GenericArguments.Add(Type(a));
            return inst;
        }
        if (m.DeclaringType.Name == "KopFix")
            return _kopMethod(m) ?? throw new InvalidOperationException("unknown KopFix call " + m.Name);
        var declaring = Type(m.DeclaringType);
        if (declaring is TypeDefinition def)
        {
            var found = def.Methods.FirstOrDefault(d => d.Name == m.Name && d.HasThis == m.HasThis
                && d.Parameters.Count == m.Parameters.Count && d.GenericParameters.Count == m.GenericParameters.Count
                && d.Parameters.Select(p => p.ParameterType.FullName).SequenceEqual(m.Parameters.Select(p => p.ParameterType.FullName)));
            return found ?? throw new InvalidOperationException($"no method {def.FullName}.{m.Name}");
        }
        var r = new MethodReference(m.Name, m.ReturnType, declaring)
        {
            HasThis = m.HasThis, ExplicitThis = m.ExplicitThis, CallingConvention = m.CallingConvention,
        };
        foreach (var gp in m.GenericParameters) r.GenericParameters.Add(new GenericParameter(gp.Name, r));
        r.ReturnType = Type(m.ReturnType);
        foreach (var p in m.Parameters) r.Parameters.Add(new ParameterDefinition(Type(p.ParameterType)));
        return r;
    }

    public FieldReference Field(FieldReference f)
    {
        if (f.DeclaringType.Name == "KopFix")
            return _kopField(f) ?? throw new InvalidOperationException("unknown KopFix field " + f.Name);
        var declaring = Type(f.DeclaringType);
        if (declaring is TypeDefinition def)
            return def.Fields.FirstOrDefault(x => x.Name == f.Name) ?? throw new InvalidOperationException($"no field {def.FullName}.{f.Name}");
        return new FieldReference(f.Name, Type(f.FieldType), declaring);
    }
}

/// Makes Cecil reuse the module's existing assembly references by name (the web player's mscorlib is
/// 2.0.5.0, the game references 2.0.0.0).
class ByNameImporter : DefaultMetadataImporter
{
    public ByNameImporter(ModuleDefinition module) : base(module) { }

    public override AssemblyNameReference ImportReference(AssemblyNameReference name) =>
        module.AssemblyReferences.FirstOrDefault(a => a.Name == name.Name) ?? base.ImportReference(name);
}

class ByNameImporterProvider : IMetadataImporterProvider
{
    public IMetadataImporter GetMetadataImporter(ModuleDefinition module) => new ByNameImporter(module);
}
