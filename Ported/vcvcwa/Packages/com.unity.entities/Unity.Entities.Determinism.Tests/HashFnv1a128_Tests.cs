using System;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities.Determinism.Tests
{
    [TestFixture]
    public class HashFnv1a128_Tests
    {
        [Test]
        public void HashContent_HashedIdentityArrayLength16_YieldsExpectedFnv1a128Hash()
        {
            using (var array = DeterminismTestUtility.GenerateIdentity(16))
            {
                var hash = Fnv1a128.Hash(array);

                Assert.AreEqual(0x3cb989fd, hash.Value.x);
                Assert.AreEqual(0x8177f0c9, hash.Value.y);
                Assert.AreEqual(0x93cff23f, hash.Value.z);
                Assert.AreEqual(0x2f57eaee, hash.Value.w);
            }
        }

        [Test]
        public void HashContent_HashedArrayLength16ZeroMemory_YieldsExpectedFnv1a128Hash()
        {
            using (var array = new NativeArray<byte>(16, Allocator.TempJob))
            {
                var hash = Fnv1a128.Hash(array);

                Assert.AreEqual(0x34145e4d, hash.Value.x);
                Assert.AreEqual(0x15171638, hash.Value.y);
                Assert.AreEqual(0xf705b5ef, hash.Value.z);
                Assert.AreEqual(0xf1f90b7b, hash.Value.w);
            }
        }

        [Test]
        public void HashContent_HashedArrayLength1ZeroMemory_YieldsExpectedFnv1a128Hash()
        {
            using (var array = new NativeArray<byte>(1, Allocator.TempJob))
            {
                var hash = Fnv1a128.Hash(array);

                Assert.AreEqual(0x4e4a147f, hash.Value.x);
                Assert.AreEqual(0x78912b70, hash.Value.y);
                Assert.AreEqual(0x101a8caf, hash.Value.z);
                Assert.AreEqual(0xd228cb69, hash.Value.w);
            }
        }

        [Test]
        public void HashContent_HashEmptyArray_YieldsExpectedFnv1a128Hash()
        {
            using (var array = new NativeArray<byte>(0, Allocator.TempJob))
            {
                var hash = Fnv1a128.Hash(array);

                Assert.AreEqual(0x6295c58d, hash.Value.x);
                Assert.AreEqual(0x62b82175, hash.Value.y);
                Assert.AreEqual(0x07bb0142, hash.Value.z);
                Assert.AreEqual(0x6c62272e, hash.Value.w);
            }
        }

        [Test]
        public void HashContent_BurstedAndNonburstedVersion_YieldSameResult()
        {
            using (var data = new NativeArray<byte>(16, Allocator.TempJob))
            {
                var nonBursted = TestHelper.HashContentNonbursted(data);
                var bursted = TestHelper.HashContentBursted(data);

                Assert.AreEqual(nonBursted,bursted);
            }
        }

        [Test]
        public void HashImpl_Multiply_SquaringLargest64BitValue_YieldsExpected128BitResult()
        {
            var result = Fnv1a128.Multiply(UInt64.MaxValue, UInt64.MaxValue);

            Assert.AreEqual(0x0000000000000001, result.x );
            Assert.AreEqual(0xfffffffffffffffe, result.y );
        }

        [Test]
        public void HashImpl_Multiply_Multiplying64BitValue_YieldsExpected128BitResult()
        {
            ulong a = 0xaaaaaaaabcdefeff;
            ulong b = 0xeeeeeeeef01234ff;

            var result = Fnv1a128.Multiply(a, b);

            Assert.AreEqual(0xc731283e6bd9cc01, result.x );
            Assert.AreEqual(0x9f49f49f5bb47209, result.y );
        }

        [Test]
        public void HashImpl_MultiplyModMax_SquaringLargest128BitValue_YieldsExcepted128BitModulusResultResult()
        {
            var uint128Max = new Fnv1a128.ulong2
            {
                x = UInt64.MaxValue,
                y = UInt64.MaxValue
            };

            var result = Fnv1a128.MultiplyMod(uint128Max, uint128Max);

            Assert.AreEqual(1, result.x );
            Assert.AreEqual(0, result.y );
        }

        [Test]
        public void Prerequisite_DotsHash128AndUnityHash128_HaveSameSize()
        {
            var sizeOfDotsHash128 = UnsafeUtility.SizeOf<Unity.Entities.Hash128>();
            var sizeOfUnityHash128 = UnsafeUtility.SizeOf<UnityEngine.Hash128>();

            Assert.AreEqual(sizeOfDotsHash128, sizeOfUnityHash128);
        }

        public static class TestHelper
        {
            public static Hash128 HashContentNonbursted(NativeArray<byte> data) => Fnv1a128.Hash(data);

            public static Hash128 HashContentBursted(NativeArray<byte> data)
            {
                using (var result = new NativeArray<Hash128>(1, Allocator.TempJob))
                {
                    new HashContentJob
                        {
                            Data = data,
                            Result = result
                        }
                        .Schedule()
                        .Complete();

                    return result[0];
                }
            }

            [BurstCompile(CompileSynchronously = true)]
            struct HashContentJob : IJob
            {
                [ReadOnly] public NativeArray<byte> Data;
                [WriteOnly] public NativeArray<Hash128> Result;

                public void Execute()
                {
                #if ENABLE_UNITY_COLLECTIONS_CHECKS
                    if (Result.Length != 1)
                    {
                        throw new ArgumentException("Result does not have the expected Length of 1");
                    }
                #endif
                    Result[0] = Fnv1a128.Hash(Data);
                }
            }
        }
    }
}
