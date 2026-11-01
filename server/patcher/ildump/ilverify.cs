#:package Mono.Cecil@0.11.6
// Dev helper: find methods whose IL has inconsistent stack depths (what Mono's verifier rejects
// with "Could not merge stacks"). dotnet run ilverify.cs -- <assembly>
using Mono.Cecil;
using Mono.Cecil.Cil;

var asm = AssemblyDefinition.ReadAssembly(args[0]);
int bad = 0;
foreach (var type in asm.MainModule.GetTypes())
foreach (var m in type.Methods.Where(m => m.HasBody))
{
    string problem;
    try { problem = Check(m.Body); }
    catch (Exception e) { problem = "analysis failed: " + e.Message; }
    if (problem != null) { bad++; Console.WriteLine($"{m.FullName}: {problem}"); }
}
Console.WriteLine($"{bad} suspicious method(s)");

static string Check(MethodBody body)
{
    var depth = new Dictionary<Instruction, int>();
    var work = new Stack<(Instruction, int)>();
    work.Push((body.Instructions[0], 0));
    foreach (var h in body.ExceptionHandlers)
    {
        work.Push((h.HandlerStart, h.HandlerType == ExceptionHandlerType.Catch || h.HandlerType == ExceptionHandlerType.Filter ? 1 : 0));
        if (h.FilterStart != null) work.Push((h.FilterStart, 1));
    }
    while (work.Count > 0)
    {
        var (ins, d) = work.Pop();
        while (ins != null)
        {
            if (depth.TryGetValue(ins, out var known))
            {
                if (known != d) return $"stack {known} vs {d} at {ins.Offset:x4} ({ins})";
                break;
            }
            depth[ins] = d;
            d = d - Pops(ins, body) + Pushes(ins);
            if (d < 0) return $"stack underflow at {ins.Offset:x4} ({ins})";
            var oc = ins.OpCode;
            if (oc.Code == Code.Leave || oc.Code == Code.Leave_S) { work.Push(((Instruction)ins.Operand, 0)); break; }
            if (oc.FlowControl == FlowControl.Branch) { ins = (Instruction)ins.Operand; continue; }
            if (oc.FlowControl == FlowControl.Cond_Branch)
            {
                if (ins.Operand is Instruction[] targets) foreach (var t in targets) work.Push((t, d));
                else work.Push(((Instruction)ins.Operand, d));
            }
            if (oc.FlowControl == FlowControl.Return || oc.FlowControl == FlowControl.Throw
                || oc.Code == Code.Endfinally || oc.Code == Code.Endfilter) break;
            ins = ins.Next;
        }
    }
    return null;
}

static int Pops(Instruction ins, MethodBody body)
{
    var oc = ins.OpCode;
    if (oc.Code == Code.Ret) return body.Method.ReturnType.MetadataType == MetadataType.Void ? 0 : 1;
    if (oc.FlowControl == FlowControl.Call)
    {
        var mr = (IMethodSignature)ins.Operand;
        int n = mr.Parameters.Count;
        if (mr.HasThis && oc.Code != Code.Newobj) n++;
        if (oc.Code == Code.Calli) n++;
        return n;
    }
    return oc.StackBehaviourPop switch
    {
        StackBehaviour.Pop0 => 0,
        StackBehaviour.Pop1 or StackBehaviour.Popi or StackBehaviour.Popref => 1,
        StackBehaviour.Pop1_pop1 or StackBehaviour.Popi_pop1 or StackBehaviour.Popi_popi or StackBehaviour.Popi_popi8
            or StackBehaviour.Popi_popr4 or StackBehaviour.Popi_popr8 or StackBehaviour.Popref_pop1 or StackBehaviour.Popref_popi => 2,
        StackBehaviour.Popi_popi_popi or StackBehaviour.Popref_popi_popi or StackBehaviour.Popref_popi_popi8
            or StackBehaviour.Popref_popi_popr4 or StackBehaviour.Popref_popi_popr8 or StackBehaviour.Popref_popi_popref
            => 3,
        StackBehaviour.PopAll => 0,
        _ => 0,
    };
}

static int Pushes(Instruction ins)
{
    var oc = ins.OpCode;
    if (oc.FlowControl == FlowControl.Call)
    {
        if (oc.Code == Code.Newobj) return 1;
        return ((IMethodSignature)ins.Operand).ReturnType.MetadataType == MetadataType.Void ? 0 : 1;
    }
    return oc.StackBehaviourPush switch
    {
        StackBehaviour.Push0 => 0,
        StackBehaviour.Push1_push1 => 2,
        _ => 1,
    };
}
