using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity.Burst.Intrinsics
{
    [StructLayout(LayoutKind.Explicit)]
	[DebuggerTypeProxy(typeof(M128DebugView))]
    public struct m128
    {
        [FieldOffset(0)] public byte Byte0;
        [FieldOffset(1)] public byte Byte1;
        [FieldOffset(2)] public byte Byte2;
        [FieldOffset(3)] public byte Byte3;
        [FieldOffset(4)] public byte Byte4;
        [FieldOffset(5)] public byte Byte5;
        [FieldOffset(6)] public byte Byte6;
        [FieldOffset(7)] public byte Byte7;
        [FieldOffset(8)] public byte Byte8;
        [FieldOffset(9)] public byte Byte9;
        [FieldOffset(10)] public byte Byte10;
        [FieldOffset(11)] public byte Byte11;
        [FieldOffset(12)] public byte Byte12;
        [FieldOffset(13)] public byte Byte13;
        [FieldOffset(14)] public byte Byte14;
        [FieldOffset(15)] public byte Byte15;

        [FieldOffset(0)] public sbyte SByte0;
        [FieldOffset(1)] public sbyte SByte1;
        [FieldOffset(2)] public sbyte SByte2;
        [FieldOffset(3)] public sbyte SByte3;
        [FieldOffset(4)] public sbyte SByte4;
        [FieldOffset(5)] public sbyte SByte5;
        [FieldOffset(6)] public sbyte SByte6;
        [FieldOffset(7)] public sbyte SByte7;
        [FieldOffset(8)] public sbyte SByte8;
        [FieldOffset(9)] public sbyte SByte9;
        [FieldOffset(10)] public sbyte SByte10;
        [FieldOffset(11)] public sbyte SByte11;
        [FieldOffset(12)] public sbyte SByte12;
        [FieldOffset(13)] public sbyte SByte13;
        [FieldOffset(14)] public sbyte SByte14;
        [FieldOffset(15)] public sbyte SByte15;

        [FieldOffset(0)] public ushort UShort0;
        [FieldOffset(2)] public ushort UShort1;
        [FieldOffset(4)] public ushort UShort2;
        [FieldOffset(6)] public ushort UShort3;
        [FieldOffset(8)] public ushort UShort4;
        [FieldOffset(10)] public ushort UShort5;
        [FieldOffset(12)] public ushort UShort6;
        [FieldOffset(14)] public ushort UShort7;

        [FieldOffset(0)] public short SShort0;
        [FieldOffset(2)] public short SShort1;
        [FieldOffset(4)] public short SShort2;
        [FieldOffset(6)] public short SShort3;
        [FieldOffset(8)] public short SShort4;
        [FieldOffset(10)] public short SShort5;
        [FieldOffset(12)] public short SShort6;
        [FieldOffset(14)] public short SShort7;

        [FieldOffset(0)] public uint UInt0;
        [FieldOffset(4)] public uint UInt1;
        [FieldOffset(8)] public uint UInt2;
        [FieldOffset(12)] public uint UInt3;

        [FieldOffset(0)] public int SInt0;
        [FieldOffset(4)] public int SInt1;
        [FieldOffset(8)] public int SInt2;
        [FieldOffset(12)] public int SInt3;

        [FieldOffset(0)] public ulong ULong0;
        [FieldOffset(8)] public ulong ULong1;

        [FieldOffset(0)] public long SLong0;
        [FieldOffset(8)] public long SLong1;

        [FieldOffset(0)] public float Float0;
        [FieldOffset(4)] public float Float1;
        [FieldOffset(8)] public float Float2;
        [FieldOffset(12)] public float Float3;

        [FieldOffset(0)] public double Double0;
        [FieldOffset(8)] public double Double1;

        public m128(byte b)
        {
            this = default(m128);
            Byte0 = Byte1 = Byte2 = Byte3 = Byte4 = Byte5 = Byte6 = Byte7 = Byte8 = Byte9 = Byte10 = Byte11 = Byte12 = Byte13 = Byte14 = Byte15 = b;
        }

        public m128(
            byte a, byte b, byte c, byte d,
            byte e, byte f, byte g, byte h,
            byte i, byte j, byte k, byte l,
            byte m, byte n, byte o, byte p)
        {
            this = default(m128);
            Byte0 = a;
            Byte1 = b;
            Byte2 = c;
            Byte3 = d;
            Byte4 = e;
            Byte5 = f;
            Byte6 = g;
            Byte7 = h;
            Byte8 = i;
            Byte9 = j;
            Byte10 = k;
            Byte11 = l;
            Byte12 = m;
            Byte13 = n;
            Byte14 = o;
            Byte15 = p;
        }

        public m128(sbyte b)
        {
            this = default(m128);
            SByte0 = SByte1 = SByte2 = SByte3 = SByte4 = SByte5 = SByte6 = SByte7 = SByte8 = SByte9 = SByte10 = SByte11 = SByte12 = SByte13 = SByte14 = SByte15 = b;
        }

        public m128(
            sbyte a, sbyte b, sbyte c, sbyte d,
            sbyte e, sbyte f, sbyte g, sbyte h,
            sbyte i, sbyte j, sbyte k, sbyte l,
            sbyte m, sbyte n, sbyte o, sbyte p)
        {
            this = default(m128);
            SByte0 = a;
            SByte1 = b;
            SByte2 = c;
            SByte3 = d;
            SByte4 = e;
            SByte5 = f;
            SByte6 = g;
            SByte7 = h;
            SByte8 = i;
            SByte9 = j;
            SByte10 = k;
            SByte11 = l;
            SByte12 = m;
            SByte13 = n;
            SByte14 = o;
            SByte15 = p;
        }

        public m128(short v)
        {
            this = default(m128);
            SShort0 = SShort1 = SShort2 = SShort3 = v;
        }

        public m128(short a, short b, short c, short d, short e, short f, short g, short h)
        {
            this = default(m128);
            SShort0 = a;
            SShort1 = b;
            SShort2 = c;
            SShort3 = d;
            SShort4 = e;
            SShort5 = f;
            SShort6 = g;
            SShort7 = h;
        }

        public m128(ushort v)
        {
            this = default(m128);
            UShort0 = UShort1 = UShort2 = UShort3 = v;
        }

        public m128(ushort a, ushort b, ushort c, ushort d, ushort e, ushort f, ushort g, ushort h)
        {
            this = default(m128);
            UShort0 = a;
            UShort1 = b;
            UShort2 = c;
            UShort3 = d;
            UShort4 = e;
            UShort5 = f;
            UShort6 = g;
            UShort7 = h;
        }

        public m128(int v)
        {
            this = default(m128);
            SInt0 = SInt1 = SInt2 = SInt3 = v;
        }

        public m128(int a, int b, int c, int d)
        {
            this = default(m128);
            SInt0 = a;
            SInt1 = b;
            SInt2 = c;
            SInt3 = d;
        }

        public m128(uint v)
        {
            this = default(m128);
            UInt0 = UInt1 = UInt2 = UInt3 = v;
        }

        public m128(uint a, uint b, uint c, uint d)
        {
            this = default(m128);
            UInt0 = a;
            UInt1 = b;
            UInt2 = c;
            UInt3 = d;
        }

        public m128(float f)
        {
            this = default(m128);
            Float0 = Float1 = Float2 = Float3 = f;
        }

        public m128(float a, float b, float c, float d)
        {
            this = default(m128);
            Float0 = a;
            Float1 = b;
            Float2 = c;
            Float3 = d;
        }

        public m128(double f)
        {
            this = default(m128);
            Double0 = Double1 = f;
        }

        public m128(double a, double b)
        {
            this = default(m128);
            Double0 = a;
            Double1 = b;
        }

        public m128(long f)
        {
            this = default(m128);
            SLong0 = SLong1 = f;
        }

        public m128(long a, long b)
        {
            this = default(m128);
            SLong0 = a;
            SLong1 = b;
        }

        public m128(ulong f)
        {
            this = default(m128);
            ULong0 = ULong1 = f;
        }

        public m128(ulong a, ulong b)
        {
            this = default(m128);
            ULong0 = a;
            ULong1 = b;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct m256
    {
        [FieldOffset(0)] public fixed byte Byte[32];
        [FieldOffset(0)] public fixed sbyte SByte[32];
        [FieldOffset(0)] public fixed ushort UShort[16];
        [FieldOffset(0)] public fixed short SShort[16];
        [FieldOffset(0)] public fixed uint UInt[8];
        [FieldOffset(0)] public fixed int SInt[8];
        [FieldOffset(0)] public fixed ulong ULong[4];
        [FieldOffset(0)] public fixed long SLong[4];
        [FieldOffset(0)] public fixed float Float[8];
    }
}