using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Entities.Determinism
{
    internal static class Fnv1a128
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Hash128 Hash(NativeArray<byte> data) => Hash(data, new ulong2 { x = seedX, y = seedY });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool RequiresRemainderHandling(NativeArray<byte> data) => (data.Length & 0xf) > 0;

        internal struct ulong2 : IEquatable<ulong2>
        {
            public ulong x;
            public ulong y;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(ulong2 other)
            {
                return x == other.x && y == other.y;
            }
        }

        const ulong mask32 = uint.MaxValue;
        const int shift32 = 32;

        const ulong primeX = 0x13b;
        const ulong primeY = 1 << 24;

        const ulong seedX = 0x62b821756295c58d;
        const ulong seedY = 0x6c62272e07bb0142;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong2 Multiply(ulong x, ulong y)
        {
            unchecked
            {
                var x0 = x & uint.MaxValue;
                var x1 = x >> 32;

                var y0 = y & uint.MaxValue;
                var y1 = y >> 32;

                var a0 = x0 * y0;
                var a1 = x0 * y1;
                var a2 = x1 * y0;
                var a3 = x1 * y1;

                var c = ((a0 >> 32) + (a1 & uint.MaxValue) + (a2 & uint.MaxValue)) >> 32;

                return new ulong2
                {
                    x = a0 + (a1 << 32) + (a2 << 32),
                    y = a3 + (a1 >> 32) + (a2 >> 32) + c
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong2 MultiplyMod(ulong2 a, ulong2 b)
        {
            unchecked
            {
                var c = Multiply(a.x, b.x);
                var d = a.x * b.y + a.y * b.x;

                return new ulong2
                {
                    x = c.x,
                    y = c.y + d
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong2 Xor(ulong2 a, ulong2 b) => new ulong2
        {
            x = a.x ^ b.x,
            y = a.y ^ b.y
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong2 Iterate(ulong2 block, ulong2 seed)
        {
            var xor = Xor(block, seed);
            return MultiplyMod(xor, new ulong2
            {
                x = primeX,
                y = primeY
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong2 Iterate(byte nextByte, ulong2 seed)
        {
            var xor = Xor(new ulong2 { x = nextByte, y = 0}, seed);

            return MultiplyMod(xor, new ulong2
            {
                x = primeX,
                y = primeY
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Hash128 ToHash128( ulong2 l )
        {
            var h = new Hash128();
            unsafe
            {
                *((ulong2*) UnsafeUtility.AddressOf(ref h)) = l;
            }
            return h;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static NativeArray<ulong2> GetULong2View(NativeArray<byte> data)
        {
            unsafe
            {
                var ptr = (ulong2*) data.GetUnsafeReadOnlyPtr();

                var blocks = data.Length >> 4;
                var r = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ulong2>(ptr, blocks, Allocator.None);

            #if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref r, NativeArrayUnsafeUtility.GetAtomicSafetyHandle(data));
            #endif

                return r;
            }
        }

        static Hash128 Hash(NativeArray<ulong2> blocks, ulong2 seed)
        {
            var length = blocks.Length;
            for (int i = 0; i < length; ++i)
            {
                seed = Iterate(blocks[i], seed);
            }

            return ToHash128(seed);
        }

        static ulong2 GetRemainderBlock(NativeArray<byte> data)
        {
            var length = data.Length;
            var remainderBytes = length & 0xf;

            var rBlock = new ulong2();
            unsafe
            {
                UnsafeUtility.MemCpy
                (
                    UnsafeUtility.AddressOf(ref rBlock),
                    (byte*) data.GetUnsafeReadOnlyPtr() + (length - remainderBytes),
                    remainderBytes
                );
            }

            return rBlock;
        }

        internal static Hash128 HashBlocks(NativeArray<byte> data)
        {
            var seed = new ulong2 { x = seedX, y = seedY };

            if (RequiresRemainderHandling(data))
            {
                seed = Iterate(GetRemainderBlock(data), seed);
            }

            return Hash(GetULong2View(data), seed);
        }

        static Hash128 Hash(NativeArray<byte> input, ulong2 seed)
        {
            var length = input.Length;
            for (int i = 0; i < length; ++i)
            {
                seed = Iterate(input[i], seed);
            }

            return ToHash128(seed);
        }
    }
}
