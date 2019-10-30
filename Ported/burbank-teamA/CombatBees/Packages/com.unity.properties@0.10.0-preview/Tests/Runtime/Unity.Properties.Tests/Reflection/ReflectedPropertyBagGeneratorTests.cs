using NUnit.Framework;
using System;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    partial class ReflectedPropertyBagGeneratorTests
    {
        struct AssertThatPropertyIsOfType<TContainer, TExpected> : IPropertyGetter<TContainer>
        {
            public void VisitProperty<TProperty, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, TValue>
            {
                Assert.That(property.GetType(), Is.EqualTo(typeof(TExpected)));
            }

            public void VisitCollectionProperty<TProperty, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
                where TProperty : ICollectionProperty<TContainer, TValue> => throw new NotImplementedException();
        }

        static void AssertPropertyIsOfType<TContainer, TValue>(IPropertyBag<TContainer> propertyBag, string propertyName)
        {
            var container = TypeConstruction.Construct<TContainer>(typeof(TContainer));
            var changeTracker = default(ChangeTracker);
            var action = new AssertThatPropertyIsOfType<TContainer, TValue>();
            Assert.That(propertyBag.FindProperty(propertyName, ref container, ref changeTracker, ref action), Is.True);
        }

        struct AssertThatPropertyValueAndTypeIsEqualTo<TContainer> : IPropertyGetter<TContainer>
        {
            public object ExpectedValue;

            public void VisitProperty<TProperty, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
                where TProperty : IProperty<TContainer, TValue>
            {
                var value = property.GetValue(ref container);
                Assert.That(value.GetType(), Is.EqualTo(ExpectedValue.GetType()));
                Assert.That(value, Is.EqualTo(ExpectedValue));
            }

            public void VisitCollectionProperty<TProperty, TValue>(TProperty property, ref TContainer container, ref ChangeTracker changeTracker)
                where TProperty : ICollectionProperty<TContainer, TValue> => throw new NotImplementedException();
        }

        static void AssertPropertyValueAndTypeIsEqualTo<TContainer>(IPropertyBag<TContainer> propertyBag, string propertyName, object expectedValue)
        {
            var container = TypeConstruction.Construct<TContainer>(typeof(TContainer));
            var changeTracker = default(ChangeTracker);
            var action = new AssertThatPropertyValueAndTypeIsEqualTo<TContainer> { ExpectedValue = expectedValue };
            Assert.That(propertyBag.FindProperty(propertyName, ref container, ref changeTracker, ref action), Is.True);
        }
    }
}
