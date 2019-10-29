using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.PerformanceTesting;

namespace Unity.Entities.Determinism.Tests
{

// disable "never assigned" compiler warnings
#pragma warning disable 0649

    struct PaddingMasksTestData_Padding : IComponentData
    {
        public bool Offset0;
        public int  Offset4;
        public bool Offset8;
        public bool Offset9;
        public int  Offset12;
    }
    struct PaddingMasksTestData_MaskLength : IComponentData
    {
        public int Offset0;
        public int Offset4;
        public bool Offset8;
    }

    struct PaddingMaskBufferTestData_Padding : IBufferElementData
    {
        public bool Offset0;
        public int Offset4;
    }

#pragma warning restore 0649

    [TestFixture]
    public class PaddingMask_Tests : PaddingMasks_TestFixture
    {
        static readonly HashSet<int> MaskedFields_TypeMaskTestData = new HashSet<int>
        {
            0, 4, 5, 6, 7, 8, 9, 12, 13, 14, 15
        };

        static readonly HashSet<int> MaskedFields_TypeMaskBufferTestData = new HashSet<int>
        {
            0, 4, 5, 6, 7, 8, 12, 13, 14, 15
        };

        [Test]
        public void PaddingMasks_Entity_HasExpectedTypeMask()
        {
            // Entity is special cased in the TypeManager

            using( var expected = DeterminismTestUtility.GenerateMask(16, _ => true) )
            {
                var mask = Masks.GetTypeMask<Entity>();
                Assert.That(mask, Is.EquivalentTo(expected), DeterminismTestUtility.PrintAll(mask, expected));
            }
        }

        [Test]
        public void PaddingMasks_BufferTestData_HasExpectedTypeMask()
        {
            // Entity is special cased in the TypeManager

            using( var expected = DeterminismTestUtility.GenerateMask(16, i => MaskedFields_TypeMaskBufferTestData.Contains(i)) )
            {
                var mask = Masks.GetTypeMask<PaddingMaskBufferTestData_Padding>();
                Assert.That(mask, Is.EquivalentTo(expected), DeterminismTestUtility.PrintAll(mask, expected));
            }
        }

        [Test]
        public void PaddingMasks_WithPaddingTestData_HasExpectedTypeMask()
        {
            using( var expected = DeterminismTestUtility.GenerateMask(16, i => MaskedFields_TypeMaskTestData.Contains(i)) )
            {
                var mask = Masks.GetTypeMask<PaddingMasksTestData_Padding>();
                Assert.That(mask, Is.EquivalentTo(expected), DeterminismTestUtility.PrintAll(mask, expected));
            }
        }

        [Test]
        public void PaddingMasks_WithLengthTestData_HasExpectedMaskLengthAndPattern()
        {
            using( var expected = DeterminismTestUtility.GenerateMask(48, i => (i % 12 ) < 9 ))
            {
                var mask = Masks.GetTypeMask<PaddingMasksTestData_MaskLength>();
                Assert.That(mask, Is.EquivalentTo(expected), DeterminismTestUtility.PrintAll(mask, expected));
            }
        }

        [Test]
        public void PaddingMasks_PassingInvalidIndex_WillReturnEmptyArray()
        {
            Assert.IsTrue(NativeArrayUtility.IsInvalidOrEmpty(Masks.GetTypeMask(new TypeManager.TypeInfo())));
        }

        [Test, Performance]
        public void PaddingMasks_Performance_BuildFromAllTypesAndDispose()
        {
            var allTypes = TypeManager.GetAllTypes();
            Measure.Method(() =>
            {
                using (_ = PaddingMaskBuilder.BuildPaddingMasks(allTypes, Allocator.Temp)) { }
            })
            .Definition($"PaddingMaskBuilder_BuildAndDisposeMasks_{allTypes.Length}_Types")
            .MeasurementCount(5)
            .Run();
        }
    }
}
