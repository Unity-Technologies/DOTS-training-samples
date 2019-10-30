using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Burst.Compiler.IL.Tests.Helpers;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityBenchShared;

namespace Burst.Compiler.IL.Tests
{
    internal partial class TestStructs
    {
        [TestCompiler]
        public static float test_struct_func_call_by_value()
        {
            var localVar = new CustomStruct();
            localVar.firstfield = 94;
            localVar.value = 123;
            return byvalue_function_helper(localVar);
        }

        [TestCompiler]
        public static float test_struct_func_call_by_ref()
        {
            var localVar = new CustomStruct
            {
                firstfield = 94,
                value = 123
            };
            byref_function_helper(ref localVar);
            return localVar.value;
        }

        [TestCompiler]
        public static float test_struct_func_call_instance()
        {
            var localVar = new CustomStruct2 { value = 123 };
            return localVar.returnDoubleValue();
        }

        [TestCompiler]
        public static float test_struct_constructor_nondefault()
        {
            var localVar = new CustomStruct2(123.0f);
            return localVar.value;
        }

        [TestCompiler]
        public static float test_struct_constructor_default()
        {
            var localVar = new CustomStruct2();
            localVar.value = 1;
            return localVar.value;
        }

        [TestCompiler]
        public static float test_struct_copysemantic()
        {
            var a = new CustomStruct2 { value = 123.0f };
            var b = a;
            b.value = 345;
            return b.value;
        }

        [TestCompiler]
        public static float test_struct_nested()
        {
            var a = new TestNestedStruct { v1 = { x = 5 } };
            return a.v1.x;
        }

        [TestCompiler(1.0f)]
        public static float test_struct_multiple_fields(float x)
        {
            var v = new TestVector4
            {
                x = 1.0f,
                y = 2.0f,
                z = 3.0f,
                w = 4.0f
            };
            return x + v.x + v.y + v.z + v.w;
        }

        [TestCompiler]
        public static float test_struct_multi_assign()
        {
            var a = new MultiAssignStruct(2.0F);
            return a.x + a.y + a.z;
        }

        [TestCompiler]
        public static int test_custom_struct_return_simple()
        {
            var a = return_value_helper_simple(1, 2);
            return a.firstfield + a.value;
        }

        [TestCompiler]
        public static int test_custom_struct_return_constructor()
        {
            var a = return_value_helper_constructor(1, 2);
            return a.firstfield + a.value;
        }

        [TestCompiler]
        public static int test_struct_self_reference()
        {
            var a = new SelfReferenceStruct
            {
                Value = 1
            };
            return a.Value;
        }

        [TestCompiler]
        public static int test_struct_deep()
        {
            var deep = new DeepStruct2();
            deep.value.value.SetValue(10);
            return deep.value.value.GetValue() + deep.value.value.value;
        }

        [TestCompiler(2)]
        public static int test_struct_empty(int x)
        {
            var emptyStruct = new EmptyStruct();
            var result = emptyStruct.Increment(x);
            return result;
        }

        [TestCompiler]
        public static float test_struct_with_static_fields()
        {
            StructWithStaticVariables myStruct = new StructWithStaticVariables();
            myStruct.myFloat = 5;
            myStruct = copy_struct_with_static_by_value(myStruct);
            mutate_struct_with_static_by_ref_value(ref myStruct);
            return myStruct.myFloat;
        }

        [TestCompiler(true)]
        [TestCompiler(false)]
        public static bool TestStructWithBoolAsInt(bool value)
        {
            var structWithBoolAsInt = new StructWithBoolAsInt(value);
            return structWithBoolAsInt;
        }

        [TestCompiler]
        [Ignore("IL Instruction LEAVE not yet supported")]
        public static int TestStructDisposable()
        {
            using (var structDisposable = new StructDisposable())
            {
                return structDisposable.x + 1;
            }
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_InstructionStsfldNotSupported)]
        public static void TestStructWithStaticFieldWrite()
        {
            var test = new StructWithStaticField();
            test.CheckWrite();
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_LoadingFromNonReadonlyStaticFieldNotSupported)]
        public static void TestStructWithStaticFieldRead()
        {
            var test = new StructWithStaticField();
            test.CheckRead();
        }

        [TestCompiler]
        public static int TestExplicitLayoutSize()
        {
            return UnsafeUtility.SizeOf<Color>();
        }

        [TestCompiler]
        public static int TestExplicitLayoutStruct()
        {
            var color = new Color() { Value = 0xAABBCCDD };
            var a = color.Value + GetColorR(ref color) + GetColorG(color) + color.GetColorB() + color.A;
            var pair = new NumberPair()
            {
                SignedA = -13,
                UnsignedB = 37
            };
            var b = pair.SignedA - ((int) pair.UnsignedA) + pair.SignedB - ((int) pair.UnsignedB);
            return ((int)a) + b;
        }

        static uint GetColorR(ref Color color)
        {
            return color.R;
        }

        static uint GetColorG(Color color)
        {
            return color.G;
        }

        [TestCompiler()]
        public static uint TestExplicitLayoutWrite()
        {
            var color = new Color() { Value = 0xAABBCCDD };
            color.G = 3;
            ColorWriteBByRef(ref color, 7);
            return color.Value;
        }

        static void ColorWriteBByRef(ref Color color, byte v)
        {
            color.B = v;
        }

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        private unsafe struct CheckHoleInner
        {
            [FieldOffset(0)]
            public byte* m_Ptr;
        }

        private struct CheckHoleOuter
        {
            public CheckHoleInner a;
            public int b;
            public CheckHoleInner c;
        }

        [TestCompiler]
        public static unsafe int TestCheckHoleSize()
        {
            return UnsafeUtility.SizeOf<CheckHoleOuter>();
        }

        [TestCompiler]
        public static unsafe int TestCheckHoleFieldOffsetA()
        {
            var value = new CheckHoleOuter();
            var addressStart = &value;
            var addressField = &value.a;
            return (int)((byte*)addressField - (byte*)addressStart);
        }

        [TestCompiler]
        public static unsafe int TestCheckHoleFieldOffsetB()
        {
            var value = new CheckHoleOuter();
            var addressStart = &value;
            var addressField = &value.b;
            return (int)((byte*)addressField - (byte*)addressStart);
        }

        [TestCompiler]
        public static unsafe int TestCheckHoleFieldOffsetC()
        {
            var value = new CheckHoleOuter();
            var addressStart = &value;
            var addressField = &value.c;
            return (int)((byte*)addressField - (byte*)addressStart);
        }

        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct ExplicitLayoutStructUnaligned
        {
            [FieldOffset(0)] public int a;
            [FieldOffset(4)] public sbyte b;
            [FieldOffset(5)] public int c;
            [FieldOffset(9)] public fixed int d[4];
        }

        [TestCompiler]
        public static unsafe int TestExplicitLayoutStructUnaligned()
        {
            var value = new ExplicitLayoutStructUnaligned
            {
                a = -2,
                b = -5,
                c = 9
            };

            value.d[0] = 1;
            value.d[1] = 2;
            value.d[2] = 3;
            value.d[3] = 4;

            return value.a + value.b + value.c + value.d[0] + value.d[1] + value.d[2] + value.d[3];
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct ExplicitLayoutStructFixedBuffer
        {
            [FieldOffset(0)]
            public int First;
            [FieldOffset(4)]
            public fixed int Data[128];

            public struct Provider : IArgumentProvider
            {
                public object Value => new ExplicitLayoutStructFixedBuffer(3);
            }

            public ExplicitLayoutStructFixedBuffer(int x)
            {
                First = x;
                fixed (int* dataPtr = Data)
                {
                    dataPtr[8] = x + 2;
                }
            }
        }

        #if UNITY_ANDROID || UNITY_IOS
        [Ignore("This test fails on mobile platforms")]
        #endif
        [TestCompiler(typeof(ExplicitLayoutStructFixedBuffer.Provider))]
        public static unsafe int TestExplicitLayoutStructFixedBuffer(ref ExplicitLayoutStructFixedBuffer x)
        {
            return x.First + x.Data[8];
        }

        [StructLayout(LayoutKind.Explicit, Size = 9)]
        public struct ExplicitStructWithSize
        {
            [FieldOffset(0)] public int a;
            [FieldOffset(4)] public sbyte b;
            [FieldOffset(5)] public int c;
        }

        [TestCompiler]
        [Ignore("UnsafeUtility not working properly with explicit sizing")]
        public static unsafe int TestStructSizingExplicitStructWithSize()
        {
            return UnsafeUtility.SizeOf<ExplicitStructWithSize>();
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct ExplicitStructWithoutSize
        {
            [FieldOffset(0)] public int a;
            [FieldOffset(4)] public sbyte b;
            [FieldOffset(5)] public int c;
        }

        [TestCompiler]
        public static unsafe int TestStructSizingExplicitStructWithoutSize()
        {
            return UnsafeUtility.SizeOf<ExplicitStructWithoutSize>();
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct ExplicitStructWithoutSize2
        {
            [FieldOffset(0)] public long a;
            [FieldOffset(8)] public sbyte b;
            [FieldOffset(9)] public int c;
        }

        [TestCompiler]
        public static unsafe int TestStructSizingExplicitStructWithoutSize2()
        {
            return UnsafeUtility.SizeOf<ExplicitStructWithoutSize2>();
        }

        [StructLayout(LayoutKind.Sequential, Size = 9)]
        public struct SequentialStructWithSize
        {
            public int a;
            public int b;
            public sbyte c;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructSizeNotSupported)]
        public static unsafe int TestStructSizingSequentialStructWithSize()
        {
            return UnsafeUtility.SizeOf<SequentialStructWithSize>();
        }

        [StructLayout(LayoutKind.Sequential, Size = 13)]
        public struct SequentialStructWithSize2
        {
            public int a;
            public int b;
            public sbyte c;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructSizeNotSupported)]
        public static unsafe int TestStructSizingSequentialStructWithSize2()
        {
            return UnsafeUtility.SizeOf<SequentialStructWithSize2>();
        }

        [StructLayout(LayoutKind.Sequential, Size = 12)]
        public struct SequentialStructWithSize3
        {
            public int a;
            public int b;
            public sbyte c;
        }

        [TestCompiler]
        public static unsafe int TestStructSizingSequentialStructWithSize3()
        {
            return UnsafeUtility.SizeOf<SequentialStructWithSize3>();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SequentialStructWithoutSize
        {
            public int a;
            public int b;
            public sbyte c;
        }

        [TestCompiler]
        public static unsafe int TestStructSizingSequentialStructWithoutSize()
        {
            return UnsafeUtility.SizeOf<SequentialStructWithoutSize>();
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct ExplicitStructEmpty { }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructZeroSizeNotSupported)]
        public static unsafe int TestStructSizingExplicitStructEmpty()
        {
            return UnsafeUtility.SizeOf<ExplicitStructEmpty>();
        }

        public struct ExplicitStructEmptyContainer
        {
            public ExplicitStructEmpty A;
            public int B;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructZeroSizeNotSupported)]
        public static unsafe int TestEmptyStructEmbeddedInStruct()
        {
            return UnsafeUtility.SizeOf<ExplicitStructEmptyContainer>();
        }

        [StructLayout(LayoutKind.Explicit, Size = 0)]
        public struct ExplicitStructEmptyWithSize { }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructZeroSizeNotSupported)]
        public static unsafe int TestStructSizingExplicitStructEmptyWithSize()
        {
            return UnsafeUtility.SizeOf<ExplicitStructEmptyWithSize>();
        }

        public struct SequentialStructEmptyNoAttributes { }

        [TestCompiler]
        public static unsafe int TestStructSizingSequentialStructEmptyNoAttributes()
        {
            return UnsafeUtility.SizeOf<SequentialStructEmptyNoAttributes>();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SequentialStructEmpty { }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructZeroSizeNotSupported)]
        public static unsafe int TestStructSizingSequentialStructEmpty()
        {
            return UnsafeUtility.SizeOf<SequentialStructEmpty>();
        }

        [StructLayout(LayoutKind.Sequential, Size = 0)]
        public struct SequentialStructEmptyWithSize { }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructZeroSizeNotSupported)]
        public static unsafe int TestStructSizingSequentialStructEmptyWithSize()
        {
            return UnsafeUtility.SizeOf<SequentialStructEmptyWithSize>();
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct SequentialStructEmptyWithNonZeroSize { }

        [TestCompiler]
        public static unsafe int TestStructSizingSequentialStructEmptyWithNonZeroSize()
        {
            return UnsafeUtility.SizeOf<SequentialStructEmptyWithNonZeroSize>();
        }

        [StructLayout(LayoutKind.Auto)]
        public struct AutoStruct
        {
            public int a;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructWithAutoLayoutNotSupported)]
        public static unsafe int TestAutoStruct()
        {
            return UnsafeUtility.SizeOf<AutoStruct>();
        }

        [TestCompiler]
        public static int TestNestedExplicitLayouts()
        {
            var nested = new NestedExplicit0()
            {
                Next = new NestedExplicit1()
                {
                    Next = new NestedExplicit2()
                    {
                        FValue = 13.37f
                    }
                }
            };
            var a = nested.NextAsInt + nested.Next.NextAsInt + nested.Next.Next.IValue;
            nested.Next.Next.FValue = 0.0042f;
            var b = nested.NextAsInt + nested.Next.NextAsInt + nested.Next.Next.IValue;
            return a + b;
        }

        [TestCompiler]
        public static int TestNestedExplicitLayoutsSize()
        {

            return UnsafeUtility.SizeOf<NestedExplicit0>();
        }

        [TestCompiler]
        public static uint TestBitcast()
        {
            return new FloatRepr()
            {
                Value = 13.37f
            }.AsUint;
        }

        [TestCompiler]
        public static uint TestExplicitStructFromCall()
        {
            return ReturnStruct().Value + ReturnStruct().R;
        }

        static Color ReturnStruct()
        {
            return new Color()
            {
                R = 10,
                G = 20,
                B = 30,
                A = 255
            };
        }

        [TestCompiler]
        public static unsafe uint TestExplicitLayoutStructWithFixedArray()
        {
            var x = new FixedArrayExplitLayoutStruct()
            {
                UpperUInt = 0xAABBCCDD,
                LowerUInt = 0xEEFF3344
            };

            uint sum = 0;
            for (int i = 0; i < 8; i++)
            {
                sum += x.Bytes[i];
                if (i < 4) sum += x.Shorts[i];
            }

            return x.UpperUInt + x.LowerUInt + sum;
        }

        [TestCompiler]
        public static unsafe int TestExplicitLayoutStructWithFixedArraySize()
        {
            return UnsafeUtility.SizeOf<FixedArrayExplitLayoutStruct>();
        }

        public struct StructInvalid
        {
            public string WowThatStringIsNotSupported;
        }

        //private struct StructInvalidProvider : IArgumentProvider
        //{
        //    public object[] Arguments => new object[] { new StructInvalid() };
        //}

        private static CustomStruct return_value_helper_simple(int a, int b)
        {
            CustomStruct val;
            val.firstfield = a;
            val.value = b;
            return val;
        }

        private static CustomStruct return_value_helper_constructor(int a, int b)
        {
            return new CustomStruct(a, b);
        }

        private static float byvalue_function_helper(CustomStruct customStruct)
        {
            return customStruct.value * 2;
        }

        private static void byref_function_helper(ref CustomStruct customStruct)
        {
            customStruct.value = customStruct.value * 2;
        }

        static StructWithStaticVariables copy_struct_with_static_by_value(StructWithStaticVariables byValue)
        {
            byValue.myFloat += 2;
            return byValue;
        }

        static void mutate_struct_with_static_by_ref_value(ref StructWithStaticVariables byValue)
        {
            byValue.myFloat += 2;
        }


        private struct EmptyStruct
        {
            public int Increment(int x)
            {
                return x + 1;
            }
        }

        private struct CustomStruct
        {
            public int firstfield;
            public int value;

            public CustomStruct(int a, int b)
            {
                firstfield = a;
                value = b;
            }
        }

        struct DeepStruct2
        {
#pragma warning disable 0649
            public DeepStruct1 value;
#pragma warning restore 0649
        }

        struct DeepStruct1
        {
#pragma warning disable 0649
            public DeepStruct0 value;
#pragma warning restore 0649
        }

        struct DeepStruct0
        {
            public int value;

            public void SetValue(int value)
            {
                this.value = value;
            }

            public int GetValue()
            {
                return value;
            }
        }

        private struct CustomStruct2
        {
            public float value;

            public float returnDoubleValue()
            {
                return value;
            }

            public CustomStruct2(float initialValue)
            {
                value = initialValue;
            }
        }

        private struct TestVector4
        {
            public float x;
            public float y;
            public float z;
            public float w;
        }

        private struct StructWithBoolAsInt
        {
            private int _value;

            public StructWithBoolAsInt(bool value)
            {
                _value = value ? 1 : 0;
            }

            public static implicit operator bool(StructWithBoolAsInt val)
            {
                return val._value != 0;
            }
        }

        private struct TestNestedStruct
        {
            public TestVector4 v1;
        }

        private struct MultiAssignStruct
        {
            public float x;
            public float y;
            public float z;

            public MultiAssignStruct(float val)
            {
                x = y = z = val;
            }
        }

        private struct SelfReferenceStruct
        {
#pragma warning disable 0649
            public int Value;
            public unsafe SelfReferenceStruct* Left;
            public unsafe SelfReferenceStruct* Right;
#pragma warning restore 0649
        }

        private struct StructForSizeOf
        {
#pragma warning disable 0649

            public IntPtr Value1;

            public Float4 Vec1;

            public IntPtr Value2;

            public Float4 Vec2;
#pragma warning disable 0649
        }

        private struct StructWithStaticField
        {
            public static int MyField;

            public void CheckWrite()
            {
                MyField = 0;
            }

            public int CheckRead()
            {
                return MyField;
            }
        }

        private struct Float4
        {
#pragma warning disable 0649
            public float x;
            public float y;
            public float z;
            public float w;
#pragma warning restore 0649
        }

        private struct StructWithStaticVariables
        {
#pragma warning disable 0414
#pragma warning disable 0649
            const float static_const_float = 9;
            static string static_string = "hello";

            public float myFloat;
            public Float4 myFloat4;

            static float static_float_2 = 5;
#pragma warning restore 0649
#pragma warning restore 0414
        }

        struct StructDisposable : IDisposable
        {
            public int x;


            public void Dispose()
            {
                x++;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct Color
        {
            [FieldOffset(0)] public uint Value;
            [FieldOffset(0)] public byte R;
            [FieldOffset(1)] public byte G;
            [FieldOffset(2)] public byte B;
            [FieldOffset(3)] public byte A;

            public byte GetColorB()
            {
                return B;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NumberPair
        {
            [FieldOffset(0)] public uint UnsignedA;
            [FieldOffset(0)] public int SignedA;
            [FieldOffset(4)] public uint UnsignedB;
            [FieldOffset(4)] public int SignedB;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NestedExplicit0
        {
            [FieldOffset(0)] public NestedExplicit1 Next;
            [FieldOffset(0)] public int NextAsInt;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NestedExplicit1
        {
            [FieldOffset(0)] public NestedExplicit2 Next;
            [FieldOffset(0)] public int NextAsInt;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NestedExplicit2
        {
            [FieldOffset(0)] public float FValue;
            [FieldOffset(0)] public int IValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatRepr
        {
            [FieldOffset(0)] public float Value;
            [FieldOffset(0)] public uint AsUint;
        }

        [StructLayout(LayoutKind.Explicit, Size =  24)]
        private struct PaddedStruct
        {
            [FieldOffset(8)] public int Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct FixedArrayExplitLayoutStruct
        {
            [FieldOffset(0)] public fixed byte Bytes[8];
            [FieldOffset(0)] public fixed ushort Shorts[4];
            [FieldOffset(0)] public uint UpperUInt;
            [FieldOffset(4)] public uint LowerUInt;
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct Chunk
        {
            [FieldOffset(0)] public Chunk* Archetype;
            [FieldOffset(8)] public Chunk* metaChunkEntity;

            [FieldOffset(16)] public int Count;
        }

        [TestCompiler]
        public static unsafe int TestRegressionInvalidGetElementPtrStructLayout()
        {
            Chunk* c = stackalloc Chunk[1];
            c[0].Archetype = null;
            c[0].metaChunkEntity = null;
            c[0].Count = 0;

            return TestRegressionInvalidGetElementPtrStructLayoutInternal(0, 1, &c);
        }

        public static unsafe int TestRegressionInvalidGetElementPtrStructLayoutInternal(int index, int limit, Chunk** currentChunk)
        {
            int rValue = 0;
            while (index >= limit + 1)
            {
                rValue += (*currentChunk)->Count;
                index += 1;
            }

            return rValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct Packet
        {
            [FieldOffset(0)] public int data;
            [FieldOffset(0)] public fixed byte moreData[1500];
        }

        [TestCompiler]
        public static unsafe int TestExplicitSizeReporting()
        {
            return sizeof(Packet);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ExplicitStructPackedButWithHoles
        {
            [FieldOffset(0)]
            public byte A;

            [FieldOffset(1)]
            public long B;

            [FieldOffset(21)]
            public byte C;
        }

        [TestCompiler]
        public static int TestExplicitStructPackedButWithHolesSize()
        {
            return UnsafeUtility.SizeOf<ExplicitStructPackedButWithHoles>();
        }

        [TestCompiler]
        public static unsafe int TestExplicitStructPackedButWithHolesOffsetC()
        {
            var value = new ExplicitStructPackedButWithHoles();
            var addressStart = &value;
            var addressField = &value.C;
            return (int)((byte*)addressField - (byte*)addressStart);
        }

        private struct ExplicitStructPackedButWithHolesContainer
        {
            public ExplicitStructPackedButWithHoles A;
            public int B;
            public ExplicitStructPackedButWithHoles C;
        }

        [TestCompiler]
        public static int TestExplicitStructPackedButWithHolesContainerSize()
        {
            return UnsafeUtility.SizeOf<ExplicitStructPackedButWithHolesContainer>();
        }

        [TestCompiler]
        public static unsafe int TestExplicitStructPackedButWithHolesContainerOffsetC()
        {
            var value = new ExplicitStructPackedButWithHolesContainer();
            var addressStart = &value;
            var addressField = &value.C;
            return (int)((byte*)addressField - (byte*)addressStart);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ExplicitStructNotPackedWithHoles
        {
            [FieldOffset(4)]
            public int A;

            [FieldOffset(12)]
            public int B;
        }

        [TestCompiler]
        public static int TestExplicitStructNotPackedWithHolesSize()
        {
            return UnsafeUtility.SizeOf<ExplicitStructNotPackedWithHoles>();
        }

        [TestCompiler]
        public static float TestExplicitStructNested()
        {
            StructWithNestUnion b;
            b.Value.Min = 5.0f;

            return b.Value.Min;
        }

        [TestCompiler]
        public static float TestExplicitStructNestedAsArgument()
        {
            float Helper(StructWithNestUnion outer)
            {
                return outer.Value.Min;
            }

            return Helper(new StructWithNestUnion
            {
                Value = new UnionValue { Min = 5.0f }
            });
        }

        public struct StructWithNestUnion
        {
            public UnionValue Value;
        }

        [StructLayout(LayoutKind.Explicit)]

        public struct UnionValue
        {
            [FieldOffset(0)]
            public float Min;
            [FieldOffset(4)]
            public float Max;

            [FieldOffset(0)]
            public uint Property;
        }

        #if UNITY_ANDROID || UNITY_IOS
        [Ignore("This test fails on mobile platforms")]
        #endif
        [TestCompiler(typeof(NetworkEndPoint.Provider), typeof(NetworkEndPoint.Provider), ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_StructByValueNotSupported)]
        public static bool TestABITransformIntoExplicitLayoutTransform(NetworkEndPoint a, NetworkEndPoint b)
        {
            return a.Compare(b);
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct NetworkEndPoint
        {
            internal const int ArrayLength = 2;
            [FieldOffset(0)]
            internal fixed byte data[ArrayLength];
            [FieldOffset(0)]
            internal ushort family;
            [FieldOffset(28)]
            internal int length;

            public bool Compare(NetworkEndPoint other)
            {
                if (length != other.length)
                    return false;

                return true;
            }
            public class Provider : IArgumentProvider
            {
                public object Value => default(NetworkEndPoint);

            }
        }

        public struct SequentialStructWithPaddingAndVectorField
        {
            public byte a;
            public float2 b;

            public class Provider : IArgumentProvider
            {
                public object Value => new SequentialStructWithPaddingAndVectorField { a = 1, b = new float2(4, 5) };
            }
        }

        #if UNITY_ANDROID || UNITY_IOS
        [Ignore("This test fails on mobile platforms")]
        #endif
        [TestCompiler(typeof(SequentialStructWithPaddingAndVectorField.Provider))]
        public static int TestSequentialStructWithPaddingAndVectorField(ref SequentialStructWithPaddingAndVectorField value)
        {
            return (int)value.b.x;
        }

        private static void TestSequentialStructWithPaddingAndVectorFieldRefHelper(ref SequentialStructWithPaddingAndVectorField value)
        {
            value.b.yx = value.b;
            value.b = value.b.yx;
        }

        #if UNITY_ANDROID || UNITY_IOS
        [Ignore("This test fails on mobile platforms")]
        #endif
        [TestCompiler(typeof(SequentialStructWithPaddingAndVectorField.Provider))]
        public static int TestSequentialStructWithPaddingAndVectorFieldRef(ref SequentialStructWithPaddingAndVectorField value)
        {
            TestSequentialStructWithPaddingAndVectorFieldRefHelper(ref value);
            return (int)value.b.x;
        }

        [TestCompiler]
        public static unsafe int TestSequentialStructWithPaddingAndVectorFieldPtr()
        {
            var vec = new float2(1, 2);
            var vecPtr = &vec;
            var value = new SequentialStructWithPaddingAndVectorField();
            value.b = *vecPtr;
            return (int)value.b.x;
        }

        [TestCompiler]
        public static unsafe int TestCreatingVectorTypeFromNonVectorScalarType()
        {
            var x = (short)4;
            var value = new int4(x, x, x, x);
            return value.w;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct ExplicitVectors
        {
            [FieldOffset(0)]
            public int A;
            [FieldOffset(4)]
            public int2 B;      //NB: Any Vector type is sufficient
        }

        [TestCompiler]
        public static unsafe int TestVectorLoadFromExplicitStruct()
        {
            var header = new ExplicitVectors{ };

            return header.B.x;
        }

        [TestCompiler(DataRange.Standard)]
        public static unsafe int TestVectorStoreToExplicitStruct(ref int2 a)
        {
            var header = new ExplicitVectors{ B=a};

            return header.B.x;
        }

        [TestCompiler(typeof(StructWithNonBlittableTypes), ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_TypeNotBlittableForFunctionPointer)]
        public static unsafe int TestStructWithNonBlittableTypes(ref StructWithNonBlittableTypes a)
        {
            var checksum = 0;
            checksum = (checksum * 397) ^ a.a0;
            checksum = (checksum * 397) ^ a.b0;
            checksum = (checksum * 397) ^ a.b1;
            checksum = (checksum * 397) ^ (a.d0 ? 10 : 0);
            checksum = (checksum * 397) ^ a.a1;
            checksum = (checksum * 397) ^ a.b1;
            checksum = (checksum * 397) ^ a.c1;
            checksum = (checksum * 397) ^ (a.d1 ? 0 : 7);
            checksum = (checksum * 397) ^ a.Check;
            return checksum;
        }

        [TestCompiler(typeof(StructWithNonBlittableTypesWithMarshalAs))]
        public static unsafe int TestStructWithBlittableTypesWithMarshalAs(ref StructWithNonBlittableTypesWithMarshalAs a)
        {
            var checksum = 0;
            checksum = (checksum * 397) ^ a.a0;
            checksum = (checksum * 397) ^ a.b0;
            checksum = (checksum * 397) ^ a.b1;
            checksum = (checksum * 397) ^ (a.d0 ? 10 : 0);
            checksum = (checksum * 397) ^ a.a1;
            checksum = (checksum * 397) ^ a.b1;
            checksum = (checksum * 397) ^ a.c1;
            checksum = (checksum * 397) ^ (a.d1 ? 0 : 7);
            checksum = (checksum * 397) ^ a.Check;
            return checksum;
        }

        [TestCompiler]
        public static int TestSizeOfStructWithBlittableTypesWithMarshalAs()
        {
            return UnsafeUtility.SizeOf<StructWithNonBlittableTypesWithMarshalAs>();
        }

#if BURST_TESTS_ONLY
        [TestCompiler(typeof(StructWithNonBlittableTypes), ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_TypeNotBlittableForFunctionPointer)]
        public static int TestStructWithNonBlittableTypesOffset(ref StructWithNonBlittableTypes a)
        {
            return Unsafe.ByteOffset(ref a.a0, ref a.a1).ToInt32();
        }
#endif

        [TestCompiler(typeof(StructWithBlittableTypes))]
        public static unsafe int TestStructWithBlittableTypes(ref StructWithBlittableTypes a)
        {
            var checksum = 0;
            checksum = (checksum * 397) ^ a.a;
            checksum = (checksum * 397) ^ a.b;
            checksum = (checksum * 397) ^ a.c;
            checksum = (checksum * 397) ^ a.d.x;
            checksum = (checksum * 397) ^ a.d.y;
            return checksum;
        }

        [TestCompiler]
        public static int TestStructWithPointerDependency()
        {
            var test = new StructWithPointerDependency();
            return test.DirectNoDependency.Value;
        }

        public struct StructWithBlittableTypes : IArgumentProvider
        {
            public StructWithBlittableTypes(int a, int b, int c, int2 d)
            {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
            }

            public int a;
            public int b;
            public int c;
            public int2 d;

            public object Value => new StructWithBlittableTypes(1, 2, 3, new int2(4, 5));
        }

        public struct StructWithNonBlittableTypes : IArgumentProvider
        {
            public StructWithNonBlittableTypes(byte a0, byte b0, byte c0, bool d0, byte a1, byte b1, byte c1, bool d1, int check)
            {
                this.a0 = a0;
                this.b0 = b0;
                this.c0 = c0;
                this.d0 = d0;
                this.a1 = a1;
                this.b1 = b1;
                this.c1 = c1;
                this.d1 = d1;
                this.Check = check;
            }

            public byte a0;
            public byte b0;
            public byte c0;
            public bool d0;

            public byte a1;
            public byte b1;
            public byte c1;
            public bool d1;

            public int Check;


            public object Value => new StructWithNonBlittableTypes(1, 2, 3, true, 5, 6, 7, false, 0x12345678);
        }

        public struct StructWithNonBlittableTypesWithMarshalAs : IArgumentProvider
        {
            public StructWithNonBlittableTypesWithMarshalAs(byte a0, byte b0, byte c0, bool d0, byte a1, byte b1, byte c1, bool d1, int check)
            {
                this.a0 = a0;
                this.b0 = b0;
                this.c0 = c0;
                this.d0 = d0;
                this.a1 = a1;
                this.b1 = b1;
                this.c1 = c1;
                this.d1 = d1;
                this.Check = check;
            }

            public byte a0;
            public byte b0;
            public byte c0;
            [MarshalAs(UnmanagedType.U1)]
            public bool d0;

            public byte a1;
            public byte b1;
            public byte c1;
            [MarshalAs(UnmanagedType.U1)]
            public bool d1;

            public int Check;


            public object Value => new StructWithNonBlittableTypesWithMarshalAs(1, 2, 3, true, 5, 6, 7, false, 0x12345678);
        }

        public unsafe struct StructWithPointerDependency
        {
            public StructWithNoDependency* PointerToNoDependency;

            public StructWithNoDependency DirectNoDependency;
        }

        public struct StructWithNoDependency
        {
            public int Value;
        }
    }
}
