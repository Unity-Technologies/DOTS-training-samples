using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    partial class ReflectedPropertyBagGeneratorTests
    {
        class ClassContainerWithInternalFields
        {
            public static string IntPropertyName => nameof(m_IntField);
            public static string FloatPropertyName => nameof(m_FloatField);
            public static string MaskedPropertyName => nameof(m_MaskedField);

            internal int m_IntField = 42;
            [Property] internal float m_FloatField = 123.456f;
            [Property] internal int m_MaskedField = 1;
        }

        class DerivedClassContainerWithInternalFields : ClassContainerWithInternalFields
        {
            public static string BoolPropertyName => nameof(m_BoolField);
            public static string StringPropertyName => nameof(m_StringField);

            internal bool m_BoolField = true;
            [Property] internal string m_StringField = "Hello the World!";
            [Property] internal new int m_MaskedField = 2;
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for class containers with internal fields.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_InternalFields()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ClassContainerWithInternalFields>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalFields.IntPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalFields.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalFields.MaskedPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalFields.IntPropertyName, 42);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalFields.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalFields.MaskedPropertyName, 1);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithInternalFields.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithInternalFields.StringPropertyName), Is.False);
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for derived class containers with internal fields.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_InternalFields_DerivedClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<DerivedClassContainerWithInternalFields>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalFields.IntPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalFields.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithInternalFields.MaskedPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalFields.IntPropertyName, 42);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalFields.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithInternalFields.MaskedPropertyName, 2);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithInternalFields.BoolPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithInternalFields.StringPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, DerivedClassContainerWithInternalFields.BoolPropertyName, true);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, DerivedClassContainerWithInternalFields.StringPropertyName, "Hello the World!");
        }
    }
}
