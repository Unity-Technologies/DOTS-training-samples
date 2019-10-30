using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#if BURST_INTERNAL || UNITY_BURST_EXPERIMENTAL_X86_INTRINSICS

namespace Unity.Burst.Intrinsics
{
    public unsafe static partial class X86
    {
        public static class Sse
        {

            [DebuggerStepThrough]
            public static m128 load_ps_128(void* ptr)
            {
                return GenericCSharpLoad(ptr);
            }

            [DebuggerStepThrough]
            public static m128 loadu_ps_128(void* ptr)
            {
                return GenericCSharpLoad(ptr);
            }

            [DebuggerStepThrough]
            public static void store_ps(void* ptr, m128 val)
            {
                GenericCSharpStore(ptr, val);
            }

            [DebuggerStepThrough]
            public static void storeu_ps(void* ptr, m128 val)
            {
                GenericCSharpStore(ptr, val);
            }

            [DebuggerStepThrough]
            public static m128 load_si128(void* ptr)
            {
                return GenericCSharpLoad(ptr);
            }

            [DebuggerStepThrough]
            public static m128 loadu_si128(void* ptr)
            {
                return GenericCSharpLoad(ptr);
            }

            [DebuggerStepThrough]
            public static void store_si128(void* ptr, m128 val)
            {
                GenericCSharpStore(ptr, val);
            }

            [DebuggerStepThrough]
            public static void storeu_si128(void* ptr, m128 val)
            {
                GenericCSharpStore(ptr, val);
            }

            /// <summary>
            /// Store 128-bits (composed of 4 packed single-precision (32-bit) floating-point elements) from "a" into memory using a non-temporal memory hint. "mem_addr" must be aligned on a 16-byte boundary or a general-protection exception will be generated.
            /// </summary>
            [DebuggerStepThrough]
            public static void stream_ps(void* mem_addr, m128 a)
            {
                CheckPointerAlignment16(mem_addr);
                GenericCSharpStore(mem_addr, a);
            }

            // _mm_cvtsi32_ss
            /// <summary> Convert the 32-bit integer "b" to a single-precision (32-bit) floating-point element, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtsi32_ss(m128 a, int b)
            {
                m128 dst = a;
                dst.Float0 = b;
                return dst;
            }

            // _mm_cvtsi64_ss
            /// <summary> Convert the 64-bit integer "b" to a single-precision (32-bit) floating-point element, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cvtsi64_ss(m128 a, long b)
            {
                m128 dst = a;
                dst.Float0 = b;
                return dst;
            }

            // _mm_add_ss
            /// <summary> Add the lower single-precision (32-bit) floating-point element in "a" and "b", store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 add_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 = dst.Float0 + b.Float0;
                return dst;
            }

            // _mm_add_ps
            /// <summary> Add packed single-precision (32-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 add_ps(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 += b.Float0;
                dst.Float1 += b.Float1;
                dst.Float2 += b.Float2;
                dst.Float3 += b.Float3;
                return dst;
            }

            // _mm_sub_ss
            /// <summary> Subtract the lower single-precision (32-bit) floating-point element in "b" from the lower single-precision (32-bit) floating-point element in "a", store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sub_ss(m128 a, m128 b)
            {
                m128 dst = a;
                a.Float0 = a.Float0 - b.Float0;
                return dst;
            }

            // _mm_sub_ps
            /// <summary> Subtract packed single-precision (32-bit) floating-point elements in "b" from packed single-precision (32-bit) floating-point elements in "a", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sub_ps(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 -= b.Float0;
                dst.Float1 -= b.Float1;
                dst.Float2 -= b.Float2;
                dst.Float3 -= b.Float3;
                return dst;
            }

            // _mm_mul_ss
            /// <summary> Multiply the lower single-precision (32-bit) floating-point element in "a" and "b", store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 mul_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 = a.Float0 * b.Float0;
                return dst;
            }

            // _mm_mul_ps
            /// <summary> Multiply packed single-precision (32-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 mul_ps(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 *= b.Float0;
                dst.Float1 *= b.Float1;
                dst.Float2 *= b.Float2;
                dst.Float3 *= b.Float3;
                return dst;
            }

            // _mm_div_ss
            /// <summary> Divide the lower single-precision (32-bit) floating-point element in "a" by the lower single-precision (32-bit) floating-point element in "b", store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 div_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 = a.Float0 / b.Float0;
                return dst;
            }

            // _mm_div_ps
            /// <summary> Divide packed single-precision (32-bit) floating-point elements in "a" by packed elements in "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 div_ps(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 /= b.Float0;
                dst.Float1 /= b.Float1;
                dst.Float2 /= b.Float2;
                dst.Float3 /= b.Float3;
                return dst;
            }

            // _mm_sqrt_ss
            /// <summary> Compute the square root of the lower single-precision (32-bit) floating-point element in "a", store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sqrt_ss(m128 a)
            {
                m128 dst = a;
                dst.Float0 = (float)Math.Sqrt(a.Float0);
                return dst;
            }

            // _mm_sqrt_ps
            /// <summary> Compute the square root of packed single-precision (32-bit) floating-point elements in "a", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 sqrt_ps(m128 a)
            {
                m128 dst = default(m128);
                dst.Float0 = (float)Math.Sqrt(a.Float0);
                dst.Float1 = (float)Math.Sqrt(a.Float1);
                dst.Float2 = (float)Math.Sqrt(a.Float2);
                dst.Float3 = (float)Math.Sqrt(a.Float3);
                return dst;
            }

            // _mm_rcp_ss
            /// <summary> Compute the approximate reciprocal of the lower single-precision (32-bit) floating-point element in "a", store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". The maximum relative error for this approximation is less than 1.5*2^-12. </summary>
            [DebuggerStepThrough]
            public static m128 rcp_ss(m128 a)
            {
                m128 dst = a;
                dst.Float0 = 1.0f / a.Float0;
                return dst;
            }

            // _mm_rcp_ps
            /// <summary> Compute the approximate reciprocal of packed single-precision (32-bit) floating-point elements in "a", and store the results in "dst". The maximum relative error for this approximation is less than 1.5*2^-12. </summary>
            [DebuggerStepThrough]
            public static m128 rcp_ps(m128 a)
            {
                m128 dst = default(m128);
                dst.Float0 = 1.0f / a.Float0;
                dst.Float1 = 1.0f / a.Float1;
                dst.Float2 = 1.0f / a.Float2;
                dst.Float3 = 1.0f / a.Float3;
                return dst;
            }

            // _mm_rsqrt_ss
            /// <summary> Compute the approximate reciprocal square root of the lower single-precision (32-bit) floating-point element in "a", store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". The maximum relative error for this approximation is less than 1.5*2^-12. </summary>
            [DebuggerStepThrough]
            public static m128 rsqrt_ss(m128 a)
            {
                m128 dst = a;
                dst.Float0 = 1.0f / (float)Math.Sqrt(a.Float0);
                return dst;
            }

            // _mm_rsqrt_ps
            /// <summary> Compute the approximate reciprocal square root of packed single-precision (32-bit) floating-point elements in "a", and store the results in "dst". The maximum relative error for this approximation is less than 1.5*2^-12. </summary>
            [DebuggerStepThrough]
            public static m128 rsqrt_ps(m128 a)
            {
                m128 dst = default(m128);
                dst.Float0 = 1.0f / (float)Math.Sqrt(a.Float0);
                dst.Float1 = 1.0f / (float)Math.Sqrt(a.Float1);
                dst.Float2 = 1.0f / (float)Math.Sqrt(a.Float2);
                dst.Float3 = 1.0f / (float)Math.Sqrt(a.Float3);
                return dst;
            }

            // _mm_min_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b", store the minimum value in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 min_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 = Math.Min(a.Float0, b.Float0);
                return dst;
            }

            // _mm_min_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b", and store packed minimum values in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 min_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Float0 = Math.Min(a.Float0, b.Float0);
                dst.Float1 = Math.Min(a.Float1, b.Float1);
                dst.Float2 = Math.Min(a.Float2, b.Float2);
                dst.Float3 = Math.Min(a.Float3, b.Float3);
                return dst;
            }

            // _mm_max_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b", store the maximum value in the lower element of "dst", and copy the upper element from "a" to the upper element of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 max_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 = Math.Max(a.Float0, b.Float0);
                return dst;
            }

            // _mm_max_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b", and store packed maximum values in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 max_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Float0 = Math.Max(a.Float0, b.Float0);
                dst.Float1 = Math.Max(a.Float1, b.Float1);
                dst.Float2 = Math.Max(a.Float2, b.Float2);
                dst.Float3 = Math.Max(a.Float3, b.Float3);
                return dst;
            }

            // _mm_and_ps
            /// <summary> Compute the bitwise AND of packed single-precision (32-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 and_ps(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 &= b.UInt0;
                dst.UInt1 &= b.UInt1;
                dst.UInt2 &= b.UInt2;
                dst.UInt3 &= b.UInt3;
                return dst;
            }

            // _mm_andnot_ps
            /// <summary> Compute the bitwise NOT of packed single-precision (32-bit) floating-point elements in "a" and then AND with "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 andnot_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = (~a.UInt0) & b.UInt0;
                dst.UInt1 = (~a.UInt1) & b.UInt1;
                dst.UInt2 = (~a.UInt2) & b.UInt2;
                dst.UInt3 = (~a.UInt3) & b.UInt3;
                return dst;
            }

            // _mm_or_ps
            /// <summary> Compute the bitwise OR of packed single-precision (32-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 or_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = a.UInt0 | b.UInt0;
                dst.UInt1 = a.UInt1 | b.UInt1;
                dst.UInt2 = a.UInt2 | b.UInt2;
                dst.UInt3 = a.UInt3 | b.UInt3;
                return dst;
            }

            // _mm_xor_ps
            /// <summary> Compute the bitwise XOR of packed single-precision (32-bit) floating-point elements in "a" and "b", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 xor_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = a.UInt0 ^ b.UInt0;
                dst.UInt1 = a.UInt1 ^ b.UInt1;
                dst.UInt2 = a.UInt2 ^ b.UInt2;
                dst.UInt3 = a.UInt3 ^ b.UInt3;
                return dst;
            }

            // _mm_cmpeq_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for equality, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpeq_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 = a.Float0 == b.Float0 ? ~0u : 0;
                return dst;
            }

            // _mm_cmpeq_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for equality, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpeq_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = a.Float0 == b.Float0 ? ~0u : 0;
                dst.UInt1 = a.Float1 == b.Float1 ? ~0u : 0;
                dst.UInt2 = a.Float2 == b.Float2 ? ~0u : 0;
                dst.UInt3 = a.Float3 == b.Float3 ? ~0u : 0;
                return dst;
            }

            // _mm_cmplt_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for less-than, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmplt_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 = a.Float0 < b.Float0 ? ~0u : 0u;
                return dst;
            }

            // _mm_cmplt_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for less-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmplt_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = a.Float0 < b.Float0 ? ~0u : 0u;
                dst.UInt1 = a.Float1 < b.Float1 ? ~0u : 0u;
                dst.UInt2 = a.Float2 < b.Float2 ? ~0u : 0u;
                dst.UInt3 = a.Float3 < b.Float3 ? ~0u : 0u;
                return dst;
            }

            // _mm_cmple_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for less-than-or-equal, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmple_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 = a.Float0 <= b.Float0 ? ~0u : 0;
                return dst;
            }

            // _mm_cmple_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for less-than-or-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmple_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = a.Float0 <= b.Float0 ? ~0u : 0u;
                dst.UInt1 = a.Float1 <= b.Float1 ? ~0u : 0u;
                dst.UInt2 = a.Float2 <= b.Float2 ? ~0u : 0u;
                dst.UInt3 = a.Float3 <= b.Float3 ? ~0u : 0u;
                return dst;
            }

            // _mm_cmpgt_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for greater-than, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpgt_ss(m128 a, m128 b)
            {
                return cmplt_ss(b, a);
            }

            // _mm_cmpgt_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for greater-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpgt_ps(m128 a, m128 b)
            {
                return cmplt_ps(b, a);
            }

            // _mm_cmpge_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for greater-than-or-equal, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpge_ss(m128 a, m128 b)
            {
                return cmple_ss(b, a);
            }

            // _mm_cmpge_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for greater-than-or-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpge_ps(m128 a, m128 b)
            {
                return cmple_ps(b, a);
            }

            // _mm_cmpneq_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for not-equal, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpneq_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 = a.Float0 != b.Float0 ? ~0u : 0u;
                return dst;
            }

            // _mm_cmpneq_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for not-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpneq_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = a.Float0 != b.Float0 ? ~0u : 0u;
                dst.UInt1 = a.Float1 != b.Float1 ? ~0u : 0u;
                dst.UInt2 = a.Float2 != b.Float2 ? ~0u : 0u;
                dst.UInt3 = a.Float3 != b.Float3 ? ~0u : 0u;
                return dst;
            }

            // _mm_cmpnlt_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for not-less-than, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnlt_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 = !(a.Float0 < b.Float0) ? ~0u : 0u;
                return dst;
            }

            // _mm_cmpnlt_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for not-less-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnlt_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = !(a.Float0 < b.Float0) ? ~0u : 0u;
                dst.UInt1 = !(a.Float1 < b.Float1) ? ~0u : 0u;
                dst.UInt2 = !(a.Float2 < b.Float2) ? ~0u : 0u;
                dst.UInt3 = !(a.Float3 < b.Float3) ? ~0u : 0u;
                return dst;
            }

            // _mm_cmpnle_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for not-less-than-or-equal, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnle_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 = !(a.Float0 <= b.Float0) ? ~0u : 0u;
                return dst;
            }

            // _mm_cmpnle_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for not-less-than-or-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnle_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = !(a.Float0 <= b.Float0) ? ~0u : 0u;
                dst.UInt1 = !(a.Float1 <= b.Float1) ? ~0u : 0u;
                dst.UInt2 = !(a.Float2 <= b.Float2) ? ~0u : 0u;
                dst.UInt3 = !(a.Float3 <= b.Float3) ? ~0u : 0u;
                return dst;
            }

            // _mm_cmpngt_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for not-greater-than, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpngt_ss(m128 a, m128 b)
            {
                return cmpnlt_ss(b, a);
            }

            // _mm_cmpngt_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for not-greater-than, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpngt_ps(m128 a, m128 b)
            {
                return cmpnlt_ps(b, a);
            }

            // _mm_cmpnge_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" for not-greater-than-or-equal, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnge_ss(m128 a, m128 b)
            {
                return cmpnle_ss(b, a);
            }

            // _mm_cmpnge_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" for not-greater-than-or-equal, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpnge_ps(m128 a, m128 b)
            {
                return cmpnle_ps(b, a);
            }

            // _mm_cmpord_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" to see if neither is NaN, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpord_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 = IsNaN(a.UInt0) || IsNaN(b.UInt0) ? 0 : ~0u;
                return dst;
            }

            // _mm_cmpord_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" to see if neither is NaN, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpord_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = IsNaN(a.UInt0) || IsNaN(b.UInt0) ? 0 : ~0u;
                dst.UInt1 = IsNaN(a.UInt1) || IsNaN(b.UInt1) ? 0 : ~0u;
                dst.UInt2 = IsNaN(a.UInt2) || IsNaN(b.UInt2) ? 0 : ~0u;
                dst.UInt3 = IsNaN(a.UInt3) || IsNaN(b.UInt3) ? 0 : ~0u;
                return dst;
            }

            // _mm_cmpunord_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point elements in "a" and "b" to see if either is NaN, store the result in the lower element of "dst", and copy the upper 3 packed elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpunord_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.UInt0 = IsNaN(a.UInt0) || IsNaN(b.UInt0) ? ~0u : 0;
                return dst;
            }

            // _mm_cmpunord_ps
            /// <summary> Compare packed single-precision (32-bit) floating-point elements in "a" and "b" to see if either is NaN, and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 cmpunord_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.UInt0 = IsNaN(a.UInt0) || IsNaN(b.UInt0) ? ~0u : 0;
                dst.UInt1 = IsNaN(a.UInt1) || IsNaN(b.UInt1) ? ~0u : 0;
                dst.UInt2 = IsNaN(a.UInt2) || IsNaN(b.UInt2) ? ~0u : 0;
                dst.UInt3 = IsNaN(a.UInt3) || IsNaN(b.UInt3) ? ~0u : 0;
                return dst;
            }

            // _mm_comieq_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for equality, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comieq_ss(m128 a, m128 b)
            {
                return a.Float0 == b.Float0 ? 1 : 0;
            }

            // _mm_comilt_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for less-than, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comilt_ss(m128 a, m128 b)
            {
                return a.Float0 < b.Float0 ? 1 : 0;
            }

            // _mm_comile_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for less-than-or-equal, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comile_ss(m128 a, m128 b)
            {
                return a.Float0 <= b.Float0 ? 1 : 0;
            }

            // _mm_comigt_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for greater-than, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comigt_ss(m128 a, m128 b)
            {
                return a.Float0 > b.Float0 ? 1 : 0;
            }

            // _mm_comige_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for greater-than-or-equal, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comige_ss(m128 a, m128 b)
            {
                return a.Float0 >= b.Float0 ? 1 : 0;
            }

            // _mm_comineq_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for not-equal, and return the boolean result (0 or 1). </summary>
            [DebuggerStepThrough]
            public static int comineq_ss(m128 a, m128 b)
            {
                return a.Float0 != b.Float0 ? 1 : 0;
            }

            // _mm_ucomieq_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for equality, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomieq_ss(m128 a, m128 b)
            {
                return a.Float0 == b.Float0 ? 1 : 0;
            }

            // _mm_ucomilt_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for less-than, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomilt_ss(m128 a, m128 b)
            {
                return a.Float0 < b.Float0 ? 1 : 0;
            }

            // _mm_ucomile_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for less-than-or-equal, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomile_ss(m128 a, m128 b)
            {
                return a.Float0 <= b.Float0 ? 1 : 0;
            }

            // _mm_ucomigt_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for greater-than, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomigt_ss(m128 a, m128 b)
            {
                return a.Float0 > b.Float0 ? 1 : 0;
            }

            // _mm_ucomige_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for greater-than-or-equal, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomige_ss(m128 a, m128 b)
            {
                return a.Float0 >= b.Float0 ? 1 : 0;
            }

            // _mm_ucomineq_ss
            /// <summary> Compare the lower single-precision (32-bit) floating-point element in "a" and "b" for not-equal, and return the boolean result (0 or 1). This instruction will not signal an exception for QNaNs. </summary>
            [DebuggerStepThrough]
            public static int ucomineq_ss(m128 a, m128 b)
            {
                return a.Float0 != b.Float0 ? 1 : 0;
            }

            // _mm_cvtss_si32
            /// <summary> Convert the lower single-precision (32-bit) floating-point element in "a" to a 32-bit integer, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static int cvtss_si32(m128 a)
            {
                return cvt_ss2si(a);
            }

            // _mm_cvt_ss2si
            /// <summary> Convert the lower single-precision (32-bit) floating-point element in "a" to a 32-bit integer, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static int cvt_ss2si(m128 a)
            {
                return (int)a.Float0;
            }

            // _mm_cvtss_si64
            /// <summary> Convert the lower single-precision (32-bit) floating-point element in "a" to a 64-bit integer, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static long cvtss_si64(m128 a)
            {
                return (long)a.Float0;
            }

            // _mm_cvtss_f32
            /// <summary> Copy the lower single-precision (32-bit) floating-point element of "a" to "dst". </summary>
            [DebuggerStepThrough]
            public static float cvtss_f32(m128 a)
            {
                return a.Float0;
            }

            // _mm_cvttss_si32
            /// <summary> Convert the lower single-precision (32-bit) floating-point element in "a" to a 32-bit integer with truncation, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static int cvttss_si32(m128 a)
            {
                using (var csr = new RoundingScope(MXCSRBits.RoundTowardZero))
                {
                    return (int)a.Float0;
                }
            }

            // _mm_cvtt_ss2si
            /// <summary> Convert the lower single-precision (32-bit) floating-point element in "a" to a 32-bit integer with truncation, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static int cvtt_ss2si(m128 a)
            {
                return cvttss_si32(a);
            }

            // _mm_cvttss_si64
            /// <summary> Convert the lower single-precision (32-bit) floating-point element in "a" to a 64-bit integer with truncation, and store the result in "dst". </summary>
            [DebuggerStepThrough]
            public static long cvttss_si64(m128 a)
            {
                using (var csr = new RoundingScope(MXCSRBits.RoundTowardZero))
                {
                    return (long)a.Float0;
                }
            }

            // _mm_set_ss
            /// <summary> Copy single-precision (32-bit) floating-point element "a" to the lower element of "dst", and zero the upper 3 elements. </summary>
            [DebuggerStepThrough]
            public static m128 set_ss(float a)
            {
                return new m128(a, 0.0f, 0.0f, 0.0f);
            }

            // _mm_set1_ps
            /// <summary> Broadcast single-precision (32-bit) floating-point value "a" to all elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 set1_ps(float a)
            {
                return new m128(a, a, a, a);
            }

            // _mm_set_ps1
            /// <summary> Broadcast single-precision (32-bit) floating-point value "a" to all elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 set_ps1(float a)
            {
                return set1_ps(a);
            }

            // _mm_set_ps
            /// <summary> Set packed single-precision (32-bit) floating-point elements in "dst" with the supplied values. </summary>
            [DebuggerStepThrough]
            public static m128 set_ps(float e3, float e2, float e1, float e0)
            {
                return new m128(e0, e1, e2, e3);
            }

            // _mm_setr_ps
            /// <summary> Set packed single-precision (32-bit) floating-point elements in "dst" with the supplied values in reverse order. </summary>
            [DebuggerStepThrough]
            public static m128 setr_ps(float e3, float e2, float e1, float e0)
            {
                return new m128(e3, e2, e1, e0);
            }

            // _mm_move_ss
            /// <summary> Move the lower single-precision (32-bit) floating-point element from "b" to the lower element of "dst", and copy the upper 3 elements from "a" to the upper elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 move_ss(m128 a, m128 b)
            {
                m128 dst = a;
                dst.Float0 = b.Float0;
                return dst;
            }

            // _MM_SHUFFLE macro
            /// <summary>
            /// Return a shuffle immediate suitable for use with _mm_shuffle_ps and similar instructions.
            /// </summary>
            public static int SHUFFLE(int a, int b, int c, int d)
            {
                return ((a & 3)) | ((b & 3) << 2) | ((c & 3) << 4) | ((d & 3) << 6);
            }

            // _mm_shuffle_ps
            /// <summary> Shuffle single-precision (32-bit) floating-point elements in "a" using the control in "imm8", and store the results in "dst". </summary>
            [DebuggerStepThrough]
            public static m128 shuffle_ps(m128 a, m128 b, int imm8)
            {
                m128 dst = default(m128);
                float* aptr = &a.Float0;
                float* bptr = &b.Float0;
                dst.Float0 = aptr[(imm8 >> 0) & 3];
                dst.Float1 = aptr[(imm8 >> 2) & 3];
                dst.Float2 = bptr[(imm8 >> 4) & 3];
                dst.Float3 = bptr[(imm8 >> 6) & 3];
                return dst;
            }

            // _mm_unpackhi_ps
            /// <summary> Unpack and interleave single-precision (32-bit) floating-point elements from the high half "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpackhi_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Float0 = a.Float2;
                dst.Float1 = b.Float2;
                dst.Float2 = a.Float3;
                dst.Float3 = b.Float3;
                return dst;
            }

            // _mm_unpacklo_ps
            /// <summary> Unpack and interleave single-precision (32-bit) floating-point elements from the low half of "a" and "b", and store the results in "dst".  </summary>
            [DebuggerStepThrough]
            public static m128 unpacklo_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Float0 = a.Float0;
                dst.Float1 = b.Float0;
                dst.Float2 = a.Float1;
                dst.Float3 = b.Float1;
                return dst;
            }

            // _mm_movehl_ps
            /// <summary> Move the upper 2 single-precision (32-bit) floating-point elements from "b" to the lower 2 elements of "dst", and copy the upper 2 elements from "a" to the upper 2 elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 movehl_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Float0 = b.Float2;
                dst.Float1 = b.Float3;
                dst.Float2 = a.Float2;
                dst.Float3 = a.Float3;
                return dst;
            }

            // _mm_movelh_ps
            /// <summary> Move the lower 2 single-precision (32-bit) floating-point elements from "b" to the upper 2 elements of "dst", and copy the lower 2 elements from "a" to the lower 2 elements of "dst". </summary>
            [DebuggerStepThrough]
            public static m128 movelh_ps(m128 a, m128 b)
            {
                m128 dst = default(m128);
                dst.Float0 = a.Float0;
                dst.Float1 = a.Float1;
                dst.Float2 = b.Float0;
                dst.Float3 = b.Float1;
                return dst;
            }

            // _mm_movemask_ps
            /// <summary> Set each bit of mask "dst" based on the most significant bit of the corresponding packed single-precision (32-bit) floating-point element in "a". </summary>
            [DebuggerStepThrough]
            public static int movemask_ps(m128 a)
            {
                int dst = 0;
                if ((a.UInt0 & 0x80000000) != 0) dst |= 1;
                if ((a.UInt1 & 0x80000000) != 0) dst |= 2;
                if ((a.UInt2 & 0x80000000) != 0) dst |= 4;
                if ((a.UInt3 & 0x80000000) != 0) dst |= 8;
                return dst;
            }
        }
    }
}

#endif
