using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#if BURST_INTERNAL || UNITY_BURST_EXPERIMENTAL_X86_INTRINSICS

namespace Unity.Burst.Intrinsics
{
    public unsafe static partial class X86
    {
        /// <summary>
        /// The 32-bit MXCSR register contains control and status information for SSE, SSE2, and SSE3 SIMD floating-point operations.
        /// </summary>
        [Flags]
        public enum MXCSRBits
        {
            /// <summary>
            /// Bit 15 (FTZ) of the MXCSR register enables the flush-to-zero mode, which controls the masked response to a SIMD floating-point underflow condition.
            /// </summary>
            ///
            /// When the underflow exception is masked and the flush-to-zero mode is enabled, the processor performs the following operations when it detects a floating-point underflow condition.
            /// - Returns a zero result with the sign of the true result
            /// - Sets the precision and underflow exception flags.
            ///
            /// If the underflow exception is not masked, the flush-to-zero bit is ignored.
            ///
            /// The flush-to-zero mode is not compatible with IEEE Standard 754. The IEEE-mandated masked response to under-flow is to deliver the denormalized result.
            /// The flush-to-zero mode is provided primarily for performance reasons. At the cost of a slight precision loss, faster execution can be achieved for applications where underflows
            /// are common and rounding the underflow result to zero can be tolerated. The flush-to-zero bit is cleared upon a power-up or reset of the processor, disabling the flush-to-zero mode.
            FlushToZero = 1 << 15,

            /// <summary>
            /// Mask for rounding control bits.
            /// </summary>
            ///
            /// The rounding modes have no effect on comparison operations, operations that produce exact results, or operations that produce NaN results.
            RoundingControlMask = (1 << 14) | (1 << 13),

            /// <summary>
            /// Rounded result is the closest to the infinitely precise result. If two values are equally close, the result is the even value (that is, the one with the least-significant bit of zero). Default.
            /// </summary>
            RoundToNearest = 0,

            /// <summary>
            /// Rounded result is closest to but no greater than the infinitely precise result.
            /// </summary>
            RoundDown = (1 << 13),

            /// <summary>
            /// Rounded result is closest to but no less than the infinitely precise result.
            /// </summary>
            RoundUp = (1 << 14),

            /// <summary>
            /// Rounded result is closest to but no greater in absolute value than the infinitely precise result.
            /// </summary>
            RoundTowardZero = (1 << 13) | (1 << 14),

            /// <summary>Bits 7 through 12 provide individual mask bits for the SIMD floating-point exceptions. An exception type is masked if the corresponding mask bit is set, and it is unmasked if the bit is clear. These mask bits are set upon a power-up or reset. This causes all SIMD floating-point exceptions to be initially masked.</summary>
            PrecisionMask = 1 << 12,
            /// <summary>Bits 7 through 12 provide individual mask bits for the SIMD floating-point exceptions. An exception type is masked if the corresponding mask bit is set, and it is unmasked if the bit is clear. These mask bits are set upon a power-up or reset. This causes all SIMD floating-point exceptions to be initially masked.</summary>
            UnderflowMask = 1 << 11,
            /// <summary>Bits 7 through 12 provide individual mask bits for the SIMD floating-point exceptions. An exception type is masked if the corresponding mask bit is set, and it is unmasked if the bit is clear. These mask bits are set upon a power-up or reset. This causes all SIMD floating-point exceptions to be initially masked.</summary>
            OverflowMask = 1 << 10,
            /// <summary>Bits 7 through 12 provide individual mask bits for the SIMD floating-point exceptions. An exception type is masked if the corresponding mask bit is set, and it is unmasked if the bit is clear. These mask bits are set upon a power-up or reset. This causes all SIMD floating-point exceptions to be initially masked.</summary>
            DivideByZeroMask = 1 << 9,
            /// <summary>Bits 7 through 12 provide individual mask bits for the SIMD floating-point exceptions. An exception type is masked if the corresponding mask bit is set, and it is unmasked if the bit is clear. These mask bits are set upon a power-up or reset. This causes all SIMD floating-point exceptions to be initially masked.</summary>
            DenormalOperationMask = 1 << 8,
            /// <summary>Bits 7 through 12 provide individual mask bits for the SIMD floating-point exceptions. An exception type is masked if the corresponding mask bit is set, and it is unmasked if the bit is clear. These mask bits are set upon a power-up or reset. This causes all SIMD floating-point exceptions to be initially masked.</summary>
            InvalidOperationMask = 1 << 7,

            /// <summary>
            /// Combine all bits for exception masking into one mask for convenience.
            /// </summary>
            ExceptionMask = PrecisionMask | UnderflowMask | OverflowMask | DivideByZeroMask | DenormalOperationMask | InvalidOperationMask,

            /// <summary>
            /// Bit 6 (DAZ) of the MXCSR register enables the denormals-are-zeros mode, which controls the processor’s response to a SIMD floating-point denormal operand condition.
            /// </summary>
            ///
            /// When the denormals-are-zeros flag is set, the processor converts all denormal source operands to a zero with the sign of the original operand before performing any computations on them.
            /// The processor does not set the denormal-operand exception flag (DE), regardless of the setting of the denormal-operand exception mask bit (DM); and it does not generate a denormal-operand
            /// exception if the exception is unmasked.The denormals-are-zeros mode is not compatible with IEEE Standard 754.
            ///
            /// The denormals-are-zeros mode is provided to improve processor performance for applications such as streaming media processing, where rounding a denormal operand to zero does not
            /// appreciably affect the quality of the processed data. The denormals-are-zeros flag is cleared upon a power-up or reset of the processor, disabling the denormals-are-zeros mode.
            ///
            /// The denormals-are-zeros mode was introduced in the Pentium 4 and Intel Xeon processor with the SSE2 extensions; however, it is fully compatible with the SSE SIMD floating-point instructions
            /// (that is, the denormals-are-zeros flag affects the operation of the SSE SIMD floating-point instructions). In earlier IA-32 processors and in some models of the Pentium 4 processor, this flag
            /// (bit 6) is reserved. Attempting to set bit 6 of the MXCSR register on processors that do not support the DAZ flag will cause a general-protection exception (#GP).
            DenormalsAreZeroes = 1 << 6,

            /// <summary>Bits 0 through 5 of the MXCSR register indicate whether a SIMD floating-point exception has been detected. They are "sticky" flags. That is, after a flag is set, it remains set until explicitly cleared. To clear these flags, use the LDMXCSR or the FXRSTOR instruction to write zeroes to them.</summary>
            PrecisionFlag = 1 << 5,
            /// <summary>Bits 0 through 5 of the MXCSR register indicate whether a SIMD floating-point exception has been detected. They are "sticky" flags. That is, after a flag is set, it remains set until explicitly cleared. To clear these flags, use the LDMXCSR or the FXRSTOR instruction to write zeroes to them.</summary>
            UnderflowFlag = 1 << 4,
            /// <summary>Bits 0 through 5 of the MXCSR register indicate whether a SIMD floating-point exception has been detected. They are "sticky" flags. That is, after a flag is set, it remains set until explicitly cleared. To clear these flags, use the LDMXCSR or the FXRSTOR instruction to write zeroes to them.</summary>
            OverflowFlag = 1 << 3,
            /// <summary>Bits 0 through 5 of the MXCSR register indicate whether a SIMD floating-point exception has been detected. They are "sticky" flags. That is, after a flag is set, it remains set until explicitly cleared. To clear these flags, use the LDMXCSR or the FXRSTOR instruction to write zeroes to them.</summary>
            DivideByZeroFlag = 1 << 2,
            /// <summary>Bits 0 through 5 of the MXCSR register indicate whether a SIMD floating-point exception has been detected. They are "sticky" flags. That is, after a flag is set, it remains set until explicitly cleared. To clear these flags, use the LDMXCSR or the FXRSTOR instruction to write zeroes to them.</summary>
            DenormalFlag = 1 << 1,
            /// <summary>Bits 0 through 5 of the MXCSR register indicate whether a SIMD floating-point exception has been detected. They are "sticky" flags. That is, after a flag is set, it remains set until explicitly cleared. To clear these flags, use the LDMXCSR or the FXRSTOR instruction to write zeroes to them.</summary>
            InvalidOperationFlag = 1 << 0,

            /// <summary>
            /// Combines all bits for flags into one mask for convenience.
            /// </summary>
            FlagMask = PrecisionFlag | UnderflowFlag | OverflowFlag | DivideByZeroFlag | DenormalFlag | InvalidOperationFlag,
        }

        [Flags]
        public enum RoundingMode
        {
            FROUND_TO_NEAREST_INT = 0x00,
            FROUND_TO_NEG_INF = 0x01,
            FROUND_TO_POS_INF = 0x02,
            FROUND_TO_ZERO = 0x03,
            FROUND_CUR_DIRECTION = 0x04,

            FROUND_RAISE_EXC = 0x00,
            FROUND_NO_EXC = 0x08,

            FROUND_NINT = FROUND_TO_NEAREST_INT | FROUND_RAISE_EXC,
            FROUND_FLOOR = FROUND_TO_NEG_INF | FROUND_RAISE_EXC,
            FROUND_CEIL = FROUND_TO_POS_INF | FROUND_RAISE_EXC,
            FROUND_TRUNC = FROUND_TO_ZERO | FROUND_RAISE_EXC,
            FROUND_RINT = FROUND_CUR_DIRECTION | FROUND_RAISE_EXC,
            FROUND_NEARBYINT = FROUND_CUR_DIRECTION | FROUND_NO_EXC,

            FROUND_NINT_NOEXC = FROUND_TO_NEAREST_INT | FROUND_NO_EXC,
            FROUND_FLOOR_NOEXC = FROUND_TO_NEG_INF | FROUND_NO_EXC,
            FROUND_CEIL_NOEXC = FROUND_TO_POS_INF | FROUND_NO_EXC,
            FROUND_TRUNC_NOEXC = FROUND_TO_ZERO | FROUND_NO_EXC,
            FROUND_RINT_NOEXC = FROUND_CUR_DIRECTION | FROUND_NO_EXC,
        }

        /// <summary>
        /// Scoped utility for changing the CSR in a using block with guaranteed restoration on block exit
        /// </summary>
        public struct MXCSRScope : IDisposable
        {
            private MXCSRBits OldBits;

            public MXCSRScope(MXCSRBits csr)
            {
                OldBits = MXCSR;
                MXCSR = csr;
            }

            public void Dispose()
            {
                MXCSR = OldBits;
            }
        }

        public struct RoundingScope : IDisposable
        {
            private MXCSRBits OldBits;

            public RoundingScope(MXCSRBits roundingMode)
            {
                OldBits = MXCSR;
                MXCSR = (OldBits & ~MXCSRBits.RoundingControlMask) | roundingMode;
            }

            public void Dispose()
            {
                MXCSR = OldBits;
            }
        }

#if !BURST_INTERNAL
        static private void BurstIntrinsicSetCSRFromManaged(int bits) { }
        static private int BurstIntrinsicGetCSRFromManaged() { return 0;  }

        [BurstCompile]
        private static void _DoSetCSRTrampoline(int bits) { BurstIntrinsicSetCSRFromManaged(bits); }

        [BurstCompile]
        private static int _DoGetCSRTrampoline() { return BurstIntrinsicGetCSRFromManaged(); }

        internal delegate void SetCSRDelegate(int bits);
        internal delegate int GetCSRDelegate();

        private static GetCSRDelegate ManagedGetCSRTrampoline = BurstCompiler.CompileDelegate<GetCSRDelegate>(_DoGetCSRTrampoline);
        private static SetCSRDelegate ManagedSetCSRTrampoline = BurstCompiler.CompileDelegate<SetCSRDelegate>(_DoSetCSRTrampoline);

        internal static int getcsr_raw()
        {
            return ManagedGetCSRTrampoline();
        }

        internal static void setcsr_raw(int bits)
        {
            ManagedSetCSRTrampoline(bits);
        }
#else
        [DllImport("burst-dllimport-native", EntryPoint = "x86_getcsr")]
        internal static extern int getcsr_raw();

        [DllImport("burst-dllimport-native", EntryPoint = "x86_setcsr")]
        internal static extern void setcsr_raw(int bits);
#endif
        public static MXCSRBits MXCSR
        {
            get
            {
                return (MXCSRBits)getcsr_raw();
            }
            set
            {
                setcsr_raw((int)value);
            }
        }
    }
}

#endif
