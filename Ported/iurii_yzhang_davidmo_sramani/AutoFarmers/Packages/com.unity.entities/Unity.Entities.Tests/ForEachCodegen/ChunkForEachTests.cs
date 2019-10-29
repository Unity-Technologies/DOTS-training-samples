#if ENABLE_DOTS_COMPILER_CHUNKS
using NUnit.Framework;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Transforms;

namespace Unity.Entities.Tests.ForEachCodegen
{
    [TestFixture]
    public class ChunkForEachTests : ECSTestsFixture
    {
        private SimpleChunkTestSystem TestSystem;
        private Entity TestEntity;

        [SetUp]
        public void SetUp()
        {
            TestSystem = World.GetOrCreateSystem<SimpleChunkTestSystem>();
            var myArch = m_Manager.CreateArchetype(
                ComponentType.ReadWrite<EcsTestData>(),
                ComponentType.ReadWrite<EcsTestData2>(),
                ComponentType.ReadWrite<EcsTestSharedComp>());
            TestEntity = m_Manager.CreateEntity(myArch);
            m_Manager.SetComponentData(TestEntity, new EcsTestData() { value = 3});
            m_Manager.SetComponentData(TestEntity, new EcsTestData2() { value0 = 4});
            m_Manager.SetSharedComponentData(TestEntity, new EcsTestSharedComp() { value = 5 });
            
        }

        [Test]
        public void SimpleChunkTest()
        {
            TestSystem.SimpleForEach().Complete();
            Assert.AreEqual(12, m_Manager.GetComponentData<EcsTestData>(TestEntity).value);
        }
        
        [Test]
        public void ChunkForEachWithNestedEntitiesForEachTest()
        {
            TestSystem.ChunkForEachWithNestedEntitiesForEach().Complete();
            Assert.AreEqual(12, m_Manager.GetComponentData<EcsTestData>(TestEntity).value);
        }

        public class SimpleChunkTestSystem : TestJobComponentSystem
        {
            public JobHandle SimpleForEach()
            {
                var type = EntityManager.GetArchetypeChunkComponentType<EcsTestData>(false);

                return Chunks.WithAll<EcsTestData2>()
                    .ForEach((archetypeChunk, chunkIndex, indexOfFirstEntityInQuery) =>
                    {
                        var data = archetypeChunk.GetNativeArray(type);
                        for (int i = 0; i != archetypeChunk.Count; i++)
                        {
                            var copy = data[i];
                            copy.value = 12;
                            data[i] = copy;
                        }
                    }).Schedule(default);
            }
            
            public JobHandle ChunkForEachWithNestedEntitiesForEach()
            {
                int capture_harry = 123;
                return Chunks.WithAll<EcsTestData2>()
                    .ForEach((archetypeChunk, chunkIndex, indexOfFirstEntityInQuery) =>
                    {
                        int capture_me = 12 + capture_harry;
                        archetypeChunk.Entities.ForEach((ref EcsTestData d) => { d.value = capture_me; });
                    }).Schedule(default);
            }
        }
    }
}
#endif