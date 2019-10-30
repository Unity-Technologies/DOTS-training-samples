using System;
using System.Linq;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    partial class PropertyContainerConstructTests
    {
        [Test]
        public void PropertyContainer_Construct_AbstractFieldDoesNotCreateNewInstance()
        {
            var src = new ClassContainerWithAbstractField {Container = new DerivedClassA()};
            var dst = new ClassContainerWithAbstractField {Container = new DerivedClassA()};

            var reference = dst.Container;

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(ReferenceEquals(reference, dst.Container));
                Assert.That(!ReferenceEquals(src.Container, dst.Container));
            }
        }

        [Test]
        public void PropertyContainer_Construct_AbstractFieldConstructsInstanceOfDifferentDerivedType()
        {
            var src = new ClassContainerWithAbstractField {Container = new DerivedClassA()};
            var dst = new ClassContainerWithAbstractField {Container = new DerivedClassB()};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.Container, Is.Not.Null);
                Assert.That(dst.Container, Is.TypeOf<DerivedClassA>());
            }
        }

        [Test]
        public void PropertyContainer_Construct_AbstractFieldConstructsNewInstanceWithCorrectDerivedType()
        {
            var src = new ClassContainerWithAbstractField {Container = new DerivedClassA {BaseIntValue = 1, A = 5}};
            var dst = new ClassContainerWithAbstractField {Container = null};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.Container, Is.Not.Null);
                Assert.That(dst.Container, Is.TypeOf<DerivedClassA>());
            }
        }

        [Test]
        public void PropertyContainer_Construct_AbstractFieldConstructsNewInstanceFromDynamicSourceType()
        {
            var src = new StructContainerWithNestedDynamicContainer {Container = new DynamicContainer(typeof(DerivedClassA).AssemblyQualifiedName)};
            var dst = new ClassContainerWithAbstractField {Container = null};

            using (var result = PropertyContainer.Construct(ref dst, ref src,
                new PropertyContainerConstructOptions {TypeIdentifierKey = DynamicContainer.TypeIdentifierKey}))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.Container, Is.Not.Null);
            }

            src = new StructContainerWithNestedDynamicContainer {Container = new DynamicContainer("unknown type")};
            dst = new ClassContainerWithAbstractField {Container = null};

            using (var result = PropertyContainer.Construct(ref dst, ref src, new PropertyContainerConstructOptions {TypeIdentifierKey = DynamicContainer.TypeIdentifierKey}))
            {
                Assert.That(result.Succeeded, Is.False);
                Assert.That(result.Exceptions.Count(), Is.EqualTo(1));
                Assert.Throws<InvalidOperationException>(result.Exceptions.First().Throw);
            }
        }
    }
}