using Mono.Cecil;

namespace obfuscatus
{

    public class MethodIdentifier
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string[] ParameterTypes { get; set; }

        public MethodIdentifier(MethodDefinition mdef)
        {
            Name = mdef.Name;
            if (!mdef.IsStatic && mdef.DeclaringType != null)
                Class = mdef.DeclaringType.Name;
            ParameterTypes = new string[mdef.Parameters.Count];
            for (int i = 0; i < mdef.Parameters.Count; i++)
                ParameterTypes[i] = mdef.Parameters[i].ParameterType.FullName;
        }
    }

}