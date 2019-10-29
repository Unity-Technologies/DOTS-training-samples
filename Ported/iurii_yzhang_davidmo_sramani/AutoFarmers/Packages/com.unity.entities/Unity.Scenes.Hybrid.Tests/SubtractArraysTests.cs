using NUnit.Framework;
using Unity.Collections;

namespace Unity.Scenes.Tests
{
    public class SubtractArraysTests
    {
        // equivalence
        [TestCase(new[] {1, 2, 3}, new[] {1, 2, 3}, new int[0], TestName = "ArrayDifference_OfIdenticalArrays_IsEmptyArray")]
        [TestCase(new[] {3, 2, 1}, new[] {1, 2, 3}, new int[0], TestName = "ArrayDifference_OfEquivalentArrays_IsEmptyArray")]
        [TestCase(new int[0], new int[0], new int[0], TestName = "ArrayDifference_OfEmptyArrays_IsEmptyArray")]

        // identity
        [TestCase(new int[0], new[] {1, 2, 3}, new int[0], TestName = "ArrayDifference_WhenMinuendIsEmpty_IsEmptyArray")]
        [TestCase(new[] {1, 2, 3}, new int[0], new[] {1, 2, 3}, TestName = "ArrayDifference_WhenSubtrahendIsEmpty_IsMinuend")]

        // distinct input sets
        [TestCase(new[] {1, 2, 3}, new[] {1, 2, 3, 4, 5}, new int[0], TestName = "ArrayDifference_WhenMinuendIsSubsetOfSubtrahend_IsEmptyArray")]
        [TestCase(new[] {1, 2, 3, 4}, new[] {1, 3}, new[] {4, 2}, TestName = "ArrayDifference_WhenSubtrahendIsSubsetOfMinuend_ExcludesTheSubset")]
        [TestCase(new[] {1, 2, 3, 4}, new[] {1, 3, 5}, new[] {4, 2}, TestName = "ArrayDifference_WhenSubtrahendContainsSubsetOfMinuend_ExcludesTheSubset")]

        // duplicates
        [TestCase(new[] {3, 2, 1, 0, 1, 2, 3}, new[] {0, 3}, new[] {2, 2, 1, 3, 1}, TestName = "ArrayDifference_WillReturnAllDuplicateDeltaItems")]
        [TestCase(new[] {1, 2, 3}, new[] {3, 2, 1, 1, 2, 3}, new int[0], TestName = "ArrayDifference_WhenSubtrahendIsMinuendWithDuplicates_IsEmptyArray")]
        [TestCase(new[] {3, 2, 1, 1, 2, 3}, new[] {1, 2, 3}, new int[]{1, 2, 3}, TestName = "ArrayDifference_WhenMinuendIsSubtrahendWithDuplicates_ReturnsDuplicates")]
        public void SubtractArrayProducesExpectedDelta(int[] array, int[] toExclude, int[] expected)
        {
            var minuend = new NativeArray<int>(array, Allocator.TempJob);
            var subtrahend = new NativeArray<int>(toExclude, Allocator.TempJob);

            var difference = LiveLinkSceneChangeTracker.SubtractArrays(minuend, subtrahend);
            var actual = difference.ToArray();

            minuend.Dispose();
            subtrahend.Dispose();
            difference.Dispose();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}