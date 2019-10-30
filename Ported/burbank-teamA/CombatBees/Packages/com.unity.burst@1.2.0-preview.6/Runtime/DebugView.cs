using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity.Burst.Intrinsics
{
    internal unsafe class M128DebugView
    {
        m128 m_Value;

        public M128DebugView(m128 value)
        {
            m_Value = value;
        }
#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe byte[] Byte
        {
            get
            {
                return new byte[]
                {
                    m_Value.Byte0, m_Value.Byte1, m_Value.Byte2, m_Value.Byte3,
                    m_Value.Byte4, m_Value.Byte5, m_Value.Byte6, m_Value.Byte7,
                    m_Value.Byte8, m_Value.Byte9, m_Value.Byte10, m_Value.Byte11,
                    m_Value.Byte12, m_Value.Byte13, m_Value.Byte14, m_Value.Byte15,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe sbyte[] SByte
        {
            get
            {
                return new sbyte[]
                {
                    m_Value.SByte0, m_Value.SByte1, m_Value.SByte2, m_Value.SByte3,
                    m_Value.SByte4, m_Value.SByte5, m_Value.SByte6, m_Value.SByte7,
                    m_Value.SByte8, m_Value.SByte9, m_Value.SByte10, m_Value.SByte11,
                    m_Value.SByte12, m_Value.SByte13, m_Value.SByte14, m_Value.SByte15,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe ushort[] UShort
        {
            get
            {
                return new ushort[]
                {
                    m_Value.UShort0, m_Value.UShort1, m_Value.UShort2, m_Value.UShort3,
                    m_Value.UShort4, m_Value.UShort5, m_Value.UShort6, m_Value.UShort7,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe short[] SShort
        {
            get
            {
                return new short[]
                {
                    m_Value.SShort0, m_Value.SShort1, m_Value.SShort2, m_Value.SShort3,
                    m_Value.SShort4, m_Value.SShort5, m_Value.SShort6, m_Value.SShort7,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe uint[] UInt
        {
            get
            {
                return new uint[]
                {
                    m_Value.UInt0, m_Value.UInt1, m_Value.UInt2, m_Value.UInt3,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe int[] SInt
        {
            get
            {
                return new int[]
                {
                    m_Value.SInt0, m_Value.SInt1, m_Value.SInt2, m_Value.SInt3,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe float[] Float
        {
            get
            {
                return new float[]
                {
                    m_Value.Float0, m_Value.Float1, m_Value.Float2, m_Value.Float3,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe long[] SLong
        {
            get
            {
                return new long[]
                {
                    m_Value.SLong0, m_Value.SLong1,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe ulong[] ULong
        {
            get
            {
                return new ulong[]
                {
                    m_Value.ULong0, m_Value.ULong1,
                };
            }
        }

#if !UNITY_DOTSPLAYER
        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
#endif
        public unsafe double[] Double
        {
            get
            {
                return new double[]
                {
                    m_Value.Double0, m_Value.Double1,
                };
            }
        }
    }
}