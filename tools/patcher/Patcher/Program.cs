// Patches the 2014 client's Assembly-CSharp.dll so it talks to our own server.
//   Patcher <Assembly-CSharp.dll in> <KopatelPatch.dll> <out.dll> <search dir>...
// What it changes is listed in docs/protocol.md.
using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

static class Program
{
	static ModuleDefinition _target;
	static TypeDefinition _patchType;
	static readonly Dictionary<IMemberDefinition, IMemberDefinition> _cloned = new Dictionary<IMemberDefinition, IMemberDefinition>();

	static int Main(string[] args)
	{
		if (args.Length < 3)
		{
			Console.Error.WriteLine("usage: Patcher <Assembly-CSharp.dll> <KopatelPatch.dll> <out.dll> [search dirs...]");
			return 2;
		}
		var resolver = new DefaultAssemblyResolver();
		foreach (string dir in args.Skip(3))
		{
			resolver.AddSearchDirectory(dir);
		}
		var readerParams = new ReaderParameters { AssemblyResolver = resolver, ReadingMode = ReadingMode.Immediate };
		AssemblyDefinition target = AssemblyDefinition.ReadAssembly(args[0], readerParams);
		AssemblyDefinition patch = AssemblyDefinition.ReadAssembly(args[1], readerParams);
		_target = target.MainModule;

		if (_target.GetType("KopatelPatch") != null)
		{
			Console.Error.WriteLine("already patched");
			return 1;
		}
		CloneType(patch.MainModule.GetType("KopatelPatch"));
		MethodReference host = _patchType.Methods.First(m => m.Name == "Host");

		int strings = RewriteHostStrings(host);
		WrapReturns("App", new[] { "GetCurScriptsUrl", "GetCurScriptsUrlSmall", "GetCurSavesUrl" }, host);
		ForceWebsiteSelfHosted();
		SwitchPhotonToTcp();
		PlainKeyEncrypt();
		BrowserFullScreen();

		target.Write(args[2]);
		Console.WriteLine("patched: " + strings + " host strings -> " + args[2]);
		return 0;
	}

	// ---- copying KopatelPatch into Assembly-CSharp ----

	static void CloneType(TypeDefinition src)
	{
		_patchType = new TypeDefinition(src.Namespace, src.Name, src.Attributes, _target.ImportReference(src.BaseType));
		_target.Types.Add(_patchType);
		foreach (FieldDefinition f in src.Fields)
		{
			var nf = new FieldDefinition(f.Name, f.Attributes, _target.ImportReference(f.FieldType));
			_patchType.Fields.Add(nf);
			_cloned[f] = nf;
		}
		foreach (MethodDefinition m in src.Methods)
		{
			var nm = new MethodDefinition(m.Name, m.Attributes, _target.ImportReference(m.ReturnType));
			foreach (ParameterDefinition p in m.Parameters)
			{
				nm.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, _target.ImportReference(p.ParameterType)));
			}
			_patchType.Methods.Add(nm);
			_cloned[m] = nm;
		}
		foreach (MethodDefinition m in src.Methods)
		{
			var nm = (MethodDefinition)_cloned[m];
			nm.Body.InitLocals = m.Body.InitLocals;
			foreach (VariableDefinition v in m.Body.Variables)
			{
				nm.Body.Variables.Add(new VariableDefinition(_target.ImportReference(v.VariableType)));
			}
			var map = new Dictionary<Instruction, Instruction>();
			ILProcessor il = nm.Body.GetILProcessor();
			foreach (Instruction ins in m.Body.Instructions)
			{
				Instruction copy = CloneInstruction(ins, nm);
				map[ins] = copy;
				il.Append(copy);
			}
			foreach (Instruction copy in nm.Body.Instructions)
			{
				if (copy.Operand is Instruction t)
				{
					copy.Operand = map[t];
				}
				else if (copy.Operand is Instruction[] ts)
				{
					copy.Operand = ts.Select(x => map[x]).ToArray();
				}
			}
			foreach (ExceptionHandler h in m.Body.ExceptionHandlers)
			{
				nm.Body.ExceptionHandlers.Add(new ExceptionHandler(h.HandlerType)
				{
					TryStart = map[h.TryStart], TryEnd = h.TryEnd == null ? null : map[h.TryEnd],
					HandlerStart = map[h.HandlerStart], HandlerEnd = h.HandlerEnd == null ? null : map[h.HandlerEnd],
					CatchType = h.CatchType == null ? null : _target.ImportReference(h.CatchType)
				});
			}
		}
	}

	static Instruction CloneInstruction(Instruction ins, MethodDefinition owner)
	{
		object op = ins.Operand;
		switch (op)
		{
		case null:
			return Instruction.Create(ins.OpCode);
		case FieldReference f:
			return Instruction.Create(ins.OpCode, f.DeclaringType.Name == "KopatelPatch" ? (FieldReference)_cloned[f.Resolve()] : _target.ImportReference(f));
		case MethodReference m:
			return Instruction.Create(ins.OpCode, m.DeclaringType.Name == "KopatelPatch" ? (MethodReference)_cloned[m.Resolve()] : _target.ImportReference(m));
		case TypeReference t:
			return Instruction.Create(ins.OpCode, _target.ImportReference(t));
		case VariableDefinition v:
			return Instruction.Create(ins.OpCode, owner.Body.Variables[v.Index]);
		case ParameterDefinition p:
			return Instruction.Create(ins.OpCode, owner.Parameters[p.Index]);
		case Instruction t:
			return Instruction.Create(ins.OpCode, t);  // fixed up after all are copied
		case Instruction[] ts:
			return Instruction.Create(ins.OpCode, ts);
		case string s:
			return Instruction.Create(ins.OpCode, s);
		case sbyte b:
			return Instruction.Create(ins.OpCode, b);
		case byte b:
			return Instruction.Create(ins.OpCode, b);
		case int i:
			return Instruction.Create(ins.OpCode, i);
		case long l:
			return Instruction.Create(ins.OpCode, l);
		case float fl:
			return Instruction.Create(ins.OpCode, fl);
		case double d:
			return Instruction.Create(ins.OpCode, d);
		default:
			throw new NotSupportedException("operand " + op.GetType());
		}
	}

	// ---- the patches ----

	static readonly string[] KnownHosts =
	{
		"vk.diggerworld.ru", "vk2.diggerworld.ru", "server1.diggerworld.ru", "www.diggerworld.ru",
		"diggerworld.ru", "62.75.224.107", "188.138.112.218", "176.60.181.71", "178.63.48.23"
	};

	static bool IsHostString(string s)
	{
		string rest = s.StartsWith("http://") ? s.Substring(7) : s.StartsWith("https://") ? s.Substring(8) : s;
		return KnownHosts.Any(h => rest == h || rest.StartsWith(h + "/"));
	}

	static IEnumerable<TypeDefinition> AllTypes(IEnumerable<TypeDefinition> types)
	{
		foreach (TypeDefinition t in types)
		{
			yield return t;
			foreach (TypeDefinition n in AllTypes(t.NestedTypes))
			{
				yield return n;
			}
		}
	}

	// Each matching ldstr becomes ldstr + call KopatelPatch.Host. Constructors are left alone:
	// MonoBehaviour constructors run while Unity deserializes, where Application.absoluteURL
	// must not be touched; the getters wrapped below cover App's URL arrays.
	static int RewriteHostStrings(MethodReference host)
	{
		int count = 0;
		foreach (TypeDefinition t in AllTypes(_target.Types.ToList()))
		{
			if (t == _patchType)
			{
				continue;
			}
			foreach (MethodDefinition m in t.Methods.Where(m => m.HasBody && !m.IsConstructor))
			{
				ILProcessor il = m.Body.GetILProcessor();
				foreach (Instruction ins in m.Body.Instructions.ToList())
				{
					if (ins.OpCode == OpCodes.Ldstr && IsHostString((string)ins.Operand))
					{
						Console.WriteLine("  " + t.FullName + "::" + m.Name + "  \"" + ins.Operand + "\"");
						il.InsertAfter(ins, il.Create(OpCodes.Call, host));
						count++;
					}
				}
			}
		}
		return count;
	}

	static void WrapReturns(string typeName, string[] methods, MethodReference host)
	{
		TypeDefinition type = _target.GetType(typeName);
		foreach (string name in methods)
		{
			MethodDefinition m = type.Methods.Single(x => x.Name == name);
			ILProcessor il = m.Body.GetILProcessor();
			foreach (Instruction ret in m.Body.Instructions.Where(i => i.OpCode == OpCodes.Ret).ToList())
			{
				// Insert before ret but keep branches that target ret landing on the call.
				Instruction call = il.Create(OpCodes.Call, host);
				il.InsertBefore(ret, call);
				SwapIdentity(m, ret, call);
			}
		}
	}

	// After inserting `call` before `ret`, branches that pointed at `ret` must point at `call`.
	static void SwapIdentity(MethodDefinition m, Instruction oldTarget, Instruction newTarget)
	{
		foreach (Instruction i in m.Body.Instructions)
		{
			if (i.Operand == oldTarget && i != newTarget)
			{
				i.Operand = newTarget;
			}
			else if (i.Operand is Instruction[] ts)
			{
				for (int k = 0; k < ts.Length; k++)
				{
					if (ts[k] == oldTarget)
					{
						ts[k] = newTarget;
					}
				}
			}
		}
	}

	// App.Start: KopatelPatch.PrefetchPolicy(); CurPlatform = WEBSITE; LocalTest = true
	// -> UseMyServer(<our host>, 5055, "Master").
	static void ForceWebsiteSelfHosted()
	{
		TypeDefinition app = _target.GetType("App");
		MethodDefinition start = app.Methods.Single(m => m.Name == "Start");
		FieldDefinition platform = app.Fields.Single(f => f.Name == "CurPlatform");
		FieldDefinition localTest = app.Fields.Single(f => f.Name == "LocalTest");
		int website = Convert.ToInt32(app.NestedTypes.Single(t => t.Name == "Platform").Fields.Single(f => f.Name == "WEBSITE").Constant);
		ILProcessor il = start.Body.GetILProcessor();
		Instruction first = start.Body.Instructions[0];
		il.InsertBefore(first, il.Create(OpCodes.Call, _patchType.Methods.First(m => m.Name == "PrefetchPolicy")));
		il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
		il.InsertBefore(first, il.Create(OpCodes.Ldc_I4, website));
		il.InsertBefore(first, il.Create(OpCodes.Stfld, platform));
		il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
		il.InsertBefore(first, il.Create(OpCodes.Ldc_I4_1));
		il.InsertBefore(first, il.Create(OpCodes.Stfld, localTest));
	}

	// PhotonNetwork..cctor: new NetworkingPeer(handler, "", ConnectionProtocol.Udp) -> Tcp.
	static void SwitchPhotonToTcp()
	{
		MethodDefinition cctor = _target.GetType("PhotonNetwork").Methods.Single(m => m.Name == ".cctor");
		Instruction ctor = cctor.Body.Instructions.Single(i => i.OpCode == OpCodes.Newobj && ((MethodReference)i.Operand).DeclaringType.Name == "NetworkingPeer");
		Instruction proto = ctor.Previous;
		if (proto.OpCode != OpCodes.Ldc_I4_0)
		{
			throw new Exception("unexpected protocol argument: " + proto);
		}
		proto.OpCode = OpCodes.Ldc_I4_1;
	}

	// MainMenu.ToggleFullScreen() (F12) -> KopatelPatch.ToggleFullScreen(): browser full screen,
	// which Esc does not leave, instead of the player's.
	static void BrowserFullScreen()
	{
		MethodDefinition m = _target.GetType("MainMenu").Methods.Single(x => x.Name == "ToggleFullScreen");
		m.Body.Instructions.Clear();
		m.Body.ExceptionHandlers.Clear();
		m.Body.Variables.Clear();
		ILProcessor il = m.Body.GetILProcessor();
		il.Append(il.Create(OpCodes.Call, _patchType.Methods.First(x => x.Name == "ToggleFullScreen")));
		il.Append(il.Create(OpCodes.Ret));
	}

	// App.KeyEncrypt(message) -> return message (the RSA private key is long gone).
	static void PlainKeyEncrypt()
	{
		MethodDefinition m = _target.GetType("App").Methods.Single(x => x.Name == "KeyEncrypt");
		m.Body.Instructions.Clear();
		m.Body.ExceptionHandlers.Clear();
		m.Body.Variables.Clear();
		ILProcessor il = m.Body.GetILProcessor();
		il.Append(il.Create(OpCodes.Ldarg_1));
		il.Append(il.Create(OpCodes.Ret));
	}
}
