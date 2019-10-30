using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    partial class ReflectedPropertyBagGeneratorTests
    {
        class ClassContainerWithPublicFields
        {
            public static string IntPropertyName => nameof(m_IntField);
            public static string FloatPropertyName => nameof(m_FloatField);
            public static string MaskedPropertyName => nameof(m_MaskedField);

            public int m_IntField = 42;
            [Property] public float m_FloatField = 123.456f;
            [Property] public int m_MaskedField = 1;
        }

        class DerivedClassContainerWithPublicFields : ClassContainerWithPublicFields
        {
            public static string BoolPropertyName => nameof(m_BoolField);
            public static string StringPropertyName => nameof(m_StringField);

            public bool m_BoolField = true;
            [Property] public string m_StringField = "Hello the World!";
            [Property] public new int m_MaskedField = 2;
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for class containers with public fields.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PublicFields()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ClassContainerWithPublicFields>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicFields.IntPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicFields.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicFields.MaskedPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicFields.IntPropertyName, 42);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicFields.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicFields.MaskedPropertyName, 1);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPublicFields.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPublicFields.StringPropertyName), Is.False);
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for derived class containers with public fields.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PublicFields_DerivedClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<DerivedClassContainerWithPublicFields>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicFields.IntPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicFields.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicFields.MaskedPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicFields.IntPropertyName, 42);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicFields.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicFields.MaskedPropertyName, 2);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPublicFields.BoolPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPublicFields.StringPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, DerivedClassContainerWithPublicFields.BoolPropertyName, true);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, DerivedClassContainerWithPublicFields.StringPropertyName, "Hello the World!");
        }
    }
}
