using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using UnityBenchShared;

namespace Burst.Compiler.IL.Tests
{
    internal class Aliasing
    {
        public unsafe struct NoAliasField
        {
            [NoAlias]
            public int* ptr1;

            public int* ptr2;

            public class Provider : IArgumentProvider
            {
                public object Value => new NoAliasField { ptr1 = null, ptr2 = null };
            }
        }

        public unsafe struct ContainerOfManyNoAliasFields
        {
            public NoAliasField s0;

            public NoAliasField s1;

            [NoAlias]
            public NoAliasField s2;

            public class Provider : IArgumentProvider
            {
                public object Value => new ContainerOfManyNoAliasFields { s0 = new NoAliasField { ptr1 = null, ptr2 = null }, s1 = new NoAliasField { ptr1 = null, ptr2 = null }, s2 = new NoAliasField { ptr1 = null, ptr2 = null } };
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Union
        {
            [FieldOffset(0)]
            public ulong a;

            [FieldOffset(1)]
            public int b;

            [FieldOffset(5)]
            public float c;

            public class Provider : IArgumentProvider
            {
                public object Value => new Union { a = 4242424242424242, b = 13131313, c = 42.0f };
            }
        }

#if UNITY_2020_1_OR_NEWER || UNITY_BURST_EXPERIMENTAL_FEATURE_ALIASING
        [TestCompiler(typeof(NoAliasField.Provider))]
        public unsafe static void CheckNoAliasFieldWithItself(ref NoAliasField s)
        {
            // Check that they correctly alias with themselves.
            Unity.Burst.Aliasing.ExpectAlias(s.ptr1, s.ptr1);
            Unity.Burst.Aliasing.ExpectAlias(s.ptr2, s.ptr2);
        }

        [TestCompiler(typeof(NoAliasField.Provider), ExpectCompilerException = true)]
        public unsafe static void CheckNoAliasFieldWithItselfBadPtr1(ref NoAliasField s)
        {
            Unity.Burst.Aliasing.ExpectNoAlias(s.ptr1, s.ptr1);
        }

        [TestCompiler(typeof(NoAliasField.Provider), ExpectCompilerException = true)]
        public unsafe static void CheckNoAliasFieldWithItselfBadPtr2(ref NoAliasField s)
        {
            Unity.Burst.Aliasing.ExpectNoAlias(s.ptr2, s.ptr2);
        }

        [TestCompiler(typeof(NoAliasField.Provider))]
        public unsafe static void CheckNoAliasFieldWithAnotherPointer(ref NoAliasField s)
        {
            // Check that they do not alias each other because of the [NoAlias] on the ptr1 field above.
            Unity.Burst.Aliasing.ExpectNoAlias(s.ptr1, s.ptr2);
        }

        [TestCompiler(typeof(NoAliasField.Provider))]
        public unsafe static void CheckNoAliasFieldWithNull(ref NoAliasField s)
        {
            // Check that comparing a pointer with null is no alias.
            Unity.Burst.Aliasing.ExpectNoAlias(s.ptr1, null);
        }

        [TestCompiler(typeof(NoAliasField.Provider))]
        public unsafe static void CheckAliasFieldWithNull(ref NoAliasField s)
        {
            // Check that comparing a pointer with null is no alias.
            Unity.Burst.Aliasing.ExpectNoAlias(s.ptr2, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private unsafe static void NoAliasInfoSubFunctionAlias(int* a, int* b)
        {
            Unity.Burst.Aliasing.ExpectAlias(a, b);
        }

        [TestCompiler(typeof(NoAliasField.Provider))]
        public unsafe static void CheckNoAliasFieldSubFunctionAlias(ref NoAliasField s)
        {
            NoAliasInfoSubFunctionAlias(s.ptr1, s.ptr1);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private unsafe static void NoAliasInfoSubFunctionNoAlias(int* a, int* b)
        {
            Unity.Burst.Aliasing.ExpectNoAlias(a, b);
        }

        [TestCompiler(typeof(NoAliasField.Provider), ExpectCompilerException = true)]
        public unsafe static void CheckNoAliasFieldSubFunctionNoAlias(ref NoAliasField s)
        {
            NoAliasInfoSubFunctionNoAlias(s.ptr1, s.ptr1);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private unsafe static void AliasInfoSubFunctionNoAlias([NoAlias] int* a, int* b)
        {
            Unity.Burst.Aliasing.ExpectNoAlias(a, b);
        }

        [TestCompiler(typeof(NoAliasField.Provider))]
        public unsafe static void CheckNoAliasFieldSubFunctionWithNoAliasParameter(ref NoAliasField s)
        {
            AliasInfoSubFunctionNoAlias(s.ptr1, s.ptr1);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private unsafe static void AliasInfoSubFunctionTwoSameTypedStructs(ref NoAliasField s0, ref NoAliasField s1)
        {
            // Check that they do not alias within their own structs.
            Unity.Burst.Aliasing.ExpectNoAlias(s0.ptr1, s0.ptr2);
            Unity.Burst.Aliasing.ExpectNoAlias(s1.ptr1, s1.ptr2);

            // But that they do alias across structs.
            Unity.Burst.Aliasing.ExpectAlias(s0.ptr1, s1.ptr1);
            Unity.Burst.Aliasing.ExpectAlias(s0.ptr1, s1.ptr2);
            Unity.Burst.Aliasing.ExpectAlias(s0.ptr2, s1.ptr1);
            Unity.Burst.Aliasing.ExpectAlias(s0.ptr2, s1.ptr2);

        }

        [TestCompiler(typeof(NoAliasField.Provider), typeof(NoAliasField.Provider))]
        public unsafe static void CheckNoAliasFieldAcrossTwoSameTypedStructs(ref NoAliasField s0, ref NoAliasField s1)
        {
            AliasInfoSubFunctionTwoSameTypedStructs(ref s0, ref s1);
        }

        [TestCompiler(4, 13)]
        public static void CheckNoAliasRefs([NoAlias] ref int a, ref int b)
        {
            Unity.Burst.Aliasing.ExpectAlias(ref a, ref a);
            Unity.Burst.Aliasing.ExpectAlias(ref b, ref b);
            Unity.Burst.Aliasing.ExpectNoAlias(ref a, ref b);
        }

        [TestCompiler(4, 13.53f)]
        public static void CheckNoAliasRefsAcrossTypes([NoAlias] ref int a, ref float b)
        {
            Unity.Burst.Aliasing.ExpectNoAlias(ref a, ref b);
        }

        [TestCompiler(typeof(Union.Provider))]
        public static void CheckNoAliasRefsInUnion(ref Union u)
        {
            Unity.Burst.Aliasing.ExpectAlias(ref u.a, ref u.b);
            Unity.Burst.Aliasing.ExpectAlias(ref u.a, ref u.c);
            Unity.Burst.Aliasing.ExpectNoAlias(ref u.b, ref u.c);
        }

        [TestCompiler(typeof(ContainerOfManyNoAliasFields.Provider))]
        public unsafe static void CheckNoAliasOfSubStructs(ref ContainerOfManyNoAliasFields s)
        {
            // Since s2 has [NoAlias], s0 and s1 do not alias with it.
            Unity.Burst.Aliasing.ExpectNoAlias(ref s.s0, ref s.s2);
            Unity.Burst.Aliasing.ExpectNoAlias(ref s.s1, ref s.s2);

            // Since ptr1 has [NoAlias], it does not alias with ptr2 within the same struct instance.
            Unity.Burst.Aliasing.ExpectNoAlias(s.s0.ptr1, s.s0.ptr2);
            Unity.Burst.Aliasing.ExpectNoAlias(s.s1.ptr1, s.s1.ptr2);
            Unity.Burst.Aliasing.ExpectNoAlias(s.s2.ptr1, s.s2.ptr2);

            // Across s0 and s1 their pointers can alias each other though.
            Unity.Burst.Aliasing.ExpectAlias(s.s0.ptr1, s.s1.ptr1);
            Unity.Burst.Aliasing.ExpectAlias(s.s0.ptr1, s.s1.ptr2);
            Unity.Burst.Aliasing.ExpectAlias(s.s0.ptr2, s.s1.ptr1);
            Unity.Burst.Aliasing.ExpectAlias(s.s0.ptr2, s.s1.ptr2);

            // But s2 cannot alias with s0 or s1's pointers.
            Unity.Burst.Aliasing.ExpectNoAlias(s.s2.ptr1, s.s0.ptr1);
            Unity.Burst.Aliasing.ExpectNoAlias(s.s2.ptr1, s.s0.ptr2);
            Unity.Burst.Aliasing.ExpectNoAlias(s.s2.ptr2, s.s1.ptr1);
            Unity.Burst.Aliasing.ExpectNoAlias(s.s2.ptr2, s.s1.ptr2);
        }
#endif
    }
}
