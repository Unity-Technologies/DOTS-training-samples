using NUnit.Framework;
using Unity.Burst;
using Unity.Mathematics;

namespace Burst.Compiler.IL.Tests
{
    internal class TestConstArrays
    {
        [TestCompiler()]
        public static int ReadFromIntArray()
        {
            return StructWithConstArray1.IntValues[1];
        }

        [TestCompiler()]
        public static int ReadFromColorArray()
        {
            var color = StructWithConstArrayWithStruct1.Colors[1];
            return ((color.R * 255) + color.G) * 255 + color.B;
        }

        [TestCompiler()]
        public static int ReadFromColorArray2()
        {
            var color = StaticArrayStruct.Colors[1];
            return ((color.R * 255) + color.G) * 255 + color.B;
        }

        struct StructWithConstArray1
        {
            public static readonly int[] IntValues = new int[4] {1, 2, 3, 4};
        }

        struct StructWithConstArrayWithStruct1
        {
            public static readonly Color[] Colors = { new Color(), new Color(1, 2, 3, 255) };
        }

        private struct Color
        {
            public Color(byte r, byte g, byte b, byte a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }

            public byte R, G, B, A;
        }
        
        private struct StaticArrayStruct
        {
            public static readonly double[] Doubles = { 3, 6, 9, 42, 43 };
            public static readonly byte[] Bytes = {1, 2, 3};
            public static readonly ushort[] UShorts = {2, 6, 8, 2, 0};
            public static readonly int[] Ints = {-6, 6, 50};
            public static readonly int[] ZeroData = {0, 0, 0, 0};
            public static readonly int[] ZeroLength = {};
            public static readonly Color[] ZeroLengthStruct = {};
            public static readonly Color[] Colors = { new Color(), new Color(1, 2, 3, 255) };
            public static readonly int3[] Positions = {new int3(0, 0, 1), new int3(0, 1, 0), new int3(1, 0, 0)};
        }

        [TestCompiler()]
        public static int TestStaticReadonlyArrayLength()
        {
            return StaticArrayStruct.Doubles.Length + StaticArrayStruct.Bytes.Length +
                   StaticArrayStruct.UShorts.Length + StaticArrayStruct.Ints.Length +
                   StaticArrayStruct.ZeroData.Length + StaticArrayStruct.ZeroLength.Length +
                   StaticArrayStruct.ZeroLengthStruct.Length + StaticArrayStruct.Colors.Length +
                   StaticArrayStruct.Positions.Length;
        }

        private struct StructP
        {
            public static readonly int[] Value = new int[One()];

            public static int One()
            {
                return 1;
            }
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_InstructionLdlenNonConstantLengthNotSupported)]
        public static int TestStaticReadonlyArrayNonConstantLength()
        {
            return StructP.Value.Length;
        }

        private struct StructQ
        {
            public static readonly int[] Value = new int[10];

            public static int One()
            {
                return 1;
            }

            static StructQ()
            {
                Value[One()] = 1;
            }
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_AccessingManagedArrayNotSupported)]
        public static int TestStaticReadonlyArrayWithNonConstantStelemIndex()
        {
            return StructQ.Value[1];
        }

        private struct StructR
        {
#pragma warning disable 0649
            public static int[] Value;
#pragma warning restore 0649

            static StructR()
            {
                Value[0] = 1;
            }
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_LoadingFromNonReadonlyStaticFieldNotSupported)]
        public static int TestStaticReadonlyArrayExplicitConstructionOfUninitialized()
        {
            return StructR.Value.Length;
        }
        
        private struct StructS
        {
            public static readonly int[] Value = new int[10];

            static StructS()
            {
                Value[0] = 1;
                Value[1] = 2;
                Value[2] = 8;
                Value[3] = 2;
                Value[4] = 0;
                Value[5] = 2;
                Value[6] = 1;
                Value[7] = 2;
                Value[8] = 2;
                Value[9] = 3;
            }
        }

        // Right now we don't support initialization of readonly arrays outside the array constructor
        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_AccessingManagedArrayNotSupported)]
        public static int TestStaticReadonlyArrayExplicitConstruction()
        {
            int sum = 0;
            for (int i = 0; i < 10; i++) sum += StructS.Value[i];
            return sum;
        }

        [TestCompiler()]
        public static int TestStaticReadonlyArrayLdelem()
        {
            var doubles = StaticArrayStruct.Doubles[0];
            for (int i = 1; i < StaticArrayStruct.Doubles.Length; i++) doubles += StaticArrayStruct.Doubles[i];

            var bytes = StaticArrayStruct.Bytes[0];
            for (int i = 1; i < StaticArrayStruct.Bytes.Length; i++) bytes += StaticArrayStruct.Bytes[i];

            var ushorts = StaticArrayStruct.UShorts[0];
            for (int i = 1; i < StaticArrayStruct.UShorts.Length; i++) ushorts += StaticArrayStruct.UShorts[i];

            var ints = StaticArrayStruct.Ints[0];
            for (int i = 1; i < StaticArrayStruct.Ints.Length; i++) ints += StaticArrayStruct.Ints[i];

            ints += StaticArrayStruct.ZeroData[0];
            for (int i = 1; i < StaticArrayStruct.ZeroData.Length; i++) ints += StaticArrayStruct.ZeroData[i];

            for (int i = 0; i < StaticArrayStruct.ZeroLength.Length; i++) doubles += StaticArrayStruct.ZeroLength[i];

            bytes = (byte)(StaticArrayStruct.Colors[0].R + StaticArrayStruct.Colors[0].G
                                                         + StaticArrayStruct.Colors[0].B
                                                         + StaticArrayStruct.Colors[0].A);
            
            for (int i = 1; i < StaticArrayStruct.Colors.Length; i++)
                bytes += (byte)(StaticArrayStruct.Colors[i].R + StaticArrayStruct.Colors[i].G
                                                              + StaticArrayStruct.Colors[i].B
                                                              + StaticArrayStruct.Colors[i].A);

            for (int i = 1; i < StaticArrayStruct.Positions.Length; i++)
                ints += math.dot(StaticArrayStruct.Positions[i-1], StaticArrayStruct.Positions[i]);

            return (int)doubles + bytes + ushorts + ints;
        }

        private static T TakesRef<T>(ref T x)
        {
            return x;
        }

        [TestCompiler()]
        public static int TestStaticReadonlyArrayWithElementRef()
        {
            return TakesRef(ref StaticArrayStruct.Ints[1]);
        }
        
        [TestCompiler()]
        public static int TestStaticReadonlyArrayWithElementVectorRef()
        {
            var x = TakesRef(ref StaticArrayStruct.Positions[1]);
            return math.dot(x, x);
        }

        [TestCompiler(1)]
        [TestCompiler(2)]
        [TestCompiler(3)]
        [TestCompiler(4)]
        public static int TestStaticReadonlyArrayWithDynamicLdelem(int count)
        {
            int sum = 0;

            for (int i = 0; i < count; i++)
            {
                sum += (int)StaticArrayStruct.Doubles[i];
            }

            return sum;
        }
        
        [TestCompiler(ExpectedException = typeof(System.IndexOutOfRangeException))]
        [MonoOnly(".NET CLR does not support burst.abort correctly")]
        public static int TestStaticReadonlyLdelemConstantIndexOutOfBounds()
        {
            return StaticArrayStruct.Ints[100];
        }

        public struct ContainerStruct
        {
            public SmallStruct A;
            public SmallStruct B;

            public static readonly ContainerStruct[] CoolStructs =
            {
                new ContainerStruct
                {
                    A = new SmallStruct { a = 3, b = 5 },
                    B = new SmallStruct { a = 9, b = 10 }
                },
                new ContainerStruct
                {
                    A = new SmallStruct { a = 1, b = 5 },
                    B = new SmallStruct { a = 7, b = 8 }
                }
            };
        }

        [TestCompiler()]
        public static int TestStaticReadonlyArrayOfStructOfStructs()
        {
            return ContainerStruct.CoolStructs[0].A.a + ContainerStruct.CoolStructs[0].A.b +
                   ContainerStruct.CoolStructs[0].B.a + ContainerStruct.CoolStructs[0].B.b +
                   ContainerStruct.CoolStructs[1].A.a + ContainerStruct.CoolStructs[1].A.b +
                   ContainerStruct.CoolStructs[1].B.a + ContainerStruct.CoolStructs[1].B.b;
        }

        /* There's currently no way of settings the safety checks on from here
        [TestCompiler(0xFFFFFFF, ExpectedException = typeof(IndexOutOfRangeException))]
        public static int TestStaticReadonlyLdelemDynamicIndexOfBounds(int x)
        {
            return StaticArrayStruct.Ints[x];
        }
        */

        public struct SmallStruct
        {
            public int a;
            public int b;
        }
    }
}

