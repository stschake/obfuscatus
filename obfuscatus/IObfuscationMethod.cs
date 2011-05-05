using Mono.Cecil;

namespace obfuscatus
{

    public interface IObfuscationMethod
    {
        string Name { get; }

        bool Process(AssemblyDefinition def);
    }

}