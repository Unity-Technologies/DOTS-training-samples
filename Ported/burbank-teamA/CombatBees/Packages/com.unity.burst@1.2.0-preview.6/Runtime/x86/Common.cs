using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#if BURST_INTERNAL || UNITY_BURST_EXPERIMENTAL_X86_INTRINSICS

namespace Unity.Burst.Intrinsics
{
    public unsafe static partial class X86
    {
        // Helper to check pointer for 16-byte alignment
        private static void CheckPointerAlignment16(void* mem_addr)
        {
            if ((((ulong)(IntPtr)mem_addr) & 0xf) != 0)
                throw new InvalidOperationException("mem_addr must be aligned on a 16-byte boundary");
        }

        private static m128 GenericCSharpLoad(void* ptr)
        {
            return *(m128*)ptr;
        }

        private static void GenericCSharpStore(void* ptr, m128 val)
        {
            *(m128*)ptr = val;
        }

        private static sbyte Saturate_To_Int8(int val)
        {
            if (val > sbyte.MaxValue)
                return sbyte.MaxValue;
            else if (val < sbyte.MinValue)
                return sbyte.MinValue;
            return (sbyte)val;
        }

        private static byte Saturate_To_UnsignedInt8(int val)
        {
            if (val > byte.MaxValue)
                return byte.MaxValue;
            else if (val < byte.MinValue)
                return byte.MinValue;
            return (byte)val;
        }

        private static short Saturate_To_Int16(int val)
        {
            if (val > short.MaxValue)
                return short.MaxValue;
            else if (val < short.MinValue)
                return short.MinValue;
            return (short)val;
        }

        private static ushort Saturate_To_UnsignedInt16(int val)
        {
            if (val > ushort.MaxValue)
                return ushort.MaxValue;
            else if (val < ushort.MinValue)
                return ushort.MinValue;
            return (ushort)val;
        }

        private static bool IsNaN(uint v)
        {
            return (v & 0x7fffffffu) > 0x7f800000;
        }

        private static bool IsNaN(ulong v)
        {
            return (v & 0x7ffffffffffffffful) > 0x7ff0000000000000ul;
        }

        // Wrapper for C# reference mode to handle FROUND_xxx
        private static double RoundDImpl(double d, int roundingMode)
        {
            switch (roundingMode & 7)
            {
                case 0: return Math.Round(d);
                case 1: return Math.Floor(d);
                case 2: return Math.Ceiling(d);
                case 3: return Math.Truncate(d);
                case 4:
                    switch (MXCSR & MXCSRBits.RoundingControlMask)
                    {
                        case MXCSRBits.RoundToNearest: return Math.Round(d);
                        case MXCSRBits.RoundDown: return Math.Floor(d);
                        case MXCSRBits.RoundUp: return Math.Ceiling(d);
                        case MXCSRBits.RoundTowardZero: return Math.Truncate(d);
                    }
                    break;
            }
            return 0.0;
        }
    }
}

#endif
