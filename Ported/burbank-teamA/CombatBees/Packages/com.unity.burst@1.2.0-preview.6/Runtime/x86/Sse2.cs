using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#if BURST_INTERNAL || UNITY_BURST_EXPERIMENTAL_X86_INTRINSICS

namespace Unity.Burst.Intrinsics
{
    public unsafe static partial class X86
    {
        public static class Sse2
        {
            /// <summary>
            /// Store 32-bit integer "a" into memory using a non-temporal hint to minimize cache pollution. If the cache line containing address "mem_addr" is already in the cache, the cache will be updated.
            /// </summary>
            [DebuggerStepThrough]
            public static void stream_si32(int* mem_addr, int a)
            {
                *mem_addr = a;
            }

            /// <summary>
            /// Store 64-bit integer a into memory using a non-temporal hint to minimize cache pollution. If the cache line containing address mem_addr is already in the cache, the cache will be updated.
            /// </summary>
            [DebuggerStepThrough]
            public static void stream_si64(long* mem_addr, long a)
            {
                *mem_addr = a;
            }

            /// <summary>
            /// Store 128-bits (composed of 2 packed double-precision (64-bit) floating-point elements) from "a" into memory using a non-temporal memory hint. "mem_addr" must be aligned on a 16-byte boundary or a general-protection exception will be generated.
            /// </summary>
            [DebuggerStepThrough]
            public static void stream_pd(void* mem_addr, m128 a)
            {
                CheckPointerAlignment16(mem_addr);
                GenericCSharpStore(mem_addr, a);
            }

            /// <summary>
            /// Store 128-bits of integer data from a into memory using a non-temporal memory hint.mem_addr must be aligned on a 16-byte boundary or a general-protection exception may be generated.
            /// </summary>
            [DebuggerStepThrough]
            public static void stream_si128(void* mem_addr, m128 a)
            {
                CheckPointerAlignment16(mem_addr);
                GenericCSharpStore(mem_addr, a);
            }

            // _mm_add_epi8
            /// <summary> Add packed 8-bit integers in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 add_epi8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                sbyte* dptr = &dst.SByte0;
                sbyte* aptr = &a.SByte0;
                sbyte* bptr = &b.SByte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = (sbyte)(aptr[j] + bptr[j]);
                }
                return dst;
            }

            // _mm_add_epi16
            /// <summary> Add packed 16-bit integers in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 add_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = (short)(aptr[j] + bptr[j]);
                }
                return dst;
            }

            // _mm_add_epi32
            /// <summary> Add packed 32-bit integers in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 add_epi32(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.SInt0 = a.SInt0 + b.SInt0;
                dst.SInt1 = a.SInt1 + b.SInt1;
                dst.SInt2 = a.SInt2 + b.SInt2;
                dst.SInt3 = a.SInt3 + b.SInt3;
                return dst;
            }

            // _mm_add_epi64
            /// <summary> Add packed 64-bit integers in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 add_epi64(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.SLong0 = a.SLong0 + b.SLong0;
                dst.SLong1 = a.SLong1 + b.SLong1;
                return dst;
            }

            // _mm_adds_epi8
            /// <summary> Add packed 8-bit integers in "a" and "b" using saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 adds_epi8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                sbyte* dptr = &dst.SByte0;
                sbyte* aptr = &a.SByte0;
                sbyte* bptr = &b.SByte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = Saturate_To_Int8(aptr[j] + bptr[j]);
                }
                return dst;
            }

            // _mm_adds_epi16
            /// <summary> Add packed 16-bit integers in "a" and "b" using saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 adds_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = Saturate_To_Int16(aptr[j] + bptr[j]);
                }
                return dst;
            }

            // _mm_adds_epu8
            /// <summary> Add packed unsigned 8-bit integers in "a" and "b" using saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 adds_epu8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = Saturate_To_UnsignedInt8(aptr[j] + bptr[j]);
                }
                return dst;
            }

            // _mm_adds_epu16
            /// <summary> Add packed unsigned 16-bit integers in "a" and "b" using saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 adds_epu16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                ushort* dptr = &dst.UShort0;
                ushort* aptr = &a.UShort0;
                ushort* bptr = &b.UShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = Saturate_To_UnsignedInt16(aptr[j] + bptr[j]);
                }
                return dst;
            }

            // _mm_avg_epu8
            /// <summary> Average packed unsigned 8-bit integers in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            // This was not added until LLVM => 9.0
            public static m128 avg_epu8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = (byte)((aptr[j] + bptr[j] + 1) >> 1);
                }
                return dst;
            }

            // _mm_avg_epu16
            /// <summary> Average packed unsigned 16-bit integers in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 avg_epu16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                ushort* dptr = &dst.UShort0;
                ushort* aptr = &a.UShort0;
                ushort* bptr = &b.UShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = (ushort)((aptr[j] + bptr[j] + 1) >> 1);
                }
                return dst;
            }

            // _mm_madd_epi16
            /// <summary> Multiply packed signed 16-bit integers in "a" and "b", producing intermediate signed 32-bit integers. Horizontally add adjacent pairs of intermediate 32-bit integers, and pack the results in "dst".</summary>
            [DebuggerStepThrough]
            public static m128 madd_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                int* dptr = &dst.SInt0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 3; j++)
                {
                    int k = 2 * j;

                    int r = aptr[k + 1] * bptr[k + 1];
                    int q = aptr[k] * bptr[k];

                    dptr[j] = r + q;
                }
                return dst;
            }

            // _mm_max_epi16
            /// <summary> Compare packed 16-bit integers in "a" and "b", and store packed maximum values in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 max_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = Math.Max(aptr[j], bptr[j]);
                }
                return dst;
            }

            // _mm_max_epu8
            /// <summary> Compare packed unsigned 8-bit integers in "a" and "b", and store packed maximum values in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 max_epu8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = Math.Max(aptr[j], bptr[j]);
                }
                return dst;
            }

            // _mm_min_epi16
            /// <summary> Compare packed 16-bit integers in "a" and "b", and store packed minimum values in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 min_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = Math.Min(aptr[j], bptr[j]);
                }
                return dst;
            }

            // _mm_min_epu8
            /// <summary> Compare packed unsigned 8-bit integers in "a" and "b", and store packed minimum values in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 min_epu8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = Math.Min(aptr[j], bptr[j]);
                }
                return dst;
            }

            // _mm_mulhi_epi16
            /// <summary> Multiply the packed 16-bit integers in "a" and "b", producing intermediate 32-bit integers, and store the high 16 bits of the intermediate integers in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 mulhi_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    int tmp = aptr[j] * bptr[j];
                    dptr[j] = (short)(tmp >> 16);
                }
                return dst;
            }

            // _mm_mulhi_epu16
            /// <summary> Multiply the packed unsigned 16-bit integers in "a" and "b", producing intermediate 32-bit integers, and store the high 16 bits of the intermediate integers in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 mulhi_epu16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                ushort* dptr = &dst.UShort0;
                ushort* aptr = &a.UShort0;
                ushort* bptr = &b.UShort0;
                for (int j = 0; j <= 7; j++)
                {
                    uint tmp = (uint)(aptr[j] * bptr[j]);
                    dptr[j] = (ushort)(tmp >> 16);
                }
                return dst;
            }

            // _mm_mullo_epi16
            /// <summary> Multiply the packed 16-bit integers in "a" and "b", producing intermediate 32-bit integers, and store the low 16 bits of the intermediate integers in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 mullo_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    int tmp = aptr[j] * bptr[j];
                    dptr[j] = (short)tmp;
                }
                return dst;
            }

            // _mm_mul_epu32
            /// <summary> Multiply the low unsigned 32-bit integers from each packed 64-bit element in "a" and "b", and store the unsigned 64-bit results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 mul_epu32(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (ulong)a.UInt0 * (ulong)b.UInt0;
                dst.ULong1 = (ulong)a.UInt2 * (ulong)b.UInt2;
                return dst;
            }

            // _mm_sad_epu8
            /// <summary> Compute the absolute differences of packed unsigned 8-bit integers in "a" and "b", then horizontally sum each consecutive 8 differences to produce two unsigned 16-bit integers, and pack these unsigned 16-bit integers in the low 16 bits of 64-bit elements in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sad_epu8(m128 a, m128 b)
            {
                m128 tmp;
                byte* tptr = &tmp.Byte0;
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                for (int j = 0; j <= 15; j++)
                {
                    tptr[j] = (byte)Math.Abs(aptr[j] - bptr[j]);
                }

                m128 dst = default(m128);
                ushort* dptr = &dst.UShort0;
                for (int j = 0; j <= 1; j++)
                {
                    int bo = j * 8;
                    dptr[4 * j] = (ushort)
                        (tptr[bo + 0] + tptr[bo + 1] + tptr[bo + 2] + tptr[bo + 3] +
                         tptr[bo + 4] + tptr[bo + 5] + tptr[bo + 6] + tptr[bo + 7]);
                }
                return dst;
            }

            // _mm_sub_epi8
            /// <summary> Subtract packed 8-bit integers in "b" from packed 8-bit integers in "a", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sub_epi8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                sbyte* dptr = &dst.SByte0;
                sbyte* aptr = &a.SByte0;
                sbyte* bptr = &b.SByte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = (sbyte)(aptr[j] - bptr[j]);
                }
                return dst;
            }

            // _mm_sub_epi16
            /// <summary> Subtract packed 16-bit integers in "b" from packed 16-bit integers in "a", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sub_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = (short)(aptr[j] - bptr[j]);
                }
                return dst;
            }

            // _mm_sub_epi32
            /// <summary> Subtract packed 32-bit integers in "b" from packed 32-bit integers in "a", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sub_epi32(m128 a, m128 b)
            {
                m128 dst = default(m128);
                int* dptr = &dst.SInt0;
                int* aptr = &a.SInt0;
                int* bptr = &b.SInt0;
                for (int j = 0; j <= 3; j++)
                {
                    dptr[j] = aptr[j] - bptr[j];
                }
                return dst;
            }


            // _mm_sub_epi64
            /// <summary> Subtract packed 64-bit integers in "b" from packed 64-bit integers in "a", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sub_epi64(m128 a, m128 b)
            {
                m128 dst = default(m128);
                long* dptr = &dst.SLong0;
                long* aptr = &a.SLong0;
                long* bptr = &b.SLong0;
                for (int j = 0; j <= 1; j++)
                {
                    dptr[j] = aptr[j] - bptr[j];
                }
                return dst;
            }

            // _mm_subs_epi8
            /// <summary> Subtract packed 8-bit integers in "b" from packed 8-bit integers in "a" using saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 subs_epi8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                sbyte* dptr = &dst.SByte0;
                sbyte* aptr = &a.SByte0;
                sbyte* bptr = &b.SByte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = Saturate_To_Int8(aptr[j] - bptr[j]);
                }
                return dst;
            }

            // _mm_subs_epi16
            /// <summary> Subtract packed 16-bit integers in "b" from packed 16-bit integers in "a" using saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 subs_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = Saturate_To_Int16(aptr[j] - bptr[j]);
                }
                return dst;
            }

            // _mm_subs_epu8
            /// <summary> Subtract packed unsigned 8-bit integers in "b" from packed unsigned 8-bit integers in "a" using saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 subs_epu8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = Saturate_To_UnsignedInt8(aptr[j] - bptr[j]);
                }
                return dst;
            }

            // _mm_subs_epu16
            /// <summary> Subtract packed unsigned 16-bit integers in "b" from packed unsigned 16-bit integers in "a" using saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 subs_epu16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                ushort* dptr = &dst.UShort0;
                ushort* aptr = &a.UShort0;
                ushort* bptr = &b.UShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = Saturate_To_UnsignedInt16(aptr[j] - bptr[j]);
                }
                return dst;
            }

            // _mm_slli_si128
            /// <summary> Shift "a" left by "imm8" bytes while shifting in zeros, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 slli_si128(m128 a, int imm8)
            {
                int dist = Math.Min(imm8 & 0xff, 16);
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                for (int j = 0; j < dist; ++j)
                {
                    dptr[j] = 0;
                }
                for (int j = dist; j < 16; ++j)
                {
                    dptr[j] = aptr[j - dist];
                }
                return dst;
            }

            // _mm_bslli_si128
            /// <summary> Shift "a" left by "imm8" bytes while shifting in zeros, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 bslli_si128(m128 a, int imm8)
            {
                return slli_si128(a, imm8);
            }

            // _mm_bsrli_si128
            /// <summary> Shift "a" right by "imm8" bytes while shifting in zeros, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 bsrli_si128(m128 a, int imm8)
            {
                int dist = Math.Min(imm8 & 0xff, 16);
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                for (int j = 0; j < 16 - dist; ++j)
                {
                    dptr[j] = aptr[dist + j];
                }
                for (int j = 16 - dist; j < 16; ++j)
                {
                    dptr[j] = 0;
                }
                return dst;
            }

            // _mm_slli_epi16
            /// <summary> Shift packed 16-bit integers in "a" left by "imm8" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 slli_epi16(m128 a, int imm8)
            {
                m128 dst = default(m128);
                int dist = imm8 & 0xff;
                ushort* dptr = &dst.UShort0;
                ushort* aptr = &a.UShort0;
                for (int j = 0; j <= 7; j++)
                {
                    if (dist > 15)
                        dptr[j] = 0;
                    else
                        dptr[j] = (ushort)(aptr[j] << dist);
                }
                return dst;
            }

            // _mm_sll_epi16
            /// <summary> Shift packed 16-bit integers in "a" left by "count" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 sll_epi16(m128 a, m128 count)
            {
                m128 dst = default(m128);
                int dist = (int)Math.Min(count.ULong0, 16);
                ushort* dptr = &dst.UShort0;
                ushort* aptr = &a.UShort0;
                for (int j = 0; j <= 7; j++)
                {
                    if (dist > 15)
                        dptr[j] = 0;
                    else
                        dptr[j] = (ushort)(aptr[j] << dist);
                }
                return dst;
            }

            // _mm_slli_epi32
            /// <summary> Shift packed 32-bit integers in "a" left by "imm8" while shifting in zeros, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 slli_epi32(m128 a, int imm8)
            {
                m128 dst = default(m128);
                int dist = Math.Min(imm8 & 0xff, 32);
                uint* dptr = &dst.UInt0;
                uint* aptr = &a.UInt0;
                for (int j = 0; j <= 3; j++)
                {
                    if (dist > 31)
                        dptr[j] = 0;
                    else
                        dptr[j] = aptr[j] << dist;
                }
                return dst;
            }

            // _mm_sll_epi32
            /// <summary> Shift packed 32-bit integers in "a" left by "count" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 sll_epi32(m128 a, m128 count)
            {
                m128 dst = default(m128);
                int dist = (int)Math.Min(count.ULong0, 32);
                uint* dptr = &dst.UInt0;
                uint* aptr = &a.UInt0;
                for (int j = 0; j <= 3; j++)
                {
                    if (dist > 31)
                        dptr[j] = 0;
                    else
                        dptr[j] = aptr[j] << dist;
                }
                return dst;
            }

            // _mm_slli_epi64
            /// <summary> Shift packed 64-bit integers in "a" left by "imm8" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 slli_epi64(m128 a, int imm8)
            {
                m128 dst = default(m128);
                int dist = Math.Min(imm8 & 0xff, 64);
                ulong* dptr = &dst.ULong0;
                ulong* aptr = &a.ULong0;
                for (int j = 0; j <= 1; j++)
                {
                    if (dist > 63)
                        dptr[j] = 0;
                    else
                        dptr[j] = aptr[j] << dist;
                }
                return dst;
            }

            // _mm_sll_epi64
            /// <summary> Shift packed 64-bit integers in "a" left by "count" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 sll_epi64(m128 a, m128 count)
            {
                m128 dst = default(m128);
                int dist = (int)Math.Min(count.ULong0, 64);
                ulong* dptr = &dst.ULong0;
                ulong* aptr = &a.ULong0;
                for (int j = 0; j <= 1; j++)
                {
                    if (dist > 63)
                        dptr[j] = 0;
                    else
                        dptr[j] = aptr[j] << dist;
                }
                return dst;
            }

            // _mm_srai_epi16
            /// <summary> Shift packed 16-bit integers in "a" right by "imm8" while shifting in sign bits, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 srai_epi16(m128 a, int imm8)
            {
                int dist = Math.Min(imm8 & 0xff, 16);

                m128 dst = a;

                short* dptr = &dst.SShort0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 7; j++)
                    {
                        // Work around modulo-16 shift distances for the 16 case (replicates sign bit)
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_sra_epi16
            /// <summary> Shift packed 16-bit integers in "a" right by "count" while shifting in sign bits, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 sra_epi16(m128 a, m128 count)
            {
                int dist = (int)Math.Min(count.ULong0, 16);

                m128 dst = a;
                short* dptr = &dst.SShort0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 7; j++)
                    {
                        // Work around modulo-16 shift distances for the 16 case (replicates sign bit)
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_srai_epi32
            /// <summary> Shift packed 32-bit integers in "a" right by "imm8" while shifting in sign bits, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 srai_epi32(m128 a, int imm8)
            {
                int dist = Math.Min(imm8 & 0xff, 32);

                m128 dst = a;
                int* dptr = &dst.SInt0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 3; j++)
                    {
                        // Work around modulo-32 shift distances for the 32 case (replicates sign bit)
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_sra_epi32
            /// <summary> Shift packed 32-bit integers in "a" right by "count" while shifting in sign bits, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 sra_epi32(m128 a, m128 count)
            {
                int dist = (int)Math.Min(count.ULong0, 32);

                m128 dst = a;
                int* dptr = &dst.SInt0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 3; j++)
                    {
                        // Work around modulo-32 shift distances for the 32 case (replicates sign bit)
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_srli_si128
            /// <summary> Shift "a" right by "imm8" bytes while shifting in zeros, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 srli_si128(m128 a, int imm8)
            {
                return bsrli_si128(a, imm8);
            }

            // _mm_srli_epi16
            /// <summary> Shift packed 16-bit integers in "a" right by "imm8" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 srli_epi16(m128 a, int imm8)
            {
                int dist = Math.Min(imm8 & 0xff, 16);

                m128 dst = a;
                ushort* dptr = &dst.UShort0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 7; j++)
                    {
                        // Work around modulo-16 shift distances for the 16 case
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_srl_epi16
            /// <summary> Shift packed 16-bit integers in "a" right by "count" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 srl_epi16(m128 a, m128 count)
            {
                int dist = (int)Math.Min(count.ULong0, 16);

                m128 dst = a;
                ushort* dptr = &dst.UShort0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 7; j++)
                    {
                        // Work around modulo-16 shift distances for the 16 case
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_srli_epi32
            /// <summary> Shift packed 32-bit integers in "a" right by "imm8" while shifting in zeros, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 srli_epi32(m128 a, int imm8)
            {
                int dist = Math.Min(imm8 & 0xff, 32);

                m128 dst = a;
                uint* dptr = &dst.UInt0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 3; j++)
                    {
                        // Work around modulo-32 shift distances for the 32 case
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_srl_epi32
            /// <summary> Shift packed 32-bit integers in "a" right by "count" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 srl_epi32(m128 a, m128 count)
            {
                int dist = (int)Math.Min(count.ULong0, 32);

                m128 dst = a;
                uint* dptr = &dst.UInt0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 3; j++)
                    {
                        // Work around modulo-32 shift distances for the 32 case
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_srli_epi64
            /// <summary> Shift packed 64-bit integers in "a" right by "imm8" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 srli_epi64(m128 a, int imm8)
            {
                int dist = Math.Min(imm8 & 0xff, 64);

                m128 dst = a;
                ulong* dptr = &dst.ULong0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 1; j++)
                    {
                        // Work around modulo-64 shift distances for the 64 case
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_srl_epi64
            /// <summary> Shift packed 64-bit integers in "a" right by "count" while shifting in zeros, and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 srl_epi64(m128 a, m128 count)
            {
                int dist = (int)Math.Min(count.ULong0, 64);

                m128 dst = a;
                ulong* dptr = &dst.ULong0;

                if (dist > 0)
                {
                    dist--;
                    for (int j = 0; j <= 1; j++)
                    {
                        // Work around modulo-32 shift distances for the 32 case
                        dptr[j] >>= 1;
                        dptr[j] >>= dist;
                    }
                }
                return dst;
            }

            // _mm_and_si128
            /// <summary> Compute the bitwise AND of 128 bits (representing integer data) in "a" and "b", and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 and_si128(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong0 & b.ULong0;
                dst.ULong1 = a.ULong1 & b.ULong1;
                return dst;
            }

            // _mm_andnot_si128
            /// <summary> Compute the bitwise NOT of 128 bits (representing integer data) in "a" and then AND with "b", and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 andnot_si128(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (~a.ULong0) & b.ULong0;
                dst.ULong1 = (~a.ULong1) & b.ULong1;
                return dst;
            }

            // _mm_or_si128
            /// <summary> Compute the bitwise OR of 128 bits (representing integer data) in "a" and "b", and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 or_si128(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong0 | b.ULong0;
                dst.ULong1 = a.ULong1 | b.ULong1;
                return dst;
            }

            // _mm_xor_si128
            /// <summary> Compute the bitwise XOR of 128 bits (representing integer data) in "a" and "b", and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 xor_si128(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong0 ^ b.ULong0;
                dst.ULong1 = a.ULong1 ^ b.ULong1;
                return dst;
            }

            // _mm_cmpeq_epi8
            /// <summary> Compare packed 8-bit integers in "a" and "b" for equality, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpeq_epi8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                byte* dptr = &dst.Byte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = (byte)(aptr[j] == bptr[j] ? 0xff : 0x00);
                }
                return dst;
            }

            // _mm_cmpeq_epi16
            /// <summary> Compare packed 16-bit integers in "a" and "b" for equality, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpeq_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                ushort* aptr = &a.UShort0;
                ushort* bptr = &b.UShort0;
                ushort* dptr = &dst.UShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = (ushort)(aptr[j] == bptr[j] ? 0xffff : 0x0000);
                }
                return dst;
            }

            // _mm_cmpeq_epi32
            /// <summary> Compare packed 32-bit integers in "a" and "b" for equality, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpeq_epi32(m128 a, m128 b)
            {
                m128 dst = default(m128);
                uint* aptr = &a.UInt0;
                uint* bptr = &b.UInt0;
                uint* dptr = &dst.UInt0;
                for (int j = 0; j <= 3; j++)
                {
                    dptr[j] = aptr[j] == bptr[j] ? 0xffffffff : 0x00000000;
                }
                return dst;
            }

            // _mm_cmpgt_epi8
            /// <summary> Compare packed 8-bit integers in "a" and "b" for greater-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpgt_epi8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                sbyte* aptr = &a.SByte0;
                sbyte* bptr = &b.SByte0;
                sbyte* dptr = &dst.SByte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = (sbyte)(aptr[j] > bptr[j] ? -1 : 0);
                }
                return dst;
            }

            // _mm_cmpgt_epi16
            /// <summary> Compare packed 16-bit integers in "a" and "b" for greater-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpgt_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                short* dptr = &dst.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = (short)(aptr[j] > bptr[j] ? -1 : 0);
                }
                return dst;
            }

            // _mm_cmpgt_epi32
            /// <summary> Compare packed 32-bit integers in "a" and "b" for greater-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpgt_epi32(m128 a, m128 b)
            {
                m128 dst = default(m128);
                int* aptr = &a.SInt0;
                int* bptr = &b.SInt0;
                int* dptr = &dst.SInt0;
                for (int j = 0; j <= 3; j++)
                {
                    dptr[j] = aptr[j] > bptr[j] ? -1 : 0;
                }
                return dst;
            }

            // _mm_cmplt_epi8
            /// <summary> Compare packed 8-bit integers in "a" and "b" for less-than, and store the results in "dst". Note: This intrinsic emits the pcmpgtb instruction with the order of the operands switched. </summary>
            [DebuggerStepThrough]
            public static m128 cmplt_epi8(m128 a, m128 b)
            {
                return cmpgt_epi8(b, a);
            }

            // _mm_cmplt_epi16
            /// <summary> Compare packed 16-bit integers in "a" and "b" for less-than, and store the results in "dst". Note: This intrinsic emits the pcmpgtw instruction with the order of the operands switched. </summary>
            [DebuggerStepThrough]
            public static m128 cmplt_epi16(m128 a, m128 b)
            {
                return cmpgt_epi16(b, a);
            }

            // _mm_cmplt_epi32
            /// <summary> Compare packed 32-bit integers in "a" and "b" for less-than, and store the results in "dst". Note: This intrinsic emits the pcmpgtd instruction with the order of the operands switched. </summary>
            [DebuggerStepThrough]
            public static m128 cmplt_epi32(m128 a, m128 b)
            {
                return cmpgt_epi32(b, a);
            }

            // _mm_cvtepi32_pd
            /// <summary> Convert packed 32-bit integers in "a" to packed double-precision (64-bit) floating-point elements, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtepi32_pd(m128 a)
            {
                m128 dst = default(m128);
                dst.Double0 = a.SInt0;
                dst.Double1 = a.SInt1;
                return dst;
            }

            // _mm_cvtsi32_sd
            /// <summary> Convert the 32-bit integer "b" to a double-precision (64-bit) floating-point element, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 cvtsi32_sd(m128 a, int b)
            {
                m128 dst = a;
                dst.Double0 = b;
                return dst;
            }

            // _mm_cvtsi64_sd
            /// <summary> Convert the 64-bit integer "b" to a double-precision (64-bit) floating-point element, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 cvtsi64_sd(m128 a, long b)
            {
                m128 dst = a;
                dst.Double0 = b;
                return dst;
            }

            // _mm_cvtsi64x_sd
            /// <summary> Convert the 64-bit integer "b" to a double-precision (64-bit) floating-point element, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 cvtsi64x_sd(m128 a, long b)
            {
                return cvtsi64_sd(a, b);
            }

            // _mm_cvtepi32_ps
            /// <summary> Convert packed 32-bit integers in "a" to packed single-precision (32-bit) floating-point elements, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtepi32_ps(m128 a)
            {
                m128 dst = default(m128);
                dst.Float0 = a.SInt0;
                dst.Float1 = a.SInt1;
                dst.Float2 = a.SInt2;
                dst.Float3 = a.SInt3;
                return dst;
            }


            // _mm_cvtsi32_si128
            /// <summary> Copy 32-bit integer "a" to the lower elements of "dst", and zero the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtsi32_si128(int a)
            {
                // This doesn't need an intrinsic implementation, the Burst IR is fine.
                m128 dst = default(m128);
                dst.SInt0 = a;
                return dst;
            }

            // _mm_cvtsi64_si128
            /// <summary> Copy 64-bit integer "a" to the lower element of "dst", and zero the upper element. </summary>
            [DebuggerStepThrough]
            public static m128 cvtsi64_si128(long a)
            {
                // This doesn't need an intrinsic implementation, the Burst IR is fine.
                m128 dst = default(m128);
                dst.SLong0 = a;
                return dst;
            }

            // _mm_cvtsi64x_si128
            /// <summary> Copy 64-bit integer "a" to the lower element of "dst", and zero the upper element. </summary>
            [DebuggerStepThrough]
            public static m128 cvtsi64x_si128(long a)
            {
                return cvtsi64x_si128(a);
            }

            // _mm_cvtsi128_si32
            /// <summary> Copy the lower 32-bit integer in "a" to "dst". </summary>
            [DebuggerStepThrough]
            public static int cvtsi128_si32(m128 a)
            {
                // This doesn't need an intrinsic implementation, the Burst IR is fine.
                return a.SInt0;
            }

            // _mm_cvtsi128_si64
            /// <summary> Copy the lower 64-bit integer in "a" to "dst". </summary>
            [DebuggerStepThrough]
            public static long cvtsi128_si64(m128 a)
            {
                // This doesn't need an intrinsic implementation, the Burst IR is fine.
                return a.SLong0;
            }

            // _mm_cvtsi128_si64x
            /// <summary> Copy the lower 64-bit integer in "a" to "dst". </summary>
            [DebuggerStepThrough]
            public static long cvtsi128_si64x(m128 a)
            {
                // This doesn't need an intrinsic implementation, the Burst IR is fine.
                return a.SLong0;
            }

            // _mm_set_epi64x
            /// <summary> Set packed 64-bit integers in "dst" with the supplied values. </summary>
            [DebuggerStepThrough]
            public static m128 set_epi64x(long e1, long e0)
            {
                // This doesn't need an intrinsic implementation, the Burst IR is fine.
                m128 dst = default(m128);
                dst.SLong0 = e0;
                dst.SLong1 = e1;
                return dst;
            }

            // _mm_set_epi32
            /// <summary> Set packed 32-bit integers in "dst" with the supplied values. </summary>
            [DebuggerStepThrough]
            public static m128 set_epi32(int e3, int e2, int e1, int e0)
            {
                m128 dst = default(m128);
                dst.SInt0 = e0;
                dst.SInt1 = e1;
                dst.SInt2 = e2;
                dst.SInt3 = e3;
                return dst;
            }

            // _mm_set_epi16
            /// <summary> Set packed 16-bit integers in "dst" with the supplied values. </summary>
            [DebuggerStepThrough]
            public static m128 set_epi16(short e7, short e6, short e5, short e4, short e3, short e2, short e1, short e0)
            {
                m128 dst = default(m128);
                dst.SShort0 = e0;
                dst.SShort1 = e1;
                dst.SShort2 = e2;
                dst.SShort3 = e3;
                dst.SShort4 = e4;
                dst.SShort5 = e5;
                dst.SShort6 = e6;
                dst.SShort7 = e7;
                return dst;
            }

            // _mm_set_epi8
            /// <summary> Set packed 8-bit integers in "dst" with the supplied values in reverse order. </summary>
            [DebuggerStepThrough]
            public static m128 set_epi8(sbyte e15, sbyte e14, sbyte e13, sbyte e12, sbyte e11, sbyte e10, sbyte e9, sbyte e8, sbyte e7, sbyte e6, sbyte e5, sbyte e4, sbyte e3, sbyte e2, sbyte e1, sbyte e0)
            {
                m128 dst = default(m128);
                dst.SByte0 = e0;
                dst.SByte1 = e1;
                dst.SByte2 = e2;
                dst.SByte3 = e3;
                dst.SByte4 = e4;
                dst.SByte5 = e5;
                dst.SByte6 = e6;
                dst.SByte7 = e7;
                dst.SByte8 = e8;
                dst.SByte9 = e9;
                dst.SByte10 = e10;
                dst.SByte11 = e11;
                dst.SByte12 = e12;
                dst.SByte13 = e13;
                dst.SByte14 = e14;
                dst.SByte15 = e15;
                return dst;
            }


            // _mm_set1_epi64x
            /// <summary> Broadcast 64-bit integer "a" to all elements of "dst". This intrinsic may generate the "vpbroadcastq". </summary>
            [DebuggerStepThrough]
            public static m128 set1_epi64x(long a)
            {
                m128 dst = default(m128);
                dst.SLong0 = a;
                dst.SLong1 = a;
                return dst;
            }

            // _mm_set1_epi32
            /// <summary> Broadcast 32-bit integer "a" to all elements of "dst". This intrinsic may generate "vpbroadcastd". </summary>
            [DebuggerStepThrough]
            public static m128 set1_epi32(int a)
            {
                m128 dst = default(m128);
                dst.SInt0 = a;
                dst.SInt1 = a;
                dst.SInt2 = a;
                dst.SInt3 = a;
                return dst;
            }

            // _mm_set1_epi16
            /// <summary> Broadcast 16-bit integer "a" to all all elements of "dst". This intrinsic may generate "vpbroadcastw". </summary>
            [DebuggerStepThrough]
            public static m128 set1_epi16(short a)
            {
                m128 dst = default(m128);
                short* dptr = &dst.SShort0;
                for (int j = 0; j <= 7; j++)
                {
                    dptr[j] = a;
                }
                return dst;
            }

            // _mm_set1_epi8
            /// <summary> Broadcast 8-bit integer "a" to all elements of "dst". This intrinsic may generate "vpbroadcastb". </summary>
            [DebuggerStepThrough]
            public static m128 set1_epi8(sbyte a)
            {
                m128 dst = default(m128);
                sbyte* dptr = &dst.SByte0;
                for (int j = 0; j <= 15; j++)
                {
                    dptr[j] = a;
                }
                return dst;
            }


            // _mm_setr_epi32
            /// <summary> Set packed 32-bit integers in "dst" with the supplied values in reverse order. </summary>
            [DebuggerStepThrough]
            public static m128 setr_epi32(int e3, int e2, int e1, int e0)
            {
                m128 dst = default(m128);
                dst.SInt0 = e3;
                dst.SInt1 = e2;
                dst.SInt2 = e1;
                dst.SInt3 = e0;
                return dst;
            }

            // _mm_setr_epi16
            /// <summary> Set packed 16-bit integers in "dst" with the supplied values in reverse order. </summary>
            [DebuggerStepThrough]
            public static m128 setr_epi16(short e7, short e6, short e5, short e4, short e3, short e2, short e1, short e0)
            {
                m128 dst = default(m128);
                dst.SShort0 = e7;
                dst.SShort1 = e6;
                dst.SShort2 = e5;
                dst.SShort3 = e4;
                dst.SShort4 = e3;
                dst.SShort5 = e2;
                dst.SShort6 = e1;
                dst.SShort7 = e0;
                return dst;
            }

            // _mm_setr_epi8
            /// <summary> Set packed 8-bit integers in "dst" with the supplied values in reverse order. </summary>
            [DebuggerStepThrough]
            public static m128 setr_epi8(sbyte e15, sbyte e14, sbyte e13, sbyte e12, sbyte e11, sbyte e10, sbyte e9, sbyte e8, sbyte e7, sbyte e6, sbyte e5, sbyte e4, sbyte e3, sbyte e2, sbyte e1, sbyte e0)
            {
                m128 dst = default(m128);
                dst.SByte0 = e15;
                dst.SByte1 = e14;
                dst.SByte2 = e13;
                dst.SByte3 = e12;
                dst.SByte4 = e11;
                dst.SByte5 = e10;
                dst.SByte6 = e9;
                dst.SByte7 = e8;
                dst.SByte8 = e7;
                dst.SByte9 = e6;
                dst.SByte10 = e5;
                dst.SByte11 = e4;
                dst.SByte12 = e3;
                dst.SByte13 = e2;
                dst.SByte14 = e1;
                dst.SByte15 = e0;
                return dst;
            }

            // _mm_setzero_si128
            /// <summary> Return vector of type __m128i with all elements set to zero. </summary>
            [DebuggerStepThrough]
            public static m128 setzero_si128()
            {
                return default(m128);
            }

            // _mm_move_epi64
            /// <summary> Copy the lower 64-bit integer in "a" to the lower element of "dst", and zero the upper element. </summary>
            [DebuggerStepThrough]
            public static m128 move_epi64(m128 a)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong0;
                dst.ULong1 = 0;
                return dst;
            }

            // _mm_packs_epi16
            /// <summary> Convert packed 16-bit integers from "a" and "b" to packed 8-bit integers using signed saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 packs_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                sbyte* dptr = &dst.SByte0;
                for (int j = 0; j < 8; ++j)
                {
                    dptr[j] = Saturate_To_Int8(aptr[j]);
                }
                for (int j = 0; j < 8; ++j)
                {
                    dptr[j + 8] = Saturate_To_Int8(bptr[j]);
                }

                return dst;
            }

            // _mm_packs_epi32
            /// <summary> Convert packed 32-bit integers from "a" and "b" to packed 16-bit integers using signed saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 packs_epi32(m128 a, m128 b)
            {
                m128 dst = default(m128);
                int* aptr = &a.SInt0;
                int* bptr = &b.SInt0;
                short* dptr = &dst.SShort0;
                for (int j = 0; j < 4; ++j)
                {
                    dptr[j] = Saturate_To_Int16(aptr[j]);
                }
                for (int j = 0; j < 4; ++j)
                {
                    dptr[j + 4] = Saturate_To_Int16(bptr[j]);
                }

                return dst;
            }

            // _mm_packus_epi16
            /// <summary> Convert packed 16-bit integers from "a" and "b" to packed 8-bit integers using unsigned saturation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 packus_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                short* aptr = &a.SShort0;
                short* bptr = &b.SShort0;
                byte* dptr = &dst.Byte0;
                for (int j = 0; j < 8; ++j)
                {
                    dptr[j] = Saturate_To_UnsignedInt8(aptr[j]);
                }
                for (int j = 0; j < 8; ++j)
                {
                    dptr[j + 8] = Saturate_To_UnsignedInt8(bptr[j]);
                }

                return dst;
            }

            // _mm_extract_epi16
            /// <summary> Extract a 16-bit integer from "a", selected with "imm8", and store the result in the lower element of "dst". </summary>
            [DebuggerStepThrough]
            public static ushort extract_epi16(m128 a, int imm8)
            {
                return (&a.UShort0)[imm8 & 7];
            }

            // _mm_insert_epi16
            /// <summary> Copy "a" to "dst", and insert the 16-bit integer "i" into "dst" at the location specified by "imm8".  </summary>
            [DebuggerStepThrough]
            public static m128 insert_epi16(m128 a, int i, int imm8)
            {
                m128 dst = a;
                (&dst.SShort0)[imm8 & 7] = (short)i;
                return dst;
            }

            // _mm_movemask_epi8
            /// <summary> Create mask from the most significant bit of each 8-bit element in "a", and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static int movemask_epi8(m128 a)
            {
                int dst = 0;
                byte* aptr = &a.Byte0;
                for (int j = 0; j <= 15; j++)
                {
                    if (0 != (aptr[j] & 0x80))
                        dst |= 1 << j;
                }
                return dst;
            }

            // _mm_shuffle_epi32
            /// <summary> Shuffle 32-bit integers in "a" using the control in "imm8", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 shuffle_epi32(m128 a, int imm8)
            {
                m128 dst = default(m128);
                uint* dptr = &dst.UInt0;
                uint* aptr = &a.UInt0;
                dptr[0] = aptr[imm8 & 3];
                dptr[1] = aptr[(imm8 >> 2) & 3];
                dptr[2] = aptr[(imm8 >> 4) & 3];
                dptr[3] = aptr[(imm8 >> 6) & 3];
                return dst;
            }

            // _mm_shufflehi_epi16
            /// <summary> Shuffle 16-bit integers in the high 64 bits of "a" using the control in "imm8". Store the results in the high 64 bits of "dst", with the low 64 bits being copied from from "a" to "dst". </summary>
            [DebuggerStepThrough]
            public static m128 shufflehi_epi16(m128 a, int imm8)
            {
                m128 dst = a;
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                dptr[4] = aptr[4 + (imm8 & 3)];
                dptr[5] = aptr[4 + ((imm8 >> 2) & 3)];
                dptr[6] = aptr[4 + ((imm8 >> 4) & 3)];
                dptr[7] = aptr[4 + ((imm8 >> 6) & 3)];
                return dst;
            }

            // _mm_shufflelo_epi16
            /// <summary> Shuffle 16-bit integers in the low 64 bits of "a" using the control in "imm8". Store the results in the low 64 bits of "dst", with the high 64 bits being copied from from "a" to "dst". </summary>
            [DebuggerStepThrough]
            public static m128 shufflelo_epi16(m128 a, int imm8)
            {
                m128 dst = a;
                short* dptr = &dst.SShort0;
                short* aptr = &a.SShort0;
                dptr[0] = aptr[(imm8 & 3)];
                dptr[1] = aptr[((imm8 >> 2) & 3)];
                dptr[2] = aptr[((imm8 >> 4) & 3)];
                dptr[3] = aptr[((imm8 >> 6) & 3)];
                return dst;
            }

            // _mm_unpackhi_epi8
            /// <summary> Unpack and interleave 8-bit integers from the high half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpackhi_epi8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                for (int j = 0; j <= 7; ++j)
                {
                    dptr[2 * j] = aptr[j + 8];
                    dptr[2 * j + 1] = bptr[j + 8];
                }
                return dst;
            }

            // _mm_unpackhi_epi16
            /// <summary> Unpack and interleave 16-bit integers from the high half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpackhi_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                ushort* dptr = &dst.UShort0;
                ushort* aptr = &a.UShort0;
                ushort* bptr = &b.UShort0;
                for (int j = 0; j <= 3; ++j)
                {
                    dptr[2 * j] = aptr[j + 4];
                    dptr[2 * j + 1] = bptr[j + 4];
                }
                return dst;
            }

            // _mm_unpackhi_epi32
            /// <summary> Unpack and interleave 32-bit integers from the high half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpackhi_epi32(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = a.UInt2;
                dst.UInt1 = b.UInt2;
                dst.UInt2 = a.UInt3;
                dst.UInt3 = b.UInt3;
                return dst;
            }

            // _mm_unpackhi_epi64
            /// <summary> Unpack and interleave 64-bit integers from the high half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpackhi_epi64(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong1;
                dst.ULong1 = b.ULong1;
                return dst;
            }

            // _mm_unpacklo_epi8
            /// <summary> Unpack and interleave 8-bit integers from the low half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpacklo_epi8(m128 a, m128 b)
            {
                m128 dst = default(m128);
                byte* dptr = &dst.Byte0;
                byte* aptr = &a.Byte0;
                byte* bptr = &b.Byte0;
                for (int j = 0; j <= 7; ++j)
                {
                    dptr[2 * j] = aptr[j];
                    dptr[2 * j + 1] = bptr[j];
                }
                return dst;
            }

            // _mm_unpacklo_epi16
            /// <summary> Unpack and interleave 16-bit integers from the low half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpacklo_epi16(m128 a, m128 b)
            {
                m128 dst = default(m128);
                ushort* dptr = &dst.UShort0;
                ushort* aptr = &a.UShort0;
                ushort* bptr = &b.UShort0;
                for (int j = 0; j <= 3; ++j)
                {
                    dptr[2 * j] = aptr[j];
                    dptr[2 * j + 1] = bptr[j];
                }
                return dst;
            }

            // _mm_unpacklo_epi32
            /// <summary> Unpack and interleave 32-bit integers from the low half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpacklo_epi32(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = a.UInt0;
                dst.UInt1 = b.UInt0;
                dst.UInt2 = a.UInt1;
                dst.UInt3 = b.UInt1;
                return dst;
            }

            // _mm_unpacklo_epi64
            /// <summary> Unpack and interleave 64-bit integers from the low half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpacklo_epi64(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong0;
                dst.ULong1 = b.ULong0;
                return dst;
            }

            // _mm_add_sd
            /// <summary> Add the lower double-precision (64-bit) floating-point element in "a" and "b", store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 add_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0 + b.Double0;
                dst.Double1 = a.Double1;
                return dst;
            }

            // _mm_add_pd
            /// <summary> Add packed double-precision (64-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 add_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0 + b.Double0;
                dst.Double1 = a.Double1 + b.Double1;
                return dst;
            }

            // _mm_div_sd
            /// <summary> Divide the lower double-precision (64-bit) floating-point element in "a" by the lower double-precision (64-bit) floating-point element in "b", store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 div_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0 / b.Double0;
                dst.Double1 = a.Double1;
                return dst;
            }

            // _mm_div_pd
            /// <summary> Divide packed double-precision (64-bit) floating-point elements in "a" by packed elements in "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 div_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0 / b.Double0;
                dst.Double1 = a.Double1 / b.Double1;
                return dst;
            }

            // _mm_max_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b", store the maximum value in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 max_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = Math.Max(a.Double0, b.Double0);
                dst.Double1 = a.Double1;
                return dst;
            }

            // _mm_max_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b", and store packed maximum values in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 max_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = Math.Max(a.Double0, b.Double0);
                dst.Double1 = Math.Max(a.Double1, b.Double1);
                return dst;
            }

            // _mm_min_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b", store the minimum value in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 min_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = Math.Min(a.Double0, b.Double0);
                dst.Double1 = a.Double1;
                return dst;
            }

            // _mm_min_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b", and store packed minimum values in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 min_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = Math.Min(a.Double0, b.Double0);
                dst.Double1 = Math.Min(a.Double1, b.Double1);
                return dst;
            }

            // _mm_mul_sd
            /// <summary> Multiply the lower double-precision (64-bit) floating-point element in "a" and "b", store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 mul_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0 * b.Double0;
                dst.Double1 = a.Double1;
                return dst;
            }

            // _mm_mul_pd
            /// <summary> Multiply packed double-precision (64-bit) floating-point elements in "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 mul_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0 * b.Double0;
                dst.Double1 = a.Double1 * b.Double1;
                return dst;
            }

            // _mm_sqrt_sd
            /// <summary> Compute the square root of the lower double-precision (64-bit) floating-point element in "b", store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sqrt_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = Math.Sqrt(b.Double0);
                dst.Double1 = a.Double1;
                return dst;
            }

            // _mm_sqrt_pd
            /// <summary> Compute the square root of packed double-precision (64-bit) floating-point elements in "a", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sqrt_pd(m128 a)
            {
                m128 dst = default(m128);
                dst.Double0 = Math.Sqrt(a.Double0);
                dst.Double1 = Math.Sqrt(a.Double1);
                return dst;
            }

            // _mm_sub_sd
            /// <summary> Subtract the lower double-precision (64-bit) floating-point element in "b" from the lower double-precision (64-bit) floating-point element in "a", store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sub_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0 - b.Double0;
                dst.Double1 = a.Double1;
                return dst;
            }

            // _mm_sub_pd
            /// <summary> Subtract packed double-precision (64-bit) floating-point elements in "b" from packed double-precision (64-bit) floating-point elements in "a", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sub_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0 - b.Double0;
                dst.Double1 = a.Double1 - b.Double1;
                return dst;
            }

            // _mm_and_pd
            /// <summary> Compute the bitwise AND of packed double-precision (64-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 and_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong0 & b.ULong0;
                dst.ULong1 = a.ULong1 & b.ULong1;
                return dst;
            }

            // _mm_andnot_pd
            /// <summary> Compute the bitwise NOT of packed double-precision (64-bit) floating-point elements in "a" and then AND with "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 andnot_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (~a.ULong0) & b.ULong0;
                dst.ULong1 = (~a.ULong1) & b.ULong1;
                return dst;
            }

            // _mm_or_pd
            /// <summary> Compute the bitwise OR of packed double-precision (64-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 or_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong0 | b.ULong0;
                dst.ULong1 = a.ULong1 | b.ULong1;
                return dst;
            }

            // _mm_xor_pd
            /// <summary> Compute the bitwise XOR of packed double-precision (64-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 xor_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.ULong0 ^ b.ULong0;
                dst.ULong1 = a.ULong1 ^ b.ULong1;
                return dst;
            }

            // _mm_cmpeq_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for equality, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpeq_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.Double0 == b.Double0 ? ~0ul : 0;
                dst.ULong1 = a.ULong1;
                return dst;
            }

            // _mm_cmplt_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for less-than, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmplt_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.Double0 < b.Double0 ? ~0ul : 0;
                dst.ULong1 = a.ULong1;
                return dst;
            }

            // _mm_cmple_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for less-than-or-equal, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmple_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.Double0 <= b.Double0 ? ~0ul : 0;
                dst.ULong1 = a.ULong1;
                return dst;
            }

            // _mm_cmpgt_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for greater-than, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpgt_sd(m128 a, m128 b)
            {
                return cmple_sd(b, a);
            }

            // _mm_cmpge_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for greater-than-or-equal, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpge_sd(m128 a, m128 b)
            {
                return cmplt_sd(b, a);
            }

            // _mm_cmpord_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" to see if neither is NaN, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpord_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = IsNaN(a.ULong0) || IsNaN(b.ULong0) ? 0 : ~0ul;
                dst.ULong1 = a.ULong1;
                return dst;
            }

            // _mm_cmpunord_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" to see if either is NaN, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpunord_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = IsNaN(a.ULong0) || IsNaN(b.ULong0) ? ~0ul : 0;
                dst.ULong1 = a.ULong1;
                return dst;
            }

            // _mm_cmpneq_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for not-equal, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpneq_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = a.Double0 != b.Double0 ? ~0ul : 0;
                dst.ULong1 = a.ULong1;
                return dst;
            }

            // _mm_cmpnlt_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for not-less-than, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnlt_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = !(a.Double0 < b.Double0) ? ~0ul : 0;
                dst.ULong1 = a.ULong1;
                return dst;
            }

            // _mm_cmpnle_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for not-less-than-or-equal, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnle_sd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = !(a.Double0 <= b.Double0) ? ~0ul : 0;
                dst.ULong1 = a.ULong1;
                return dst;
            }

            // _mm_cmpngt_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for not-greater-than, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpngt_sd(m128 a, m128 b)
            {
                return cmpnlt_sd(b, a);
            }

            // _mm_cmpnge_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point elements in "a" and "b" for not-greater-than-or-equal, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnge_sd(m128 a, m128 b)
            {
                return cmpnle_sd(b, a);
            }

            // _mm_cmpeq_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for equality, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpeq_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (a.Double0 == b.Double0) ? ~0ul : 0;
                dst.ULong1 = (a.Double1 == b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmplt_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for less-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmplt_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (a.Double0 < b.Double0) ? ~0ul : 0;
                dst.ULong1 = (a.Double1 < b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmple_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for less-than-or-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmple_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (a.Double0 <= b.Double0) ? ~0ul : 0;
                dst.ULong1 = (a.Double1 <= b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmpgt_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for greater-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpgt_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (a.Double0 > b.Double0) ? ~0ul : 0;
                dst.ULong1 = (a.Double1 > b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmpge_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for greater-than-or-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpge_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (a.Double0 >= b.Double0) ? ~0ul : 0;
                dst.ULong1 = (a.Double1 >= b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmpord_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" to see if neither is NaN, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpord_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (IsNaN(a.ULong0) || IsNaN(b.ULong0)) ? 0 : ~0ul;
                dst.ULong1 = (IsNaN(a.ULong1) || IsNaN(b.ULong1)) ? 0 : ~0ul;
                return dst;
            }

            // _mm_cmpunord_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" to see if either is NaN, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpunord_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (IsNaN(a.ULong0) || IsNaN(b.ULong0)) ? ~0ul : 0;
                dst.ULong1 = (IsNaN(a.ULong1) || IsNaN(b.ULong1)) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmpneq_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for not-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpneq_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = (a.Double0 != b.Double0) ? ~0ul : 0;
                dst.ULong1 = (a.Double1 != b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmpnlt_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for not-less-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnlt_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = !(a.Double0 < b.Double0) ? ~0ul : 0;
                dst.ULong1 = !(a.Double1 < b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmpnle_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for not-less-than-or-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnle_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = !(a.Double0 <= b.Double0) ? ~0ul : 0;
                dst.ULong1 = !(a.Double1 <= b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmpngt_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for not-greater-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpngt_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = !(a.Double0 > b.Double0) ? ~0ul : 0;
                dst.ULong1 = !(a.Double1 > b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_cmpnge_pd
            /// <summary> Compare packed double-precision (64-bit) floating-point elements in "a" and "b" for not-greater-than-or-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnge_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.ULong0 = !(a.Double0 >= b.Double0) ? ~0ul : 0;
                dst.ULong1 = !(a.Double1 >= b.Double1) ? ~0ul : 0;
                return dst;
            }

            // _mm_comieq_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for equality, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comieq_sd(m128 a, m128 b)
            {
                return a.Double0 == b.Double0 ? 1 : 0;
            }

            // _mm_comilt_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for less-than, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comilt_sd(m128 a, m128 b)
            {
                return a.Double0 < b.Double0 ? 1 : 0;
            }

            // _mm_comile_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for less-than-or-equal, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comile_sd(m128 a, m128 b)
            {
                return a.Double0 <= b.Double0 ? 1 : 0;
            }

            // _mm_comigt_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for greater-than, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comigt_sd(m128 a, m128 b)
            {
                return a.Double0 > b.Double0 ? 1 : 0;
            }

            // _mm_comige_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for greater-than-or-equal, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comige_sd(m128 a, m128 b)
            {
                return a.Double0 >= b.Double0 ? 1 : 0;
            }

            // _mm_comineq_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for not-equal, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comineq_sd(m128 a, m128 b)
            {
                return a.Double0 != b.Double0 ? 1 : 0;
            }

            // _mm_ucomieq_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for equality, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomieq_sd(m128 a, m128 b)
            {
                return a.Double0 == b.Double0 ? 1 : 0;
            }

            // _mm_ucomilt_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for less-than, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomilt_sd(m128 a, m128 b)
            {
                return a.Double0 < b.Double0 ? 1 : 0;
            }

            // _mm_ucomile_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for less-than-or-equal, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomile_sd(m128 a, m128 b)
            {
                return a.Double0 <= b.Double0 ? 1 : 0;
            }

            // _mm_ucomigt_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for greater-than, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomigt_sd(m128 a, m128 b)
            {
                return a.Double0 > b.Double0 ? 1 : 0;
            }

            // _mm_ucomige_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for greater-than-or-equal, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomige_sd(m128 a, m128 b)
            {
                return a.Double0 >= b.Double0 ? 1 : 0;
            }

            // _mm_ucomineq_sd
            /// <summary> Compare the lower double-precision (64-bit) floating-point element in "a" and "b" for not-equal, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomineq_sd(m128 a, m128 b)
            {
                return a.Double0 != b.Double0 ? 1 : 0;
            }

            // _mm_cvtpd_ps
            /// <summary> Convert packed double-precision (64-bit) floating-point elements in "a" to packed single-precision (32-bit) floating-point elements, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtpd_ps(m128 a)
            {
                m128 dst = default(m128);
                dst.Float0 = (float)a.Double0;
                dst.Float1 = (float)a.Double1;
                dst.Float2 = 0;
                dst.Float3 = 0;
                return dst;
            }

            // _mm_cvtps_pd
            /// <summary> Convert packed single-precision (32-bit) floating-point elements in "a" to packed double-precision (64-bit) floating-point elements, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtps_pd(m128 a)
            {
                // The normal Burst IR does fine here.
                m128 dst = default(m128);
                dst.Double0 = a.Float0;
                dst.Double1 = a.Float1;
                return dst;
            }

            // _mm_cvtpd_epi32
            /// <summary> Convert packed double-precision (64-bit) floating-point elements in "a" to packed 32-bit integers, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtpd_epi32(m128 a)
            {
                m128 dst = default(m128);
                dst.SInt0 = (int)Math.Round(a.Double0);
                dst.SInt1 = (int)Math.Round(a.Double1);
                return dst;
            }

            // _mm_cvtsd_si32
            /// <summary> Convert the lower double-precision (64-bit) floating-point element in "a" to a 32-bit integer, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static int cvtsd_si32(m128 a)
            {
                return (int)Math.Round(a.Double0);
            }

            // _mm_cvtsd_si64
            /// <summary> Convert the lower double-precision (64-bit) floating-point element in "a" to a 64-bit integer, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static long cvtsd_si64(m128 a)
            {
                return (long)Math.Round(a.Double0);
            }

            // _mm_cvtsd_si64x
            /// <summary> Convert the lower double-precision (64-bit) floating-point element in "a" to a 64-bit integer, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static long cvtsd_si64x(m128 a)
            {
                return cvtsd_si64(a);
            }

            // _mm_cvtsd_ss
            /// <summary> Convert the lower double-precision (64-bit) floating-point element in "b" to a single-precision (32-bit) floating-point element, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtsd_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 = (float)b.Double0;
                return dst;
            }

            // _mm_cvtsd_f64
            /// <summary> Copy the lower double-precision (64-bit) floating-point element of "a" to "dst". </summary>
            [DebuggerStepThrough]
            public static double cvtsd_f64(m128 a)
            {
                // Burst IR is OK
                return a.Double0;
            }

            // _mm_cvtss_sd
            /// <summary> Convert the lower single-precision (32-bit) floating-point element in "b" to a double-precision (64-bit) floating-point element, store the result in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtss_sd(m128 a, m128 b)
            {
                // Burst IR is OK
                m128 dst = default(m128);
                dst.Double0 = b.Float0;
                dst.Double1 = a.Float0;
                return dst;
            }

            // _mm_cvttpd_epi32
            /// <summary> Convert packed double-precision (64-bit) floating-point elements in "a" to packed 32-bit integers with truncation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvttpd_epi32(m128 a)
            {
                m128 dst = default(m128);
                dst.SInt0 = (int)a.Double0;
                dst.SInt1 = (int)a.Double1;
                return dst;
            }

            // _mm_cvttsd_si32
            /// <summary> Convert the lower double-precision (64-bit) floating-point element in "a" to a 32-bit integer with truncation, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static int cvttsd_si32(m128 a)
            {
                return (int)a.Double0;
            }

            // _mm_cvttsd_si64
            /// <summary> Convert the lower double-precision (64-bit) floating-point element in "a" to a 64-bit integer with truncation, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static long cvttsd_si64(m128 a)
            {
                return (long)a.Double0;
            }

            // _mm_cvttsd_si64x
            /// <summary> Convert the lower double-precision (64-bit) floating-point element in "a" to a 64-bit integer with truncation, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static long cvttsd_si64x(m128 a)
            {
                return cvttsd_si64(a);
            }

            // _mm_cvtps_epi32
            /// <summary> Convert packed single-precision (32-bit) floating-point elements in "a" to packed 32-bit integers, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtps_epi32(m128 a)
            {
                m128 dst = default(m128);
                dst.SInt0 = (int)Math.Round(a.Float0);
                dst.SInt1 = (int)Math.Round(a.Float1);
                dst.SInt2 = (int)Math.Round(a.Float2);
                dst.SInt3 = (int)Math.Round(a.Float3);
                return dst;
            }

            // _mm_cvttps_epi32
            /// <summary> Convert packed single-precision (32-bit) floating-point elements in "a" to packed 32-bit integers with truncation, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvttps_epi32(m128 a)
            {
                m128 dst = default(m128);
                dst.SInt0 = (int)a.Float0;
                dst.SInt1 = (int)a.Float1;
                dst.SInt2 = (int)a.Float2;
                dst.SInt3 = (int)a.Float3;
                return dst;
            }

            // _mm_set_sd
            /// <summary> Copy double-precision (64-bit) floating-point element "a" to the lower element of "dst", and zero the upper element. </summary>
            [DebuggerStepThrough]
            public static m128 set_sd(double a)
            {
                // Burst IR is fine.
                m128 dst = default(m128);
                dst.Double0 = a;
                dst.Double1 = 0.0;
                return dst;
            }

            // _mm_set1_pd
            /// <summary> Broadcast double-precision (64-bit) floating-point value "a" to all elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 set1_pd(double a)
            {
                // Burst IR is fine.
                m128 dst = default(m128);
                dst.Double0 = dst.Double1 = a;
                return dst;
            }

            // _mm_set_pd1
            /// <summary> Broadcast double-precision (64-bit) floating-point value "a" to all elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 set_pd1(double a)
            {
                // Burst IR is fine.
                return set1_pd(a);
            }

            // _mm_set_pd
            /// <summary> Set packed double-precision (64-bit) floating-point elements in "dst" with the supplied values. </summary>
            [DebuggerStepThrough]
            public static m128 set_pd(double e1, double e0)
            {
                // Burst IR is fine.
                m128 dst = default(m128);
                dst.Double0 = e0;
                dst.Double1 = e1;
                return dst;
            }

            // _mm_setr_pd
            /// <summary> Set packed double-precision (64-bit) floating-point elements in "dst" with the supplied values in reverse order. </summary>
            [DebuggerStepThrough]
            public static m128 setr_pd(double e1, double e0)
            {
                // Burst IR is fine.
                m128 dst = default(m128);
                dst.Double0 = e1;
                dst.Double1 = e0;
                return dst;
            }

            // _mm_unpackhi_pd
            /// <summary> Unpack and interleave double-precision (64-bit) floating-point elements from the high half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpackhi_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double1;
                dst.Double1 = b.Double1;
                return dst;
            }

            // _mm_unpacklo_pd
            /// <summary> Unpack and interleave double-precision (64-bit) floating-point elements from the low half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpacklo_pd(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Double0 = a.Double0;
                dst.Double1 = b.Double0;
                return dst;
            }

            // _mm_movemask_pd
            /// <summary> Set each bit of mask "dst" based on the most significant bit of the corresponding packed double-precision (64-bit) floating-point element in "a". </summary>
            [DebuggerStepThrough]
            public static int movemask_pd(m128 a)
            {
                int dst = 0;
                if ((a.ULong0 & 0x8000000000000000ul) != 0)
                    dst |= 1;
                if ((a.ULong1 & 0x8000000000000000ul) != 0)
                    dst |= 2;
                return dst;
            }

            // _mm_shuffle_pd
            /// <summary> Shuffle double-precision (64-bit) floating-point elements using the control in "imm8", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 shuffle_pd(m128 a, m128 b, int imm8)
            {
                m128 dst = default(m128);
                double* aptr = &a.Double0;
                double* bptr = &b.Double0;
                dst.Double0 = aptr[(imm8 & 1)];
                dst.Double1 = bptr[((imm8 >> 1) & 1)];
                return dst;
            }

            // _mm_move_sd
            /// <summary> Move the lower double-precision (64-bit) floating-point element from "b" to the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 move_sd(m128 a, m128 b)
            {
                // Burst IR is fine.
                m128 dst = default(m128);
                dst.Double0 = b.Double0;
                dst.Double1 = a.Double1;
                return dst;
            }
        }
    }
}

#endif
