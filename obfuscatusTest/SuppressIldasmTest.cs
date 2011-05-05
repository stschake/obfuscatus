using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using obfuscatus;

namespace obfuscatusTest
{

    [TestFixture]
    public class SuppressIldasmTest : BaseObfuscatorTest
    {

        [Test]
        public void ProcessesTestTarget()
        {
            var target = GetTarget("String");
            var obfuscator = new SuppressIldasm();
            Assert.IsTrue(obfuscator.Process(target));
            target.Write("string-suppressildasm.exe");
        }

        [Test]
        public void SuppressesIldasm()
        {
            string toolPath = "Tools//ildasm//";
            const string tempPath = "SuppressesIldasm.exe";

            // if the resource file is missing, ildasm will fail silently
            if (!File.Exists(toolPath + "ildasm.exe") || !File.Exists(toolPath + "ILDasmrc.dll"))
                Assert.Fail("Test requires ildasm tool");
            toolPath += "ildasm.exe";

            Process toolProcess = null;
            try
            {
                // process the test target first and write the resulting assembly
                var target = GetTarget("String");
                var obfuscator = new SuppressIldasm();
                Assert.IsTrue(obfuscator.Process(target));
                target.Write(tempPath);

                // start the tool
                toolProcess = new Process
                                  {
                                      StartInfo = new ProcessStartInfo(toolPath)
                                                      {
                                                          Arguments = "/TEXT " + tempPath,
                                                          UseShellExecute = false,
                                                          CreateNoWindow = true,
                                                          RedirectStandardError = true
                                                      }
                                  };
                toolProcess.Start();
                if (!toolProcess.WaitForExit(10000))
                    Assert.Fail("Tool process didn't exit in 10 seconds");
                
                // test if ildasm correctly refused to process the assembly
                var error = toolProcess.StandardError.ReadToEnd();
                Assert.IsNotNullOrEmpty(error);
                Assert.AreEqual("Protected module -- cannot disassemble\r\n", error);
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);

                if (toolProcess != null && !toolProcess.HasExited)
                    toolProcess.Kill();
            }
        }

    }

}