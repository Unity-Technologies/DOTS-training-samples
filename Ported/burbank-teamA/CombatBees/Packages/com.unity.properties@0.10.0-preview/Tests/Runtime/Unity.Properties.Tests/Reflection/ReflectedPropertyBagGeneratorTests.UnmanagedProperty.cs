using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    partial class ReflectedPropertyBagGeneratorTests
    {
        struct ContainerWithCharField
        {
#pragma warning disable 649
            public char c;
#pragma warning restore 649
        }

        /// <summary>
        /// Tests that the <see cref="ReflectedPropertyBagProvider"/> correctly generates a <see cref="UnmanagedProperty{TContainer,TValue}"/> for char fields.
        /// </summary>
        [Test]
        public void ReflectedPropertyBagGenerator_UnmanagedProperty_Char()
        {
            var propertyBag = new ReflectedPropertyBagProvider().Generate<ContainerWithCharField>();
            AssertPropertyIsOfType<ContainerWithCharField, UnmanagedProperty<ContainerWithCharField, char>>(propertyBag, "c");

            var container = default(ContainerWithCharField);
            var changeTracker = default(ChangeTracker);
            var action = new AssertThatPropertyIsOfType<ContainerWithCharField, UnmanagedProperty<ContainerWithCharField, char>>();
            Assert.That(propertyBag.FindProperty("c", ref container, ref changeTracker, ref action), Is.True);
        }
    }
}
