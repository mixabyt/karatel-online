#:package Mono.Cecil@0.11.6
// Dev helper: list places where the fan build overwrote a call with "pop; ldc.i4.x; nop..." filler,
// plus other nop runs, with a few instructions of context. dotnet run fanhacks.cs -- <assembly>
using Mono.Cecil;
using Mono.Cecil.Cil;

var asm = AssemblyDefinition.ReadAssembly(args[0]);
foreach (var type in asm.MainModule.GetTypes())
foreach (var m in type.Methods.Where(m => m.HasBody))
{
    var list = m.Body.Instructions;
    for (int i = 0; i < list.Count; i++)
    {
        if (list[i].OpCode != OpCodes.Nop) continue;
        int j = i;
        while (j < list.Count && list[j].OpCode == OpCodes.Nop) j++;
        if (j - i < 2) { i = j; continue; }
        Console.WriteLine($"{m.FullName}  nops {list[i].Offset:x4}..{list[j - 1].Offset:x4}");
        for (int k = Math.Max(0, i - 4); k < Math.Min(list.Count, j + 2); k++)
            Console.WriteLine($"      {list[k]}");
        i = j;
    }
}
