using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using System.Linq;

namespace obfuscatus
{

    public class SuppressIldasm : IObfuscationMethod
    {
        public string Name
        {
            get { return "Suppress IL DASM"; }
        }

        public bool Process(AssemblyDefinition def)
        {
            var suppressIldasmType = typeof (SuppressIldasmAttribute);

            if (def.CustomAttributes.Any(ca => ca.AttributeType.FullName == suppressIldasmType.FullName))
                return false;

            var ctr = def.MainModule.Import(suppressIldasmType.GetConstructor(Type.EmptyTypes));
            def.CustomAttributes.Add(new CustomAttribute(ctr));
            return true;
        }
    }

}