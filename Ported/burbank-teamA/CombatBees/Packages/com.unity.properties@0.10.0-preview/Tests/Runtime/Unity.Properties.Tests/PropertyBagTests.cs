using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    class PropertyBagTests
    {
        class SetPropertyVisitor : PropertyVisitor
        {
            public string Name;
            public object Value;

            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                if (string.Equals(property.GetName(), Name))
                {
                    value = TypeConversion.Convert<object, TValue>(Value);
                }
                
                return base.Visit(property, ref container, ref value, ref changeTracker);
            }
        }

        [SetUp]
        public void SetUp()
        {
            TestData.InitializePropertyBags();
        }
        
        /// <summary>
        /// Test that <see cref="IPropertyBag.Accept{TVisitor}"/> will correctly handle boxed structs.
        /// </summary>
        [Test]
        public void PropertyBag_Accept_Object()
        {
            var untypedPropertyBag = PropertyBagResolver.Resolve(typeof(TestPrimitiveContainer));
            var boxedContainer = (object) default(TestPrimitiveContainer);
            var visitor = new SetPropertyVisitor {Name = nameof(TestPrimitiveContainer.Int32Value), Value = 42};
            var changeTracker = default(ChangeTracker);
            untypedPropertyBag.Accept(ref boxedContainer, ref visitor, ref changeTracker);
            Assert.That(((TestPrimitiveContainer) boxedContainer).Int32Value, Is.EqualTo(42));
        }
    }
}