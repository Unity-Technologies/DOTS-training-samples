using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    partial class ReflectedPropertyBagGeneratorTests
    {
        class ClassContainerWithPublicProperties
        {
            public static string IntPropertyName => nameof(IntProperty);
            public static string FloatPropertyName => nameof(FloatProperty);
            public static string MaskedPropertyName => nameof(MaskedProperty);
            public static string VirtualPropertyName => nameof(VirtualProperty);

            public int IntProperty { get; set; } = 42;
            [Property] public float FloatProperty { get; set; } = 123.456f;
            [Property] public int MaskedProperty { get; set; } = 1;
            [Property] public virtual short VirtualProperty { get; set; } = -12345;
        }

        class DerivedClassContainerWithPublicProperties : ClassContainerWithPublicProperties
        {
            public static string BoolPropertyName => nameof(BoolProperty);
            public static string StringPropertyName => nameof(StringProperty);

            public bool BoolProperty { get; set; } = true;
            [Property] public string StringProperty { get; set; } = "Hello the World!";
            [Property] public new int MaskedProperty { get; set; } = 2;
            [Property] public override short VirtualProperty { get; set; } = 12345;
        }

        abstract class AbstractClassContainerWithPublicProperties
        {
            public static string IntPropertyName => nameof(IntProperty);
            public static string FloatPropertyName => nameof(FloatProperty);

            public abstract int IntProperty { get; set; }
            [Property] public abstract float FloatProperty { get; set; }
        }

        class ImplementedAbstractClassContainerWithPublicProperties : AbstractClassContainerWithPublicProperties
        {
            public override int IntProperty { get; set; } = 13;
            [Property] public override float FloatProperty { get; set; } = 3.1416f;
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for class containers with public properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PublicProperties()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ClassContainerWithPublicProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicProperties.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicProperties.MaskedPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicProperties.VirtualPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicProperties.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicProperties.MaskedPropertyName, 1);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicProperties.VirtualPropertyName, (short)-12345);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPublicProperties.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPublicProperties.StringPropertyName), Is.False);
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for derived class containers with public properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PublicProperties_DerivedClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<DerivedClassContainerWithPublicProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicProperties.FloatPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicProperties.MaskedPropertyName), Is.True);
            Assert.That(propertyBag.HasProperty(ClassContainerWithPublicProperties.VirtualPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicProperties.FloatPropertyName, 123.456f);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicProperties.MaskedPropertyName, 2);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, ClassContainerWithPublicProperties.VirtualPropertyName, (short)12345);

            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPublicProperties.BoolPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(DerivedClassContainerWithPublicProperties.StringPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, DerivedClassContainerWithPublicProperties.StringPropertyName, "Hello the World!");
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for abstract class containers with public properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PublicProperties_AbstractClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<AbstractClassContainerWithPublicProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(AbstractClassContainerWithPublicProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(AbstractClassContainerWithPublicProperties.FloatPropertyName), Is.True);
        }

        /// <summary>
        /// Ensure <see cref="ReflectedPropertyBagProvider"/> correctly generates for implemented abstract class containers with public properties.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_PublicProperties_ImplementedAbstractClass()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ImplementedAbstractClassContainerWithPublicProperties>();
            Assert.That(propertyBag, Is.Not.Null);

            Assert.That(propertyBag.HasProperty(AbstractClassContainerWithPublicProperties.IntPropertyName), Is.False);
            Assert.That(propertyBag.HasProperty(AbstractClassContainerWithPublicProperties.FloatPropertyName), Is.True);
            AssertPropertyValueAndTypeIsEqualTo(propertyBag, AbstractClassContainerWithPublicProperties.FloatPropertyName, 3.1416f);
        }
    }
}
