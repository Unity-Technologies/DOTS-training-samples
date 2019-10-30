using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Mono.Cecil.Cil;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public class CecilHelpersTests
    {
        [Test]
        public void MatchStartInExactMatch()
        {
            var instructions = MakeInstructions(Pattern1Instructions);
            var pattern = Pattern1;

            Assert.IsTrue(CecilHelpers.IsStartOfSequence(instructions.First(), pattern, out var match));
            CollectionAssert.AreEquivalent(instructions, match);
        }

        [Test]
        public void MatchEndInExactMatch()
        {
            var instructions = MakeInstructions(Pattern1Instructions);
            var pattern = Pattern1;

            Assert.IsTrue(CecilHelpers.IsEndOfSequence(instructions.Last(), pattern, out var match));
            CollectionAssert.AreEquivalent(instructions, match);
        }


        private static Func<Instruction, bool>[] Pattern1 =>
            new Func<Instruction, bool>[]
            {
                i => i.OpCode == OpCodes.Add,
                i => i.OpCode == OpCodes.Dup,
            };

        private static Instruction[] Pattern1Instructions => MakeInstructions(new[]{Instruction.Create(OpCodes.Add), Instruction.Create(OpCodes.Dup)});

        private static Instruction[] MakeInstructions(params Instruction[] inputs)
        {
            for (int i = 0; i != inputs.Length - 1;i++)
            {
                inputs[i].Next = inputs[i + 1];
                inputs[i + 1].Previous = inputs[i];
            }

            return inputs;
        }
    }
}
