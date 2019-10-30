using System.Collections.Generic;
using NUnit.Framework;

namespace Unity.Properties.Reflection.Tests
{
    [TestFixture]
    class ReflectedPropertyBagVisitorTests
    {
        struct SimpleContainer
        {
#pragma warning disable 649
            public int Int32Value;
            public float Float32Value;
            public string StringValue;
            public byte UInt8Value;
            public ushort Int16Value;
            public NestedContainer Nested;
#pragma warning restore 649
        }

        struct Foo
        {
#pragma warning disable 649
            public NestedContainer Nested;
#pragma warning restore 649
        }

        struct NestedContainer
        {
#pragma warning disable 649
            public int Int32Value;
            public int Foo;
            public byte UInt8Value;
            public ushort Int16Value;
#pragma warning restore 649
        }
        
        struct ContainerWithCollections
        {
            public IList<int> IListGeneric;
            public List<int> ListGeneric;
            public int[] Array;
        }

        class VoidVisitor : PropertyVisitor
        {
            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                return VisitStatus.Handled;
            }
        }

        [Test]
        public void ReflectedPropertyBagVisitor_Visit()
        {
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<SimpleContainer>());
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<NestedContainer>());

            PropertyContainer.Visit(new SimpleContainer(), new VoidVisitor());
        }

        [Test]
        public void ReflectedPropertyBagVisitor_Transfer_NestedContainer()
        {
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<SimpleContainer>());
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<NestedContainer>());
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<Foo>());

            var source = new SimpleContainer
            {
                Int32Value = 15,
                Nested = new NestedContainer
                {
                    Int32Value = 42
                }
            };

            var foo = new Foo
            {
                Nested = new NestedContainer {Int32Value = 10}
            };

            using (var result = PropertyContainer.Transfer(ref foo, ref source))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.AreEqual(42, foo.Nested.Int32Value);
            }
        }

        [Test]
        public void ReflectedPropertyBagVisitor_Transfer_Collections()
        {
            PropertyBagResolver.Register(new ReflectedPropertyBagProvider().Generate<ContainerWithCollections>());

            var source = new ContainerWithCollections
            {
                IListGeneric = new List<int> { 4, 5, 6},
                ListGeneric = new List<int> { 7 },
                Array = new [] { 6, 9, 10, 11 }
            };

            var destination = new ContainerWithCollections
            {
                IListGeneric = new List<int>(),
                ListGeneric = new List<int>(),
                Array = new int[0]
            };

            using (var result = PropertyContainer.Transfer(ref destination, ref source))
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(destination.IListGeneric.Count, Is.EqualTo(3));
                Assert.That(destination.ListGeneric.Count, Is.EqualTo(1));
                Assert.That(destination.Array.Length, Is.EqualTo(4));
            }
        }
    }
}
