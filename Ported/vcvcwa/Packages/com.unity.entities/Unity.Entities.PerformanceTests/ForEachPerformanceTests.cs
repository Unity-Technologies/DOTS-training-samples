using Unity.Entities.Tests;
using Unity.PerformanceTesting;
using Unity.Collections;
using NUnit.Framework;

namespace Unity.Entities.PerformanceTests
{
    [Category("Performance")]
    class ForEachPerformanceTests : EntityQueryBuilderTestFixture
    {
        // Tests the performance of the ForEach & ForEach on ReadOnly components
        // No structural change expected
        [Test, Performance]
        [Category("Performance")] // bug: this redundant category here required because our current test runner ignores Category on a fixture for generated test methods  
        public void ForEach_vs_ForEachRO([Values(1, 1000, 100000)] int entityCount, [Range(0, 3)] int componentCount)
        {
            EntityArchetype archetype = new EntityArchetype();
            switch (componentCount)
            {
                case 0: archetype = m_Manager.CreateArchetype(); break;
                case 1: archetype = m_Manager.CreateArchetype(typeof(EcsTestData)); break;
                case 2: archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2)); break;
                case 3: archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData3)); break;
            }
            using (var entities = new NativeArray<Entity>(entityCount, Allocator.TempJob))
            {
                m_Manager.CreateEntity(archetype, entities);
                switch (componentCount)
                {
                    case 0:
                        Measure.Method(() =>
                            {
                                int count = 0;
                                TestSystem.Entities
                                    .ForEach((Entity entity) =>
                                {
                                    count++;
                                });
                            })
                            .Definition("ForEach")
                            .Run();
                        Measure.Method(() =>
                            {
                                int count = 0;
                                TestSystem.Entities
                                    .WithAllReadOnly<EcsTestData>()
                                    .ForEach((Entity entity) =>
                                {
                                    count++;
                                });
                            })
                            .Definition("ForEachRO")
                            .Run();
                        break;
                    case 1:
                        Measure.Method(() =>
                            {
                                int count = 0;
                                TestSystem.Entities
                                    .ForEach((Entity entity, ref EcsTestData d1) =>
                                {
                                    count++;
                                });
                            })
                            .Definition("ForEach")
                            .Run();
                        Measure.Method(() =>
                            {
                                int count = 0;
                                TestSystem.Entities
                                    .WithAllReadOnly<EcsTestData>()
                                    .ForEach((Entity entity, ref EcsTestData d1) =>
                                {
                                    count++;
                                });
                            })
                            .Definition("ForEachRO")
                            .Run();
                        break;
                    case 2:
                        Measure.Method(() =>
                            {
                                int count = 0;
                                TestSystem.Entities
                                    .ForEach((Entity entity, ref EcsTestData d1, ref EcsTestData2 d2) =>
                                {
                                    count++;
                                });
                            })
                            .Definition("ForEach")
                            .Run();
                        Measure.Method(() =>
                            {
                                int count = 0;
                                TestSystem.Entities
                                    .WithAllReadOnly<EcsTestData, EcsTestData2>()
                                    .ForEach((Entity entity, ref EcsTestData d1, ref EcsTestData2 d2) =>
                                {
                                    count++;
                                });
                            })
                            .Definition("ForEachRO")
                            .Run();
                        break;
                    case 3:
                        Measure.Method(() =>
                            {
                                int count = 0;
                                TestSystem.Entities
                                    .ForEach((Entity entity, ref EcsTestData d1, ref EcsTestData2 d2, ref EcsTestData3 d3) =>
                                {
                                    count++;
                                });
                            })
                            .Definition("ForEach")
                            .Run();
                        Measure.Method(() =>
                            {
                                int count = 0;
                                TestSystem.Entities
                                    .WithAllReadOnly<EcsTestData, EcsTestData2, EcsTestData3>()
                                    .ForEach((Entity entity, ref EcsTestData d1, ref EcsTestData2 d2, ref EcsTestData3 d3) =>
                                {
                                    count++;
                                });
                            })
                            .Definition("ForEachRO")
                            .Run();
                        break;
                }
            }
        }
    }
}
