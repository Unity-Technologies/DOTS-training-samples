using System;
using System.Linq;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities.Tests
{
    [TestFixture("CompleteAllJobs")]
    [TestFixture("CompleteJob1")]
    [TestFixture("CompleteJob2")]
    [TestFixture("CompleteNoJobs")]
    [StandaloneFixme]
    class EntityQuerySyncChangeFilterTypesTests : ECSTestsFixture
    {
        bool completeJob1;
        bool completeJob2;

        JobHandle job1;
        JobHandle job2;

        EntityQuery entityQuery;
        Entity entity;

        public EntityQuerySyncChangeFilterTypesTests(string variation)
        {
            this.completeJob1 = variation == "CompleteJob1" || variation == "CompleteAllJobs";
            this.completeJob2 = variation == "CompleteJob2" || variation == "CompleteAllJobs";
        }

        public override void Setup()
        {
            base.Setup();

            m_Manager.Debug.SetGlobalSystemVersion(10);
            entity = m_Manager.CreateEntity(typeof(EcsTestData), typeof(EcsTestData2), ComponentType.ChunkComponent<EcsTestData3>());

            // These jobs wont actually set the change version so the query will not match any entities so no structural change is executed.
            // Otherwise BeforeStructuralChange will throw an InvalidOperationException when it calls CompleteAllJobs.
            // We want to only check for the InvalidOperationException from SyncFilterTypes.
            job1 = new SetComponentDataJob_EcsTestData {value = new EcsTestData(1)}.Schedule(m_Manager.CreateEntityQuery(typeof(EcsTestData)));
            job2 = new SetComponentDataJob_EcsTestData2 {value = new EcsTestData2(2)}.Schedule(m_Manager.CreateEntityQuery(typeof(EcsTestData2)));

            if(completeJob1)
                job1.Complete();

            if(completeJob2)
                job2.Complete();

            entityQuery = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2));
            entityQuery.SetChangedVersionFilter(new ComponentType[]{ComponentType.ReadWrite<EcsTestData>(), ComponentType.ReadWrite<EcsTestData2>()});
            entityQuery.SetChangedFilterRequiredVersion(10);
        }

        public override void TearDown()
        {
            job1.Complete();
            job2.Complete();

            entityQuery.Dispose();

            base.TearDown();
        }

        void AssertThrowsIfAnyJobNotCompleted(TestDelegate code)
        {
            if(completeJob1 && completeJob2)
                Assert.DoesNotThrow(code);
            else
                Assert.Throws<InvalidOperationException>(code);
        }

        public struct SetComponentDataJob_EcsTestData : IJobForEach<EcsTestData>
        {
            public EcsTestData value;
            public void Execute(ref EcsTestData component) => component = value;
        }

        public struct SetComponentDataJob_EcsTestData2 : IJobForEach<EcsTestData2>
        {
            public EcsTestData2 value;
            public void Execute(ref EcsTestData2 component) => component = value;
        }

        [Test]
        public void CommandBuffer_AddComponentWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            using (EntityCommandBuffer cmds = new EntityCommandBuffer(Allocator.TempJob))
            {
                cmds.AddComponent(entityQuery, typeof(EcsTestData3));
                AssertThrowsIfAnyJobNotCompleted(() => cmds.Playback(m_Manager));
            }
        }

        [Test]
        public void CommandBuffer_RemoveComponentWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            using (EntityCommandBuffer cmds = new EntityCommandBuffer(Allocator.TempJob))
            {
                cmds.RemoveComponent(entityQuery, typeof(EcsTestData));
                AssertThrowsIfAnyJobNotCompleted(() => cmds.Playback(m_Manager));
            }
        }

        [Test]
        public void CommandBuffer_DestroyEntityWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            using (EntityCommandBuffer cmds = new EntityCommandBuffer(Allocator.TempJob))
            {
                cmds.DestroyEntity(entityQuery);
                AssertThrowsIfAnyJobNotCompleted(() => cmds.Playback(m_Manager));
            }
        }

        [Test]
        public void CommandBuffer_AddSharedComponentWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            using (EntityCommandBuffer cmds = new EntityCommandBuffer(Allocator.TempJob))
            {
                cmds.AddSharedComponent(entityQuery, new EcsTestSharedComp(7));
                AssertThrowsIfAnyJobNotCompleted(() => cmds.Playback(m_Manager));
            }
        }

        [Test]
        public void EntityManager_RemoveComponentWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            AssertThrowsIfAnyJobNotCompleted(() =>m_Manager.RemoveComponent(entityQuery, ComponentType.ReadWrite<EcsTestData>()));
        }

        [Test]
        public void EntityManager_AddComponentWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            AssertThrowsIfAnyJobNotCompleted(() =>m_Manager.AddComponent(entityQuery, ComponentType.ReadWrite<EcsTestData3>()));
        }

        [Test]
        public void EntityManager_AddChunkComponentDataWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            AssertThrowsIfAnyJobNotCompleted(() =>m_Manager.AddChunkComponentData(entityQuery, new EcsTestData3(7)));
        }

        [Test]
        public void EntityManager_RemoveChunkComponentDataWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            AssertThrowsIfAnyJobNotCompleted(() =>m_Manager.RemoveChunkComponentData<EcsTestData3>(entityQuery));
        }

        [Test]
        public void EntityManager_AddSharedComponentDataWithEntityQuery_Syncs_ChangeFilterTypes()
        {
            AssertThrowsIfAnyJobNotCompleted(() =>m_Manager.AddSharedComponentData(entityQuery, new EcsTestSharedComp(7)));
        }
    }
}
