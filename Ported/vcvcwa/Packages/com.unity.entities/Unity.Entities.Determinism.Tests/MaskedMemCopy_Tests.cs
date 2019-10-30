using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Determinism.Tests
{
    struct MemMaskTestData_Mask : IComponentData
    {
        public bool Offset0;
        public int Offset4;
        public bool Offset8;
    }

    [TestFixture]
    public class MaskedMemCopy_Tests : PaddingMasks_TestFixture
    {
        const int DefaultDataSize = 31;
        const int DefaultMaskSize = 16;

        static readonly HashSet<int> MaskedFields_MemMaskTestData = new HashSet<int>
        {
            0, 4, 5, 6, 7, 8 /* repeats mod 12 */
        };

        [Test]
        public void MaskedMemCpy_CopyWithAllMaskBitsSet_WillYieldSameSequence()
        {
            using (var data = DeterminismTestUtility.GenerateIdentity(DefaultDataSize))
            using (var mask = DeterminismTestUtility.GenerateMask(DefaultMaskSize, _ => true))

            using (var result = MaskedMemCopy.CreateMaskedBuffer(data, mask, Allocator.Temp))
            {
                Assert.That(result, Is.EquivalentTo(data), DeterminismTestUtility.PrintAll(result, data));
            }
        }

        [Test]
        public void MaskedMemCpy_CopyWithAllMaskBitsZero_WillYieldZeroSequence()
        {
            using (var data = DeterminismTestUtility.GenerateIdentity(DefaultDataSize))
            using (var mask = DeterminismTestUtility.GenerateMask(DefaultMaskSize, _ => false))
            using (var expected = DeterminismTestUtility.GenerateData(DefaultDataSize, _ => 0))

            using (var result = MaskedMemCopy.CreateMaskedBuffer(data, mask, Allocator.Temp))
            {
                Assert.That(result, Is.EquivalentTo(expected), DeterminismTestUtility.PrintAll(result, expected));
            }
        }

        [Test]
        public void MaskedMemCpy_CopyWithAlternatingMaskBits_WillYieldAlternatingSequence()
        {
            using (var data = DeterminismTestUtility.GenerateIdentity(DefaultDataSize))
            using (var mask = DeterminismTestUtility.GenerateMask(DefaultMaskSize, i => 0 == (i & 1)))
            using (var expected = DeterminismTestUtility.GenerateData(DefaultDataSize, i => ( 0 == (i & 1) ) ? (byte)i : default ))

            using (var result = MaskedMemCopy.CreateMaskedBuffer(data, mask, Allocator.Temp))
            {
                Assert.That(result, Is.EquivalentTo(expected), DeterminismTestUtility.PrintAll(result, expected));
            }
        }

        [Test]
        public void MaskedMemCpy_EmptyMask_WillThrow()
        {
            using (var emptyMask = new NativeArray<byte>(0, Allocator.Temp))
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    _ = MaskedMemCopy.CreateMaskedBuffer(emptyMask, emptyMask, Allocator.Temp);
                });
            }
        }

        [Test]
        public void MaskedMemCpy_InvalidMaskSize_WillThrow()
        {
            using (var emptyMask = new NativeArray<byte>(15, Allocator.Temp))
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    _ = MaskedMemCopy.CreateMaskedBuffer(emptyMask, emptyMask, Allocator.Temp);
                });
            }
        }

        [Test]
        public void MaskedMemCpy_MaskedMemCopyTestsData_HasExpectedMask()
        {
            var mask = Masks.GetTypeMask<MemMaskTestData_Mask>();
            using (var expectedMask = DeterminismTestUtility.GenerateMask(48, i => MaskedFields_MemMaskTestData.Contains(i % 12)))
            {
                Assert.That(mask, Is.EquivalentTo(expectedMask), DeterminismTestUtility.PrintAll(mask, expectedMask));
            }
        }

        [Test]
        public void MaskedMemCpy_EndToEnd_HasExpectedValueAfterMemMask()
        {
            // see: MaskedMemCpy_MaskedMemCopyTestsData_HasExpectedMask for the prerequisites:
            // --> we got a size of 12 bytes for the test data, 16 byte block size thus 48 byte of mask block data.
            MaskedMemCpy_MaskedMemCopyTestsData_HasExpectedMask(); // adding test here such that it can not get removed accidentally.

            // we generate some test data (24 bytes, which will initially set all padding to zero), and hash it.
            // then we mess up the padding bytes, we hash again which will give a different value. (assert#1)
            // we assert (AssertMemberwiseEqual) that we actually did not change any members (padding is transparent when accessing the members)
            // finally we apply the expected padding mask which should bring back everything to the original state

            const int seed = 0x72;
            var testData0 = new MemMaskTestData_Mask
            {
                Offset0 = true,
                Offset4 = 0x19abcdef,
                Offset8 = true
            };

            var testData1 = new MemMaskTestData_Mask
            {
                Offset0 = true,
                Offset4 = 0x01abcdef,
                Offset8 = true
            };

            var mask = Masks.GetMaskView<MemMaskTestData_Mask>();
            using (var _ = new NativeArray<MemMaskTestData_Mask>(2, Allocator.Persistent))
            {
                var data = _;
                data[0] = testData0;
                data[1] = testData1;

                var view = NativeViewUtility.GetReadView(data);
                var h0 = HashUtility.GetDenseHash(view, seed);

                MessUpPaddingBytes(view, mask);

                var h1 = HashUtility.GetDenseHash(view, seed);
                Assert.AreNotEqual(h0, h1);

                AssertMemberwiseEqual(testData0, data[0]);
                AssertMemberwiseEqual(testData1, data[1]);

                MaskedMemCopy.ApplyMask(view, mask);
                Assert.AreEqual(h0, HashUtility.GetDenseHash(view, seed));
            }
        }

        void MessUpPaddingBytes(NativeView view, NativeView mask)
        {
            var maskLength = mask.LengthInBytes;
            for (int i = 0; i < view.LengthInBytes; i++)
            {
                var maskIndex = i % maskLength;
                var isUnused = NativeViewUtility.ReadByte(mask, maskIndex) == 0;

                if (isUnused)
                {
                    NativeViewUtility.WriteByte(view, i, (byte)i);
                }
            }
        }

        void AssertMemberwiseEqual(MemMaskTestData_Mask expected, MemMaskTestData_Mask actual)
        {
            Assert.AreEqual(expected.Offset0, actual.Offset0);
            Assert.AreEqual(expected.Offset4, actual.Offset4);
            Assert.AreEqual(expected.Offset8, actual.Offset8);
        }
    }
}
