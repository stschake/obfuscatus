using NUnit.Framework;
using obfuscatus;

namespace obfuscatusTest
{
    
    [TestFixture]
    public class DisableReflectorTest : BaseObfuscatorTest
    {

        [Test]
        public void ProcessesTestTarget()
        {
            var target = GetTarget("String");
            var obfuscator = new DisableReflector();
            Assert.IsTrue(obfuscator.Process(target));
            target.Write("rewritten-disablereflector.exe");
        }

    }

}