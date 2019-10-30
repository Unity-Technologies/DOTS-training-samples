using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace Unity.Properties.Tests
{
    [TestFixture]
    class PropertyVisitorTests
    {
        class DebugLogVisitor : PropertyVisitor
        {
            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Debug.Log($"Visit PropertyType=[{typeof(TProperty)}] PropertyName=[{property.GetName()}]");
                return VisitStatus.Handled;
            }

            protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Debug.Log($"BeginContainer PropertyType=[{typeof(TProperty)}] PropertyName=[{property.GetName()}]");
                return VisitStatus.Handled;
            }

            protected override void EndContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Debug.Log($"EndContainer PropertyType=[{typeof(TProperty)}] PropertyName=[{property.GetName()}]");
            }

            protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Debug.Log($"BeginCollection PropertyType=[{typeof(TProperty)}] PropertyName=[{property.GetName()}]");
                return VisitStatus.Handled;
            }

            protected override void EndCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Debug.Log($"EndCollection PropertyType=[{typeof(TProperty)}] PropertyName=[{property.GetName()}]");
            }
        }

        class AssertConcreteTypeVisitor : PropertyVisitor
        {
            public Type ExpectedConcreteType;

            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Assert.That(typeof(TContainer), Is.EqualTo(ExpectedConcreteType));
                return base.Visit(property, ref container, ref value, ref changeTracker);
            }
        }

        struct StructVisitorWithState : IPropertyVisitor
        {
            public int VisitCount;

            public VisitStatus VisitProperty<TProperty, TContainer, TValue>(
                TProperty property, 
                ref TContainer container, 
                ref ChangeTracker changeTracker) 
                where TProperty : IProperty<TContainer, TValue>
            {
                VisitCount++;
                return VisitStatus.Handled;
            }

            public VisitStatus VisitCollectionProperty<TProperty, TContainer, TValue>(
                TProperty property, 
                ref TContainer container, 
                ref ChangeTracker changeTracker) 
                where TProperty : ICollectionProperty<TContainer, TValue>
            {
                VisitCount++;
                return VisitStatus.Handled;
            }
        }

        [SetUp]
        public void SetUp()
        {
            TestData.InitializePropertyBags();
        }

        [Test]
        public void PropertyVisitor_Visit_Struct()
        {
            var container = new TestPrimitiveContainer();
            PropertyContainer.Visit(ref container, new DebugLogVisitor());
        }

        [Test]
        public void PropertyVisitor_Visit_StructWithNestedStruct()
        {
            var container = new TestNestedContainer();
            PropertyContainer.Visit(ref container, new DebugLogVisitor());
        }

        [Test]
        public void PropertyVisitor_Visit_StructWithArray()
        {
            var container = new TestArrayContainer
            {
                Int32Array = new [] { 1, 2, 3 },
                TestContainerArray = new [] { new TestPrimitiveContainer(), new TestPrimitiveContainer() }
            };

            PropertyContainer.Visit(ref container, new DebugLogVisitor());
        }
        
        [Test]
        public void PropertyVisitor_Visit_BoxedStruct()
        {
            var container = new TestPrimitiveContainer();
            var boxed = (object) container;
            PropertyContainer.Visit(ref boxed, new AssertConcreteTypeVisitor { ExpectedConcreteType = typeof(TestPrimitiveContainer)});
        }
        
        [Test]
        public void PropertyVisitor_Visit_StructWithNestedInterface()
        {
            var container = new TestInterfaceContainer
            {
                CustomData = new CustomDataFoo()
            };
            PropertyContainer.Visit(ref container, new DebugLogVisitor());
        }

        [Test]
        public void PropertyVisitor_Visit_MultiThread()
        {
            // Simple test to ensure we can visit a handful of types on many threads.
            Assert.DoesNotThrow(() =>
            {
                const int kThreads = 8;
                ThreadStart func = MultiVisitWorker;
                List<Thread> threads = new List<Thread>(kThreads);
                for (int i = 0; i < kThreads; ++i)
                    threads.Add(new Thread(func));

                foreach (var t in threads)
                    t.Start();
                foreach (var t in threads)
                    t.Join();
            });
        }

        [Test]
        public void PropertyVisitor_Visit_StructVisitorWithState()
        {
            var container = new TestPrimitiveContainer();
            var visitor = new StructVisitorWithState();
            PropertyContainer.Visit(ref container, ref visitor);
            Assert.That(visitor.VisitCount, Is.EqualTo(14));
        }
        
        static void MultiVisitWorker()
        {
            // Many types to make sure our thread has enough work
            var containers = new object[]
            {
                new TestPrimitiveContainer(),
                new int(),
                new TestInterfaceContainer(),
                new char(),
                new bool(),
                new CustomDataFoo(),
                "",
                new TestNestedContainer(),
                new TestArrayContainer()
            };

            for(int i = 0; i < containers.Length; ++i)
                PropertyContainer.Visit(ref containers[i], new DebugLogVisitor());
        }
    }
}
