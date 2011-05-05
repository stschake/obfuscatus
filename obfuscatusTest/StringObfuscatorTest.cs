using System;
using Mono.Cecil.Cil;
using NUnit.Framework;
using obfuscatus;
using obfuscatusTest.Targets;

namespace obfuscatusTest
{

    [TestFixture]
    public class StringObfuscatorTest : BaseObfuscatorTest
    {

        private class BranchesTestTarget : BaseTestTarget
        {
            protected override void GenerateBody()
            {
                Nop(125);
                LoadString("Test A");
                LoadString("Test B");
                Processor.InsertFront(Processor.Create(OpCodes.Br_S,
                                                       EntryMethod.Body.Instructions[
                                                           EntryMethod.Body.Instructions.Count - 1]));
                Ret();
            }
        }

        [Test]
        public void DoesNotCorruptBranches()
        {
            var targetGenerator = new BranchesTestTarget();
            var target = targetGenerator.Generate();
            var sizeBefore = targetGenerator.EntryMethod.Body.Instructions.Count;

            var obfuscator = new StringObfuscator();
            Assert.IsTrue(obfuscator.Process(target));

            var sizeAfter = targetGenerator.EntryMethod.Body.Instructions.Count;
            Assert.Greater(sizeAfter, sizeBefore);
                
            BranchHelper.VerifyBranches(targetGenerator.EntryMethod);
        }
        
        [Test]
        public void ProcessesTestTarget()
        {
            var target = GetTarget("String");
            var obfuscator = new StringObfuscator();
            Assert.IsTrue(obfuscator.Process(target));
        }

        [Test]
        public void IgnoresAccessibility()
        {
            var target = GetTarget("String");
            var obfuscator = new StringObfuscator();
            Assert.IsTrue(obfuscator.Process(target));
            Assert.Contains("Public", obfuscator.FoundStrings, "Missed public accessibility");
            Assert.Contains("Private", obfuscator.FoundStrings, "Missed private accessibility");
            Assert.Contains("Protected", obfuscator.FoundStrings, "Missed protected accessibility");
            Assert.Contains("Internal", obfuscator.FoundStrings, "Missed internal accessibility");
        }

    }

}
