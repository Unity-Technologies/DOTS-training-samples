using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    partial class PropertyContainerConstructTests
    {
        [Test]
        public void PropertyContainer_Construct_ReferenceFieldDoesNotDestroyExistingInstance()
        {
            var src = new ClassContainerWithNestedClass {Container = new ClassContainerWithPrimitives()};
            var dst = new ClassContainerWithNestedClass {Container = new ClassContainerWithPrimitives()};

            var reference = dst.Container;

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(ReferenceEquals(reference, dst.Container));
                Assert.That(!ReferenceEquals(src.Container, dst.Container));
            }
        }

        [Test]
        public void PropertyContainer_Construct_ReferenceFieldConstructsNewInstance()
        {
            var src = new ClassContainerWithNestedClass {Container = new ClassContainerWithPrimitives()};
            var dst = new ClassContainerWithNestedClass {Container = null};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.Container, Is.Not.Null);
                Assert.That(!ReferenceEquals(src.Container, dst.Container));
            }
        }

        [Test]
        public void PropertyContainer_Construct_ReferenceFieldAssignsNull()
        {
            var src = new ClassContainerWithNestedClass {Container = null};
            var dst = new ClassContainerWithNestedClass {Container = new ClassContainerWithPrimitives()};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.Container, Is.Null);
            }
        }

        [Test]
        public void PropertyContainer_Construct_ReferenceFieldConstructsNewInstanceWithDifferentSourceType()
        {
            var src = new StructContainerWithNestedStruct {Container = new StructContainerWithPrimitives()};
            var dst = new ClassContainerWithNestedClass {Container = null};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.Container, Is.Not.Null);
            }
        }

        [Test]
        public void PropertyContainer_Construct_ShouldConstructWhenDstContainerIsNull()
        {
            var src = new ClassContainerWithNestedClass {Container = new ClassContainerWithPrimitives()};
            var dst = (ClassContainerWithNestedClass) null;

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst, Is.Not.Null);
                Assert.That(dst.Container, Is.Not.Null);
            }
        }
    }
}