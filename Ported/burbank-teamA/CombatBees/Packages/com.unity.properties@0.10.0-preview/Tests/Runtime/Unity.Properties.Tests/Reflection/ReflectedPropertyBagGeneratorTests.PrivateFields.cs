using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    partial class ReflectedPropertyBagGeneratorTests
    {
        class ClassContainerWithPrivateFields
        {
            public static string IntPropertyName => nameof(m_IntField);
            public static string FloatPropertyName => nameof(m_FloatField);
            public static string NonMaskedPropertyName => nameof(m_NonMaskedField);

#pragma warning disable 414 // member is assigned but its value is never used
            private int m_IntField = 42;
            [Property] private float m_FloatField = 123.456f;
            [Property] private int m_NonMaskedField = 1;
#pragma warning restore 414 // member is assigned but its value is never used
        }

        class DerivedClassContainerWithPrivateFields : ClassContainerWithPrivateFields
        {
            public static string BoolPropertyName => nameof(m_BoolField);
            public static string StringPropertyName => nameof(m_StringField);

#pragma warning disable 414 // member is assigned but its value is never used
            private bool m_BoolField = true;
            [Property] private string m_StringField = "Hello the World!";
            [Property] private int m_NonMaskedField = 2;
#pragma warning restore 414 // member is assigned but its value is never used
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for class containers with private fields.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PrivateFields()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ClassContainerWithPrivateFields>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateFields.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateFields.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateFields.NonMaskedPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPrivateFields.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPrivateFields.NonMaskedPropertyName, 1);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPrivateFields.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPrivateFields.StringPropertyName), Is.False);
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for derived class containers with private fields.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PrivateFields_DerivedClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<DerivedClassContainerWithPrivateFields>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateFields.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateFields.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPrivateFields.NonMaskedPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPrivateFields.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPrivateFields.NonMaskedPropertyName, 2);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPrivateFields.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPrivateFields.StringPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, DerivedClassContainerWithPrivateFields.StringPropertyName, "Hello the World!");
        }
    }
}
