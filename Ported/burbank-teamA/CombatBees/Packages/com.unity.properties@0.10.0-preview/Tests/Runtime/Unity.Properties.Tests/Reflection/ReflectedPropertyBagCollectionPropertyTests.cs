using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    class ReflectedPropertyBagCollectionPropertyTests
    {
        struct ContainerWithCollections
        {
#pragma warning disable 649
            public IList<int> IListGeneric;
            public List<int> ListGeneric;
            public int[] Array;
#pragma warning restore 649
        }

        [Test]
        public void ReflectedPropertyBag_ReflectedCollectionProperties()
        {
            var container = default(ContainerWithCollections);

            AssertCollectionPropertyType(ref container, nameof(ContainerWithCollections.IListGeneric), typeof(ReflectedGenericListProperty<ContainerWithCollections, IList<int>, int>));
            AssertCollectionPropertyType(ref container, nameof(ContainerWithCollections.ListGeneric), typeof(ReflectedGenericListProperty<ContainerWithCollections, List<int>, int>));
            AssertCollectionPropertyType(ref container, nameof(ContainerWithCollections.Array), typeof(ReflectedArrayProperty<ContainerWithCollections, int>));
        }
        
        static void AssertCollectionPropertyType<TContainer>(ref TContainer container, string name, Type type)
        {
            var change = new ChangeTracker();
            var action = new AssertCollectionPropertyTypeGetter<TContainer>
            {
                ExpectedPropertyType = type
            };

            PropertyBagResolver.Resolve<TContainer>().FindProperty(name, ref container, ref change, ref action);
        }

        struct AssertCollectionPropertyTypeGetter<T> : IPropertyGetter<T>
        {
            public Type ExpectedPropertyType;
            
            public void VisitProperty<TProperty, TValue>(TProperty property, ref T container, ref ChangeTracker changeTracker) where TProperty : IProperty<T, TValue>
            {
                throw new AssertionException("Property type is expected to be a collection.");
            }

            public void VisitCollectionProperty<TProperty, TValue>(TProperty property, ref T container, ref ChangeTracker changeTracker) where TProperty : ICollectionProperty<T, TValue>
            {
                Assert.That(typeof(TProperty), Is.EqualTo(ExpectedPropertyType));
            }
        }
    }
}