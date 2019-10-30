using NUnit.Framework;
using Unity.Collections;
using Unity.PerformanceTesting;

namespace Unity.Entities.PerformanceTests
{
    [Category("Performance")]
    public sealed class SharedComponentPerformanceTests : EntityPerformanceTestFixture
    {
        struct TestShared1 : ISharedComponentData
        {
            public int value;
        }

        struct TestShared2 : ISharedComponentData
        {
            public int value;
        }

        unsafe struct TestData1 : IComponentData
        {
            public fixed long value[16];
        }

        struct TestData2 : IComponentData
        {
#pragma warning disable 649
            public int value;
#pragma warning restore 649
        }


        [Test, Performance]
        public void SetSharedComponentDataPerformanceTest()
        {
            var archetype = m_Manager.CreateArchetype(typeof(TestData1), typeof(TestShared1), typeof(TestShared2));
            var setSharedComponentData = new SampleGroupDefinition("SetSharedComponentData");

            NativeArray<Entity> entities = new NativeArray<Entity>(16384, Allocator.Temp);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; ++i)
                    {
                        m_Manager.SetSharedComponentData(entities[i], new TestShared1{value = i&0x003F});
                        m_Manager.SetSharedComponentData(entities[i], new TestShared2{value = i&0x0FC0});
                    }
                })
                .SetUp(() =>
                {
                    m_Manager.CreateEntity(archetype, entities);
                })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                }).Run();

            entities.Dispose();
        }

        [Test, Performance]
        public void AddComponentPerformanceTest()
        {
            var archetype = m_Manager.CreateArchetype(typeof(TestData1), typeof(TestShared1), typeof(TestShared2));
            var addComponent = new SampleGroupDefinition("AddComponent");

            NativeArray<Entity> entities = new NativeArray<Entity>(16384, Allocator.Temp);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; ++i)
                    {
                        m_Manager.AddComponentData(entities[i], new TestData2());
                    }
                })
                .SetUp(() =>
                {
                    m_Manager.CreateEntity(archetype, entities);
                    for (int i = 0; i < entities.Length; ++i)
                    {
                        m_Manager.SetSharedComponentData(entities[i], new TestShared1 {value = i & 0x003F});
                        m_Manager.SetSharedComponentData(entities[i], new TestShared2 {value = i & 0x0FC0});
                    }
                })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                }).Run();

            entities.Dispose();
        }
    }
}
