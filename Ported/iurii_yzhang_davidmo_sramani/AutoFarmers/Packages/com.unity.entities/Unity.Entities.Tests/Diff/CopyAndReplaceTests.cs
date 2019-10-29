using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    [TestFixture]
    sealed class CopyAndReplaceTests : EntityDifferTestFixture
    {
        //@TODO: Test class based components (Currently doesn't work)
        //@TODO: Test number of created / destroyed chunk counts to be what is expected (for perf)
        //@TODO: Handle chunk component versions. It seems likely that they can currently go out of sync between two worlds so systems might not pick up a change. (Currently doesn't work)
        //@TODO: Test for blob data (Manually tested)
        //@TODO: Test that Copy&Replace doesn't modify, add or remove system state components. But also don't change chunk layout. (Currently doesn't work)

        unsafe void CreateTestData(out Entity entity, out Entity metaEntity, int value, int componentChunkValue)
        {
            entity = SrcEntityManager.CreateEntity();
            SrcEntityManager.AddComponentData(entity, new EcsTestData(value));
            SrcEntityManager.AddSharedComponentData(entity, new EcsTestSharedComp(6));
            SrcEntityManager.AddChunkComponentData(SrcEntityManager.UniversalQuery, new EcsTestData2(7));
            
            metaEntity = SrcEntityManager.GetChunk(entity).m_Chunk->metaChunkEntity; 

            Assert.AreEqual(7, SrcEntityManager.GetComponentData<EcsTestData2>(metaEntity).value0);
        }
        
        unsafe void TestValues(Entity entity, Entity  metaEntity, int componentDataValue, int componentChunkValue)
        {
            Assert.AreEqual(componentDataValue, DstEntityManager.GetComponentData<EcsTestData>(entity).value);
            Assert.AreEqual(6, DstEntityManager.GetSharedComponentData<EcsTestSharedComp>(entity).value);
            Assert.AreEqual(componentChunkValue, DstEntityManager.GetChunkComponentData<EcsTestData2>(entity).value0);
            
            Assert.AreEqual(metaEntity, DstEntityManager.GetChunk(entity).m_Chunk->metaChunkEntity);
            
            SrcEntityManager.Debug.CheckInternalConsistency();
            DstEntityManager.Debug.CheckInternalConsistency();
        }
        
        [Test]
        public void ReplaceEntityManagerContents([Values]bool createToReplaceEntity)
        {
            CreateTestData(out var entity, out var metaEntity, 5, 7);

            if (createToReplaceEntity)
                DstEntityManager.CreateEntity(typeof(EcsTestData), typeof(EcsTestSharedComp));

            DstEntityManager.CopyAndReplaceEntitiesFrom(SrcEntityManager);

            Assert.AreEqual(1, SrcEntityManager.UniversalQuery.CalculateEntityCount());
            Assert.AreEqual(1, DstEntityManager.UniversalQuery.CalculateEntityCount());
            
            TestValues(entity, metaEntity, 5, 7);
        }

        [Test]
        public void ReplaceChangedEntities()
        {
            CreateTestData(out var entity, out var metaEntity, 5, 7);
            DstEntityManager.CopyAndReplaceEntitiesFrom(SrcEntityManager);
            
            SrcEntityManager.SetComponentData(entity, new EcsTestData(11));
            DstEntityManager.CopyAndReplaceEntitiesFrom(SrcEntityManager);
            TestValues(entity, metaEntity, 11, 7);
        }
        
        [Test]
        public void ReplaceChangedChunkComponent()
        {
            CreateTestData(out var entity, out var metaEntity, 5, 7);
            DstEntityManager.CopyAndReplaceEntitiesFrom(SrcEntityManager);
            
            SrcEntityManager.SetComponentData(metaEntity, new EcsTestData2(11));
            DstEntityManager.CopyAndReplaceEntitiesFrom(SrcEntityManager);

            TestValues(entity, metaEntity, 5, 11);
        }
        
        [Test]
        public void ReplaceChangedNothing()
        {
            CreateTestData(out var entity, out var metaEntity, 5, 7);
            DstEntityManager.CopyAndReplaceEntitiesFrom(SrcEntityManager);

            DstEntityManager.CopyAndReplaceEntitiesFrom(SrcEntityManager);
            TestValues(entity, metaEntity, 5, 7);
        }
    }
}