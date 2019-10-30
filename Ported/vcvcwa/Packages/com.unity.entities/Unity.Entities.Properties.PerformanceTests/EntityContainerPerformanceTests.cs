using NUnit.Framework;
using Unity.Entities.PerformanceTests;
using Unity.Entities.Properties.Tests;
using Unity.PerformanceTesting;
using Unity.Properties;

namespace Unity.Entities.Properties.PerformanceTests
{
    [TestFixture]
    [Category("Performance")]
    sealed class EntityContainerPerformanceTests : EntityPerformanceTestFixture
    {
        [Test, Performance]
        public void PerformanceTest_VisitEntityContainer()
        {
            var entity = m_Manager.CreateEntity(typeof(TestComponent), typeof(TestBufferElementData));

            var testComponent = m_Manager.GetComponentData<TestComponent>(entity);
            testComponent.x = 123f;
            m_Manager.SetComponentData(entity, testComponent);

            var buffer = m_Manager.GetBuffer<TestBufferElementData>(entity);
            buffer.Add(new TestBufferElementData { flt = 1.2f });
            buffer.Add(new TestBufferElementData { flt = 3.4f });

            var visitor = new DebugVisitor();

            Measure.Method(() =>
                   {
                       PropertyContainer.Visit(ref testComponent, visitor);
                   })
                   .Definition("EntityContainerVisit")
                   .WarmupCount(1)
                   .MeasurementCount(100)
                   .GC()
                   .Run();
        }

        class DebugVisitor : PropertyVisitor
        {
            protected override VisitStatus Visit<TProperty, TContainer, TValue>(TProperty property, ref TContainer container, ref TValue value, ref ChangeTracker changeTracker)
            {
                return VisitStatus.Handled;
            }
        }
    }
}
