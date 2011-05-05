using System.IO;
using Mono.Cecil;
using NUnit.Framework;

namespace obfuscatusTest
{

    public abstract class BaseObfuscatorTest
    {
        protected AssemblyDefinition GetTarget(string target)
        {
            var filePath = "Targets" + Path.DirectorySeparatorChar + target + "TestTarget";
            if (!File.Exists(filePath + ".dll") && !File.Exists(filePath + ".exe"))
                throw new FileNotFoundException("Test target " + target + " was not found", filePath);

            filePath += File.Exists(filePath + ".dll") ? ".dll" : ".exe";
            var def = AssemblyDefinition.ReadAssembly(filePath);
            Assert.NotNull(def);
            return def;
        }
    }

}