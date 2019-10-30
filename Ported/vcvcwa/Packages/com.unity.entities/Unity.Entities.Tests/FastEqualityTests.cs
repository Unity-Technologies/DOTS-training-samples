#if !UNITY_DOTSPLAYER
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;
using System;
using System.Collections.Generic;

namespace Unity.Entities.Tests
{
    public class FastEqualityTests
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Simple
        {
            int a;
            int b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SimpleEmbedded
        {
            float4 a;
            int b;
        }

        [StructLayout(LayoutKind.Sequential)]

        struct BytePadding
        {
            byte a;
            byte b;
            float c;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct AlignSplit
        {
            float3 a;
            double b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct EndPadding
        {
            double a;
            float b;

            public EndPadding(double a, float b)
            {
                this.a = a;
                this.b = b;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        unsafe struct FloatPointer
        {
            float* a;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ClassInStruct : IEquatable<ClassInStruct>
        {
            string blah;

            public bool Equals(ClassInStruct other)
            {
                return blah == other.blah;
            }

            public override int GetHashCode()
            {
                if (blah != null)
                    return blah.GetHashCode();
                else
                    return 0;
            }
        }

        [StructLayout((LayoutKind.Sequential))]
        unsafe struct FixedArrayStruct
        {
            public fixed int array[3];
        }

        enum Nephew
        {
            Huey,
            Dewey,
            Louie
        }

        [StructLayout((LayoutKind.Sequential))]
        struct EnumStruct
        {
            public Nephew nephew;
        }

        int PtrAligned4Count = UnsafeUtility.SizeOf<FloatPointer>() / 4;

        [Test]
        public void SimpleLayout()
        {
            var res = FastEquality.CreateTypeInfo(typeof(Simple)).Layouts;
            Assert.AreEqual(1, res.Length);
            Assert.AreEqual(new FastEquality.Layout {offset = 0, count = 2, Aligned4 = true }, res[0]);
        }

        [Test]
        public void PtrLayout()
        {
            var layout = FastEquality.CreateTypeInfo(typeof(FloatPointer)).Layouts;
            Assert.AreEqual(1, layout.Length);
            Assert.AreEqual(new FastEquality.Layout {offset = 0, count = PtrAligned4Count, Aligned4 = true }, layout[0]);
        }

        [Test]
        public void ClassLayout()
        {
            var ti = FastEquality.CreateTypeInfo(typeof(ClassInStruct));
            Assert.AreEqual(null, ti.Layouts);
            Assert.IsNotNull(ti.GetHashFn);
            Assert.IsNotNull(ti.EqualFn);
        }

        [Test]
        public void SimpleEmbeddedLayout()
        {
            var layout = FastEquality.CreateTypeInfo(typeof(SimpleEmbedded)).Layouts;
            Assert.AreEqual(1, layout.Length);
            Assert.AreEqual(new FastEquality.Layout {offset = 0, count = 5, Aligned4 = true }, layout[0]);
        }

        [Test]
        public void EndPaddingLayout()
        {
            var layout = FastEquality.CreateTypeInfo(typeof(EndPadding)).Layouts;
            Assert.AreEqual(1, layout.Length);
            Assert.AreEqual(new FastEquality.Layout {offset = 0, count = 3, Aligned4 = true }, layout[0]);
        }

        [Test]
        public void AlignSplitLayout()
        {
            var layout = FastEquality.CreateTypeInfo(typeof(AlignSplit)).Layouts;
            Assert.AreEqual(2, layout.Length);

            Assert.AreEqual(new FastEquality.Layout {offset = 0, count = 3, Aligned4 = true }, layout[0]);
            Assert.AreEqual(new FastEquality.Layout {offset = 16, count = 2, Aligned4 = true }, layout[1]);
        }

        [Test]
        public void BytePaddding()
        {
            var layout = FastEquality.CreateTypeInfo(typeof(BytePadding)).Layouts;
            Assert.AreEqual(2, layout.Length);

            Assert.AreEqual(new FastEquality.Layout {offset = 0, count = 2, Aligned4 = false }, layout[0]);
            Assert.AreEqual(new FastEquality.Layout {offset = 4, count = 1, Aligned4 = true }, layout[1]);
        }

        [Test]
        public void EqualsInt4()
        {
            var typeInfo = FastEquality.CreateTypeInfo(typeof(int4));

            Assert.IsTrue(FastEquality.Equals(new int4(1, 2, 3, 4), new int4(1, 2, 3, 4), typeInfo));
            Assert.IsFalse(FastEquality.Equals(new int4(1, 2, 3, 4), new int4(1, 2, 3, 5), typeInfo));
            Assert.IsFalse(FastEquality.Equals(new int4(1, 2, 3, 4), new int4(0, 2, 3, 4), typeInfo));
            Assert.IsFalse(FastEquality.Equals(new int4(1, 2, 3, 4), new int4(5, 6, 7, 8), typeInfo));
        }

        [Test]
        public void EqualsPadding()
        {
            var typeInfo = FastEquality.CreateTypeInfo(typeof(EndPadding));

            Assert.IsTrue(FastEquality.Equals(new EndPadding(1, 2), new EndPadding(1, 2), typeInfo));
            Assert.IsFalse(FastEquality.Equals(new EndPadding(1, 2), new EndPadding(1, 3), typeInfo));
            Assert.IsFalse(FastEquality.Equals(new EndPadding(1, 2), new EndPadding(4, 2), typeInfo));
        }

        [Test]
        public unsafe void EqualsFixedArray()
        {
            var typeInfo = FastEquality.CreateTypeInfo(typeof(FixedArrayStruct));
            Assert.AreEqual(1, typeInfo.Layouts.Length);
            Assert.AreEqual(3, typeInfo.Layouts[0].count);

            var a = new FixedArrayStruct();
            a.array[0] = 123;
            a.array[1] = 234;
            a.array[2] = 345;

            var b = a;

            Assert.IsTrue(FastEquality.Equals(a, b, typeInfo));

            b.array[1] = 456;

            Assert.IsFalse(FastEquality.Equals(a, b, typeInfo));
        }

        [Test]
        public void EqualsEnum()
        {
            var typeInfo = FastEquality.CreateTypeInfo(typeof(EnumStruct));

            var a = new EnumStruct { nephew = Nephew.Huey };
            var b = new EnumStruct { nephew = Nephew.Dewey };

            Assert.IsTrue(FastEquality.Equals(a, a, typeInfo));
            Assert.IsFalse(FastEquality.Equals(a, b, typeInfo));
        }

        [Test]
        public void TypeHash()
        {
            int[] hashes =
            {
                FastEquality.CreateTypeInfo(typeof(Simple)).Hash,
                FastEquality.CreateTypeInfo(typeof(SimpleEmbedded)).Hash,
                FastEquality.CreateTypeInfo(typeof(BytePadding)).Hash,
                FastEquality.CreateTypeInfo(typeof(AlignSplit)).Hash,
                FastEquality.CreateTypeInfo(typeof(EndPadding)).Hash,
                FastEquality.CreateTypeInfo(typeof(FloatPointer)).Hash,
                FastEquality.CreateTypeInfo(typeof(ClassInStruct)).Hash,
                FastEquality.CreateTypeInfo(typeof(FixedArrayStruct)).Hash,
                FastEquality.CreateTypeInfo(typeof(EnumStruct)).Hash
            };

            Assert.AreEqual(hashes.Distinct().Count(), hashes.Length);
        }

        [DisableAutoTypeRegistration]
        struct TypeWithoutHashCodeOverride : ISharedComponentData, IEquatable<TypeWithoutHashCodeOverride>
        {
#pragma warning disable 649
            public string Foo;
#pragma warning restore 649

            public bool Equals(TypeWithoutHashCodeOverride other)
            {
                return Foo == other.Foo;
            }
        }

        [Test]
        public void ForgettingGetHashCodeIsAnError()
        {
            var ex = Assert.Throws<ArgumentException>(() => FastEquality.CreateTypeInfo(typeof(TypeWithoutHashCodeOverride)));
            Assert.IsTrue(ex.Message.Contains("GetHashCode"));
        }

        struct DoubleEquals : ISharedComponentData, IEquatable<DoubleEquals>
        {
            public bool Equals(DoubleEquals other)
            {
                return true;
            }

            public override bool Equals(object obj)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        [Test]
        public void CorrectEqualsIsUsed()
        {
            var typeInfo = FastEquality.CreateTypeInfo(typeof(DoubleEquals));
            var a = new DoubleEquals { };
            var b = new DoubleEquals { };
            bool iseq = FastEquality.Equals<DoubleEquals>(a, b, typeInfo);

            Assert.IsTrue(iseq);
        }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
        class ComponentWithUnityObjectArray : IComponentData
        {
            public UnityEngine.Texture2D Texture;
            public UnityEngine.Object[] Objects;
            public List<string> Strings;
        }

        class ComponentOverridesGetHashCode : IComponentData
        {
            public bool GetHashCodeWasCalled = false;

            public override int GetHashCode()
            {
                GetHashCodeWasCalled = true;
                return base.GetHashCode();
            }
        }

        class ComponentImplementsIEquatable : IComponentData, IEquatable<ComponentImplementsIEquatable>
        {
            public bool EqualsWasCalled = false;
            public bool Equals(ComponentImplementsIEquatable other)
            {
                bool result = other.EqualsWasCalled == EqualsWasCalled;
                EqualsWasCalled = true;
                return result;
            }
        }

        [Test]
        public void ManagedComponentEquals()
        {
            {
                var typeInfo = FastEquality.CreateTypeInfo(typeof(EcsTestManagedComponent));
                var obj1 = new EcsTestManagedComponent() { value = "SomeString" };
                var obj12 = new EcsTestManagedComponent() { value = "SomeString" };
                var obj2 = new EcsTestManagedComponent() { value = "SomeOtherString" };
                Assert.IsTrue(FastEquality.ManagedEquals(obj1, obj1, typeInfo));
                Assert.IsTrue(FastEquality.ManagedEquals(obj1, obj12, typeInfo));
                Assert.IsFalse(FastEquality.ManagedEquals(obj1, obj2, typeInfo));
            }

            {
                var typeInfo = FastEquality.CreateTypeInfo(typeof(ComponentWithUnityObjectArray));
                var tex1 = new UnityEngine.Texture2D(512, 512);
                var tex2 = new UnityEngine.Texture2D(512, 512);
                var tex3 = new UnityEngine.Texture2D(512, 512);
                var stringList1  = new List<string>() { "yo", "hi", "hej", "hello" };
                var stringList12 = new List<string>() { "yo", "hi", "hej", "hello" };
                var stringList2  = new List<string>() { "yo", "hi", "hey", "hello" };

                var obj1  = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex2, tex3 } };
                var obj12 = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex2, tex3 } };
                var obj13 = new ComponentWithUnityObjectArray() { Strings = stringList12, Texture = tex1, Objects = new[] { tex2, tex3 } };
                var obj2  = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex2, tex2 } };
                var obj3  = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex1, null } };
                var obj4  = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex1 }};
                var obj5  = new ComponentWithUnityObjectArray() { Strings = stringList2,  Texture = tex1, Objects = new[] { tex2, tex3 } };
                var obj6  = new ComponentWithUnityObjectArray() { Strings = stringList2,  Texture = tex2, Objects = new[] { tex3, tex1 } };
                Assert.IsTrue(FastEquality.ManagedEquals(obj1, obj1, typeInfo));
                Assert.IsTrue(FastEquality.ManagedEquals(obj1, obj12, typeInfo));
                Assert.IsTrue(FastEquality.ManagedEquals(obj1, obj13, typeInfo));
                Assert.IsFalse(FastEquality.ManagedEquals(obj1, obj2, typeInfo));
                Assert.IsFalse(FastEquality.ManagedEquals(obj1, obj3, typeInfo));
                Assert.IsFalse(FastEquality.ManagedEquals(obj1, obj4, typeInfo));
                Assert.IsFalse(FastEquality.ManagedEquals(obj1, obj5, typeInfo));
                Assert.IsFalse(FastEquality.ManagedEquals(obj1, obj6, typeInfo));
            }

            {
                var typeInfo = FastEquality.CreateTypeInfo(typeof(ComponentImplementsIEquatable));
                var obj = new ComponentImplementsIEquatable();
                Assert.IsTrue(FastEquality.ManagedEquals(obj, obj, typeInfo));
                Assert.IsTrue(obj.EqualsWasCalled);
            }
        }

        [Test]
        public void ManagedComponentGetHashCode()
        {
            {
                var typeInfo = FastEquality.CreateTypeInfo(typeof(EcsTestManagedComponent));
                var obj1 = new EcsTestManagedComponent() { value = "SomeString" };
                var obj12 = new EcsTestManagedComponent() { value = "SomeString" };
                var obj2 = new EcsTestManagedComponent() { value = "SomeOtherString" };
                Assert.AreEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj1, typeInfo));
                Assert.AreEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj12, typeInfo));
                Assert.AreNotEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj2, typeInfo));
            }

            {
                var typeInfo = FastEquality.CreateTypeInfo(typeof(ComponentWithUnityObjectArray));
                var tex1 = new UnityEngine.Texture2D(512, 512);
                var tex2 = new UnityEngine.Texture2D(512, 512);
                var tex3 = new UnityEngine.Texture2D(512, 512);
                var stringList1  = new List<string>() { "yo", "hi", "hej", "hello" };
                var stringList12 = new List<string>() { "yo", "hi", "hej", "hello" };
                var stringList2  = new List<string>() { "yo", "hi", "hey", "hello" };

                var obj1  = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex2, tex3 } };
                var obj12 = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex2, tex3 } };
                var obj13 = new ComponentWithUnityObjectArray() { Strings = stringList12, Texture = tex1, Objects = new[] { tex2, tex3 } };
                var obj2  = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex2, tex2 } };
                var obj3  = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex1, null } };
                var obj4  = new ComponentWithUnityObjectArray() { Strings = stringList1,  Texture = tex1, Objects = new[] { tex1 } };
                var obj5  = new ComponentWithUnityObjectArray() { Strings = stringList2,  Texture = tex1, Objects = new[] { tex2, tex3 } };
                var obj6  = new ComponentWithUnityObjectArray() { Strings = stringList2,  Texture = tex2, Objects = new[] { tex3, tex1 } };
                Assert.AreEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj1, typeInfo));
                Assert.AreEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj12, typeInfo));
                Assert.AreEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj13, typeInfo));
                Assert.AreNotEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj2, typeInfo));
                Assert.AreNotEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj3, typeInfo));
                Assert.AreNotEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj4, typeInfo));
                Assert.AreNotEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj5, typeInfo));
                Assert.AreNotEqual(FastEquality.ManagedGetHashCode(obj1, typeInfo), FastEquality.ManagedGetHashCode(obj6, typeInfo));
            }

            {
                var typeInfo = FastEquality.CreateTypeInfo(typeof(ComponentOverridesGetHashCode));
                var obj = new ComponentOverridesGetHashCode();
                Assert.AreEqual(FastEquality.ManagedGetHashCode(obj, typeInfo), FastEquality.ManagedGetHashCode(obj, typeInfo));
                Assert.IsTrue(obj.GetHashCodeWasCalled);
            }
        }
#endif
    }
}
#endif
