using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    partial class ReflectedPropertyBagGeneratorTests
    {
        class ClassContainerWithInternalProperties
        {
            public static string IntPropertyName => nameof(IntProperty);
            public static string FloatPropertyName => nameof(FloatProperty);
            public static string MaskedPropertyName => nameof(MaskedProperty);
            public static string VirtualPropertyName => nameof(VirtualProperty);

            internal int IntProperty { get; set; } = 42;
            [Property] internal float FloatProperty { get; set; } = 123.456f;
            [Property] internal int MaskedProperty { get; set; } = 1;
            [Property] internal virtual short VirtualProperty { get; set; } = -12345;
        }

        class DerivedClassContainerWithInternalProperties : ClassContainerWithInternalProperties
        {
            public static string BoolPropertyName => nameof(BoolProperty);
            public static string StringPropertyName => nameof(StringProperty);

            internal bool BoolProperty { get; set; } = true;
            [Property] internal string StringProperty { get; set; } = "Hello the World!";
            [Property] internal new int MaskedProperty { get; set; } = 2;
            [Property] internal override short VirtualProperty { get; set; } = 12345;
        }

        abstract class AbstractClassContainerWithInternalProperties
        {
            public static string IntPropertyName => nameof(IntProperty);
            public static string FloatPropertyName => nameof(FloatProperty);

            internal abstract int IntProperty { get; set; }
            [Property] internal abstract float FloatProperty { get; set; }
        }

        class ImplementedAbstractClassContainerWithInternalProperties : AbstractClassContainerWithInternalProperties
        {
            internal override int IntProperty { get; set; } = 13;
            [Property] internal override float FloatProperty { get; set; } = 3.1416f;
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for class containers with internal properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_InternalProperties()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ClassContainerWithInternalProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalProperties.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalProperties.MaskedPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalProperties.VirtualPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalProperties.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalProperties.MaskedPropertyName, 1);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalProperties.VirtualPropertyName, (short)-12345);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithInternalProperties.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithInternalProperties.StringPropertyName), Is.False);
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for derived class containers with internal properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_InternalProperties_DerivedClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<DerivedClassContainerWithInternalProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalProperties.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalProperties.MaskedPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalProperties.VirtualPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalProperties.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalProperties.MaskedPropertyName, 2);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalProperties.VirtualPropertyName, (short)12345);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithInternalProperties.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithInternalProperties.StringPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, DerivedClassContainerWithInternalProperties.StringPropertyName, "Hello the World!");
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for abstract class containers with internal properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_InternalProperties_AbstractClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<AbstractClassContainerWithInternalProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(AbstractClassContainerWithInternalProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(AbstractClassContainerWithInternalProperties.FloatPropertyName), Is.True);
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for implemented abstract class containers with internal properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_InternalProperties_ImplementedAbstractClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ImplementedAbstractClassContainerWithInternalProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(AbstractClassContainerWithInternalProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(AbstractClassContainerWithInternalProperties.FloatPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, AbstractClassContainerWithInternalProperties.FloatPropertyName, 3.1416f);
        }
    }
}
