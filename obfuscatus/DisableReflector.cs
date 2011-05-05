using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace obfuscatus
{

    /// <summary>
    /// Reflector 7.0, as all previous versions, is borderline bad for a decompiler and will crash at the smallest nuisance.
    /// This proves the point.
    /// </summary>
    public class DisableReflector : IObfuscationMethod
    {
        public string Name
        {
            get { return "Disable Reflector"; }
        }

        public bool Process(AssemblyDefinition def)
        {
            foreach (var type in def.MainModule.Types)
                ProcessType(def, type);

            return true;
        }

        private static void ProcessType(AssemblyDefinition def, TypeDefinition type)
        {
            foreach (var method in type.Methods)
                ProcessMethod(def, type, method);
        }

        private static void ProcessMethod(AssemblyDefinition def, TypeDefinition type, MethodDefinition method)
        {
            if (!method.HasBody || method.Body.Instructions.Count < 1)
                return;

            var processor = method.Body.GetILProcessor();
            method.Body.Variables.Add(new VariableDefinition("dummy", def.MainModule.Import(typeof (void))));
            // invalid load
            processor.InsertFront(processor.Create(OpCodes.Ldloc, method.Body.Variables.Count - 1));
            method.Body.Variables.RemoveAt(method.Body.Variables.Count - 1);
            // jump to skip on execution
            processor.InsertFront(processor.Create(OpCodes.Br, processor.Body.Instructions[1]));
        }
    }

}