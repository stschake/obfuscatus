using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace obfuscatusTest
{

    public static class BranchHelper
    {
        
        /// <summary>
        /// Verifies branches of a method and tries to detect corruption due to obfuscation
        /// </summary>
        /// <param name="method">the method to check, must have a body</param>
        public static void VerifyBranches(MethodDefinition method)
        {
            Assert.True(method.HasBody);

            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode.Code == Code.Br || instruction.OpCode.Code == Code.Br_S)
                {
                    Assert.IsNotNull(instruction.Operand, "Branch operand is null");
                    var operand = instruction.Operand as Instruction;
                    Assert.IsNotNull(operand, "Branch operand is not an instruction");
                    Assert.AreNotEqual(instruction, operand, "Branch instruction and operand are equal");
                    Assert.AreNotEqual(instruction.Offset, operand.Offset, "Branch instruction has same offset as target");
                }
            }
        }

    }

}