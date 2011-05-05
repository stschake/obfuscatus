using Mono.Cecil;

namespace obfuscatus
{

    public interface INamingScheme
    {
        string GetName(MethodIdentifier mid);
        string GetOriginalName(MethodIdentifier mid);
    }

}