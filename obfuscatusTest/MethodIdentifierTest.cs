using System;
using Mono.Cecil;
using NUnit.Framework;
using obfuscatus;

namespace obfuscatusTest
{
    
    [TestFixture]
    public class MethodIdentifierTest
    {
        private AssemblyDefinition _assembly;
       
        [TestFixtureSetUp]
        public void SetUp()
        {
            _assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("Test", new Version(1, 0)),
                                                          "Test", ModuleKind.Windows);
        }

        private MethodDefinition CreateDefinition(string name, MethodAttributes attributes, Type returnType)
        {
            return new MethodDefinition(name, attributes, _assembly.MainModule.Import(returnType));
        }

        [Test]
        public void RetainsMethodName()
        {
            var identifier = new MethodIdentifier(CreateDefinition("SomeName", MethodAttributes.Static | MethodAttributes.Public, typeof(void)));
            Assert.AreEqual("SomeName", identifier.Name);
        }

    }

}