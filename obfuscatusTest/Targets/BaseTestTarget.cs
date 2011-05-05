using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace obfuscatusTest.Targets
{

    /// <summary>
    /// a base class for dynamically generated test targets
    /// </summary>
    public abstract class BaseTestTarget
    {
        public AssemblyDefinition Assembly;
        public TypeDefinition ProgramType;
        public MethodDefinition EntryMethod;
        public ILProcessor Processor;

        public AssemblyDefinition Generate()
        {
            Assembly =
                AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("BaseTestTarget", new Version(1, 0)),
                                                  "JumpTestTarget", ModuleKind.Console);

            ProgramType = new TypeDefinition("BaseTestTarget", "Program",
                                                 TypeAttributes.Public | TypeAttributes.AnsiClass |
                                                 TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit |
                                                 TypeAttributes.Abstract, Assembly.MainModule.Import(typeof(object)));

            EntryMethod = new MethodDefinition("Main", MethodAttributes.Public | MethodAttributes.Static,
                                                   Assembly.MainModule.Import(typeof(void)));
            Processor = EntryMethod.Body.GetILProcessor();
            GenerateBody();

            ProgramType.Methods.Add(EntryMethod);
            Assembly.MainModule.Types.Add(ProgramType);
            Assembly.MainModule.EntryPoint = EntryMethod;
            return Assembly;
        }

        protected abstract void GenerateBody();

        protected void Nop(int count)
        {
            for (int i = 0; i < count; i++)
                Processor.Emit(OpCodes.Nop);
        }

        protected void LoadString(string value)
        {
            Processor.Emit(OpCodes.Ldstr, value);
        }

        protected void Ret()
        {
            Processor.Emit(OpCodes.Ret);
        }
    }

}