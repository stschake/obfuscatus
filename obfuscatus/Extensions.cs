using Mono.Cecil.Cil;

namespace obfuscatus
{

    public static class ILProcessorExtensions
    {
        public static void InsertFront(this ILProcessor processor, Instruction instr)
        {
            if (processor.Body.Instructions.Count <= 0)
                processor.Append(instr);
            else
                processor.InsertBefore(processor.Body.Instructions[0], instr);
        }
    }

}