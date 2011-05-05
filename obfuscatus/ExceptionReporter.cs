using Mono.Cecil;
using Mono.Cecil.Cil;

namespace obfuscatus
{

    public class ExceptionReporter : IObfuscationMethod
    {
        public string Name
        {
            get { return "Exception Handler"; }
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

            var newHandler = new ExceptionHandler(ExceptionHandlerType.Catch);
            newHandler.TryStart = method.Body.Instructions[0];
            newHandler.TryEnd = method.Body.Instructions[method.Body.Instructions.Count - 1];
            // todo: add handler to body (create new static class to avoid code bloat) and register bounds with newHandler
            method.Body.ExceptionHandlers.Add(newHandler);
        }
    }

}