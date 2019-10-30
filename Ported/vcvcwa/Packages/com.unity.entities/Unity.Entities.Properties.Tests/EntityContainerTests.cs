using NUnit.Framework;
using Unity.Entities.Tests;
using Unity.Properties;
using Unity.Properties.Reflection;
using UnityEngine;

namespace Unity.Entities.Properties.Tests
{
    [TestFixture]
    internal sealed class EntityContainerTests : ECSTestsFixture
    {
        [Test]
        public void VisitEntityContainer()
        {
            var entity = m_Manager.CreateEntity(typeof(TestComponent), typeof(TestBufferElementData));

            var testComponent = m_Manager.GetComponentData<TestComponent>(entity);
            testComponent.x = 123f;
            m_Manager.SetComponentData(entity, testComponent);

            var buffer = m_Manager.GetBuffer<TestBufferElementData>(entity);
            buffer.Add(new TestBufferElementData { flt = 1.2f });
            buffer.Add(new TestBufferElementData { flt = 3.4f });

            var container = new EntityContainer(m_Manager, entity);

            PropertyContainer.Visit(container, new EntityVisitor());
        }
#if !UNITY_DISABLE_MANAGED_COMPONENTS
        [Test]
        public void VisitManagedComponentTest()
        {
            var entity = m_Manager.CreateEntity();
            m_Manager.AddComponentObject(entity, new TestManagedComponent());
            var container = new EntityContainer(m_Manager, entity);
            PropertyContainer.Visit(container, new EntityVisitor());
        }
#endif
        private class EntityVisitor : PropertyVisitor
        {
            protected override VisitStatus BeginContainer<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Debug.Log($"BeginContainer [{typeof(TContainer)}] [{typeof(TValue)}]");
#if !UNITY_DISABLE_MANAGED_COMPONENTS
                if (typeof(TestManagedComponent).IsAssignableFrom(typeof(TValue)))
                {
                    Assert.That(value, Is.Not.Null);
                }
#endif
                return VisitStatus.Handled;
            }

            protected override VisitStatus BeginCollection<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Debug.Log($"BeginCollection [{typeof(TContainer)}] [{typeof(TValue)}]");
                return VisitStatus.Handled;
            }

            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                Debug.Log($"Visit [{typeof(TContainer)}] [{typeof(TValue)}]");
                return VisitStatus.Handled;
            }
        }
    }
}
