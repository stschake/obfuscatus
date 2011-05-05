using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace obfuscatus
{

    public class StringObfuscator : IObfuscationMethod
    {
        public string Name
        {
            get { return "String Obfuscator"; }
        }

        public List<string> FoundStrings { get; private set; }

        private MethodDefinition _getMethod;
        private FieldDefinition _initializedField;
        private FieldDefinition _listField;
        private MethodDefinition _initializeMethod;
        private TypeDefinition _type;

        public bool Process(AssemblyDefinition assembly)
        {
            var types = assembly.MainModule.Types;

            FoundStrings = new List<string>();
            AddHandler(assembly);

            foreach (var type in types)
                ProcessType(type);

            FinalizeHandler(assembly);
            return true;
        }

        private void AddHandler(AssemblyDefinition assembly)
        {
            _type = new TypeDefinition("obfuscatus", "StringHandler",
                                       TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AutoClass |
                                       TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit |
                                       TypeAttributes.Abstract, assembly.MainModule.Import(typeof (object)));
            

            _initializedField = new FieldDefinition("_initialized", FieldAttributes.Private | FieldAttributes.Static,
                                                       assembly.MainModule.Import(typeof(bool)));
            _listField = new FieldDefinition("_list", FieldAttributes.Private | FieldAttributes.Static,
                                                assembly.MainModule.Import(typeof(List<string>)));
            _initializeMethod = new MethodDefinition("Initialize", MethodAttributes.Private | MethodAttributes.Static,
                                            assembly.MainModule.Import(typeof(void)));
            _type.Methods.Add(_initializeMethod);
            _type.Fields.Add(_initializedField);
            _type.Fields.Add(_listField);

            _getMethod = new MethodDefinition("Get", MethodAttributes.Public | MethodAttributes.Static,
                                              assembly.MainModule.Import(typeof(string)));
            _getMethod.Parameters.Add(new ParameterDefinition(assembly.MainModule.Import(typeof(int))));

            _type.Methods.Add(_getMethod);

            {
                var processor = _getMethod.Body.GetILProcessor();
                processor.Body.InitLocals = true;
                processor.Body.Variables.Add(new VariableDefinition(assembly.MainModule.Import(typeof (string))));
                processor.Body.Variables.Add(new VariableDefinition(assembly.MainModule.Import(typeof (bool))));
                processor.Emit(OpCodes.Ldsfld, _initializedField);
                processor.Emit(OpCodes.Stloc_1);
                processor.Emit(OpCodes.Ldloc_1);
                processor.Emit(OpCodes.Nop);
                processor.Emit(OpCodes.Call, _initializeMethod);
                processor.Emit(OpCodes.Ldsfld, _listField);
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Callvirt, assembly.MainModule.Import(typeof (List<string>).GetMethod("get_Item")));
                processor.Emit(OpCodes.Stloc_0);
                processor.Emit(OpCodes.Ldloc_0);
                processor.Emit(OpCodes.Ret);
                // fix up initialization check
                processor.Replace(processor.Body.Instructions[3],
                                  processor.Create(OpCodes.Brtrue_S, processor.Body.Instructions[5]));
            }
        }

        private void FinalizeHandler(AssemblyDefinition assembly)
        {
            var processor = _initializeMethod.Body.GetILProcessor();
            processor.Emit(OpCodes.Ldc_I4, FoundStrings.Count);
            processor.Emit(OpCodes.Newobj, assembly.MainModule.Import(typeof(List<string>).GetConstructors()[1]));
            processor.Emit(OpCodes.Stsfld, _listField);
            foreach (var str in FoundStrings)
            {
                processor.Emit(OpCodes.Ldsfld, _listField);
                processor.Emit(OpCodes.Ldstr, str);
                processor.Emit(OpCodes.Callvirt, assembly.MainModule.Import(typeof(List<string>).GetMethod("Add")));
            }
            processor.Emit(OpCodes.Ldc_I4_1);
            processor.Emit(OpCodes.Stsfld, _initializedField);
            processor.Emit(OpCodes.Ret);
            assembly.MainModule.Types.Add(_type);
        }

        private void ProcessType(TypeDefinition type)
        {
            if (!type.HasMethods)
                return;

            foreach (var method in type.Methods)
                ProcessMethod(method);
        }

        private void ProcessMethod(MethodDefinition method)
        {
            if (!method.HasBody)
                return;

            var processor = method.Body.GetILProcessor();
            var instructionsToReplace = new Dictionary<Instruction, int>();
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Code != Code.Ldstr)
                    continue;

                if (!(instruction.Operand is string))
                    continue;

                FoundStrings.Add(instruction.Operand as string);
                instructionsToReplace.Add(instruction, FoundStrings.Count - 1);
            }

            foreach (var kvp in instructionsToReplace)
            {
                processor.InsertAfter(kvp.Key, processor.Create(OpCodes.Call, _getMethod));
                processor.Replace(kvp.Key, processor.Create(OpCodes.Ldc_I4, kvp.Value));
            }
        }
    }

}