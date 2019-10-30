using System.Collections.Generic;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    partial class PropertyContainerConstructTests
    {
        [Test]
        public void PropertyContainer_Construct_ListFieldDoesNotDestroyExistingInstance()
        {
            var src = new ClassContainerWithLists {IntList = new List<int>()};
            var dst = new ClassContainerWithLists {IntList = new List<int>()};

            var reference = dst.IntList;

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(ReferenceEquals(reference, dst.IntList));
                Assert.That(!ReferenceEquals(src.IntList, dst.IntList));
            }
        }
        
        [Test]
        public void PropertyContainer_Construct_ListFieldConstructsNewInstance()
        {
            var src = new ClassContainerWithLists {IntList = new List<int>()};
            var dst = new ClassContainerWithLists {IntList = null};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.IntList, Is.Not.Null);
                Assert.That(!ReferenceEquals(src.IntList, dst.IntList));
            }
        }
        
        [Test]
        public void PropertyContainer_Construct_ListFieldAssignsNull()
        {
            var src = new ClassContainerWithLists {IntList = null};
            var dst = new ClassContainerWithLists {IntList = new List<int>()};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.IntList, Is.Null);
            }
        }
        
        [Test]
        public void PropertyContainer_Construct_ListFieldElementConstructsNewInstance()
        {
            var src = new ClassContainerWithLists {ContainerWithPrimitivesList = new List<ClassContainerWithPrimitives>
            {
                new ClassContainerWithPrimitives()
            }};
            var dst = new ClassContainerWithLists {ContainerWithPrimitivesList = new List<ClassContainerWithPrimitives>()};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.ContainerWithPrimitivesList, Is.Not.Null);
                Assert.That(!ReferenceEquals(src.ContainerWithPrimitivesList, dst.ContainerWithPrimitivesList));
                Assert.That(dst.ContainerWithPrimitivesList.Count, Is.EqualTo(1));
                Assert.That(!ReferenceEquals(src.ContainerWithPrimitivesList[0], dst.ContainerWithPrimitivesList[0]));
            }
        }
        
        [Test]
        public void PropertyContainer_Construct_ListFieldElementAssignsNull()
        {
            var src = new ClassContainerWithLists {ContainerWithPrimitivesList = new List<ClassContainerWithPrimitives>
            {
                null
            }};
            var dst = new ClassContainerWithLists {ContainerWithPrimitivesList = new List<ClassContainerWithPrimitives>
            {
                new ClassContainerWithPrimitives()
            }};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.ContainerWithPrimitivesList, Is.Not.Null);
                Assert.That(!ReferenceEquals(src.ContainerWithPrimitivesList, dst.ContainerWithPrimitivesList));
                Assert.That(dst.ContainerWithPrimitivesList.Count, Is.EqualTo(1));
                Assert.That(dst.ContainerWithPrimitivesList[0], Is.Null);
            }
        }
        
        [Test]
        public void PropertyContainer_Construct_ListFieldElementConstructsNewInstanceWithDifferentSourceType()
        {
            var src = new ClassContainerWithLists {ContainerWithPrimitivesList = new List<ClassContainerWithPrimitives>
            {
                null
            }};
            var dst = new ClassContainerWithLists {ContainerWithPrimitivesList = new List<ClassContainerWithPrimitives>
            {
                new ClassContainerWithPrimitives()
            }};

            using (var result = PropertyContainer.Construct(ref dst, ref src))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(dst.ContainerWithPrimitivesList, Is.Not.Null);
                Assert.That(!ReferenceEquals(src.ContainerWithPrimitivesList, dst.ContainerWithPrimitivesList));
                Assert.That(dst.ContainerWithPrimitivesList.Count, Is.EqualTo(1));
                Assert.That(dst.ContainerWithPrimitivesList[0], Is.Null);
            }
        }
    }
}