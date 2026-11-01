#:package Mono.Cecil@0.11.6
// Dev helper: print one method's IL with offsets and a simple stack-depth trace.
// dotnet run ildump.cs -- <assembly> <Type> <Method>
using Mono.Cecil;
using Mono.Cecil.Cil;

var asm = AssemblyDefinition.ReadAssembly(args[0]);
var type = asm.MainModule.GetTypes().First(t => t.Name == args[1]);
foreach (var m in type.Methods.Where(m => m.Name == args[2] && m.HasBody))
{
    Console.WriteLine($"== {m.FullName} maxstack={m.Body.MaxStackSize} handlers={m.Body.ExceptionHandlers.Count}");
    foreach (var h in m.Body.ExceptionHandlers)
        Console.WriteLine($"   {h.HandlerType} try {h.TryStart.Offset:x4}-{h.TryEnd?.Offset:x4} handler {h.HandlerStart.Offset:x4}-{h.HandlerEnd?.Offset:x4}");
    foreach (var ins in m.Body.Instructions)
        Console.WriteLine($"   {ins}");
}
