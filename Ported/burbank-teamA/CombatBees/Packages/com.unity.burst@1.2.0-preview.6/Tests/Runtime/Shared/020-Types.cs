using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using NUnit.Framework;
using Unity.Burst;

namespace Burst.Compiler.IL.Tests
{
    /// <summary>
    /// Tests types
    /// </summary>
    internal partial class Types
    {
        [TestCompiler]
        public static int Bool()
        {
            return sizeof(bool);
        }

        [TestCompiler(true)]
        [TestCompiler(false)]
        public static bool BoolArgAndReturn(bool value)
        {
            return !value;
        }

        [TestCompiler]
        public static int Char()
        {
            return sizeof(char);
        }

        [TestCompiler]
        public static int Int8()
        {
            return sizeof(sbyte);
        }

        [TestCompiler]
        public static int Int16()
        {
            return sizeof(short);
        }

        [TestCompiler]
        public static int Int32()
        {
            return sizeof(int);
        }

        [TestCompiler]
        public static int Int64()
        {
            return sizeof(long);
        }

        [TestCompiler]
        public static int UInt8()
        {
            return sizeof(byte);
        }

        [TestCompiler]
        public static int UInt16()
        {
            return sizeof(ushort);
        }

        [TestCompiler]
        public static int UInt32()
        {
            return sizeof(uint);
        }

        [TestCompiler]
        public static int UInt64()
        {
            return sizeof(ulong);
        }

        [TestCompiler]
        public static int EnumSizeOf()
        {
            return sizeof(MyEnum);
        }


        [TestCompiler]
        public static int EnumByteSizeOf()
        {
            return sizeof(MyEnumByte);
        }

        [TestCompiler(MyEnumByte.Tada2)]
        public static MyEnumByte CheckEnumByte(ref MyEnumByte value)
        {
            // Check stloc for enum
            value = MyEnumByte.Tada1;
            return value;
        }

        [TestCompiler(MyEnum.Value15)]
        public static int EnumByParam(MyEnum value)
        {
            return 1 + (int)value;
        }

        [TestCompiler]
        public static float Struct()
        {
            var value = new MyStruct(1,2,3,4);
            return value.x + value.y + value.z + value.w;
        }

        [TestCompiler]
        public static long StructAccess()
        {
            var s = new InterleavedBoolStruct();
            s.b1 = true;
            s.i2 = -1;
            s.b3 = true;
            s.i5 = 3;
            return s.i5;
        }

        [TestCompiler(true)]
        [TestCompiler(false)]
        public static bool StructWithBool(bool value)
        {
            // This method test that storage of boolean between local and struct is working
            // (as they could have different layout)
            var b = new BoolStruct();
            b.b1 = !value;
            return b.b1;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructWithPackNotSupported)]
        public static int CheckStructWithPack()
        {
            return UnsafeUtility.SizeOf<StructWithPack>();
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_InstructionLdstrNotSupported)]
        public static int TestUsingReferenceType()
        {
            return "this is not supported by burst".Length;
        }

        private struct MyStruct
        {
            public MyStruct(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            public float x;
            public float y;
            public float z;
            public float w;
        }

        private struct BoolStruct
        {
#pragma warning disable 0649
            public bool b1;
            public bool b2;
#pragma warning restore 0649
        }

        private unsafe struct BoolFixedStruct
        {
#pragma warning disable 0649
            public fixed bool Values[16];
#pragma warning restore 0649
        }

        private struct InterleavedBoolStruct
        {
#pragma warning disable 0649
            public bool b1;
            public int i2;
            public bool b3;
            public bool b4;
            public long i5;
            public MyEnum e6;
#pragma warning restore 0649
        }

        public enum MyEnum
        {
            Value1 = 1,
            Value15 = 15,
        }


        [StructLayout(LayoutKind.Explicit)]
        private struct ExplicitLayoutStruct
        {
            [FieldOffset(0)]
            public int FieldA;

            [FieldOffset(0)]
            public int FieldB;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        private struct StructWithPack
        {
            public int FieldA;

            public int FieldB;
        }

        [StructLayout(LayoutKind.Sequential, Size = 1024)]
        private struct StructWithSize
        {
            public int FieldA;

            public int FieldB;
        }

        private struct EmptyStruct
        {
        }

        public enum MyEnumByte : byte
        {
            Tada1 = 1,

            Tada2 = 2
        }
    }
}