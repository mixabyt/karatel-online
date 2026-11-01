#:package Mono.Cecil@0.11.6
// Dev helper: opcode histogram of the methods of one type, to spot IL newer compilers emit and old
// Mono verifiers may reject. dotnet run opcodes.cs -- <assembly> <Type> [method-name-substring]
using Mono.Cecil;

var asm = AssemblyDefinition.ReadAssembly(args[0]);
var type = asm.MainModule.GetTypes().First(t => t.Name == args[1]);
var counts = new SortedDictionary<string, int>();
foreach (var m in type.Methods.Where(m => m.HasBody && (args.Length < 3 || m.Name.Contains(args[2]))))
    foreach (var ins in m.Body.Instructions)
        counts[ins.OpCode.Name] = counts.GetValueOrDefault(ins.OpCode.Name) + 1;
Console.WriteLine(string.Join(" ", counts.Select(kv => $"{kv.Key}:{kv.Value}")));
