using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    partial class ReflectedPropertyBagGeneratorTests
    {
        class ClassContainerWithPrivateProperties
        {
            public static string IntPropertyName => nameof(IntProperty);
            public static string FloatPropertyName => nameof(FloatProperty);
            public static string NonMaskedPropertyName => nameof(NonMaskedProperty);

            private int IntProperty { get; set; } = 42;
            [Property] private float FloatProperty { get; set; } = 123.456f;
            [Property] private int NonMaskedProperty { get; set; } = 1;
        }

        class DerivedClassContainerWithPrivateProperties : ClassContainerWithPrivateProperties
        {
            public static string BoolPropertyName => nameof(BoolProperty);
            public static string StringPropertyName => nameof(StringProperty);

            private bool BoolProperty { get; set; } = true;
            [Property] private string StringProperty { get; set; } = "Hello the World!";
            [Property] private int NonMaskedProperty { get; set; } = 2;
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for class containers with private properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PrivateProperties()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ClassContainerWithPrivateProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateProperties.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateProperties.NonMaskedPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPrivateProperties.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPrivateProperties.NonMaskedPropertyName, 1);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPrivateProperties.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPrivateProperties.StringPropertyName), Is.False);
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for derived class containers with private properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PrivateProperties_DerivedClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<DerivedClassContainerWithPrivateProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateProperties.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateProperties.NonMaskedPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPrivateProperties.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPrivateProperties.NonMaskedPropertyName, 2);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPrivateProperties.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPrivateProperties.StringPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, DerivedClassContainerWithPrivateProperties.StringPropertyName, "Hello the World!");
        }
    }
}
