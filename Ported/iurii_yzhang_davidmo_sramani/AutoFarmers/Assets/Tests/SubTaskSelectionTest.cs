using GameAI;
using NUnit.Framework;
using Unity.Entities;
using Unity.Entities.Tests;

namespace Tests
{
    public class SubTaskSelectionTest : ECSTestsFixture
    {
        [Test]
        public void TillGroundFindTile()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskTill));

            var tillGroundSystem = World.GetOrCreateSystem<AITillGroundSubTaskSystem>();
            Assert.IsTrue(tillGroundSystem.Enabled && tillGroundSystem.ShouldRunSystem());
            tillGroundSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagComplete>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AISubTaskTagFindUntilledTile>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagTillGroundTile>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagPlantSeed>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskTill>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }

        [Test]
        public void TillGroundTillTile()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskTill), typeof(AISubTaskTagFindUntilledTile), typeof(AISubTaskTagComplete));

            var tillGroundSystem = World.GetOrCreateSystem<AITillGroundSubTaskSystem>();
            Assert.IsTrue(tillGroundSystem.Enabled && tillGroundSystem.ShouldRunSystem());
            tillGroundSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagComplete>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindUntilledTile>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AISubTaskTagTillGroundTile>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagPlantSeed>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskTill>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }

        [Test]
        public void TillGroundPlantSeed()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskTill), typeof(AISubTaskTagTillGroundTile), typeof(AISubTaskTagComplete));

            var tillGroundSystem = World.GetOrCreateSystem<AITillGroundSubTaskSystem>();
            Assert.IsTrue(tillGroundSystem.Enabled && tillGroundSystem.ShouldRunSystem());
            tillGroundSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagComplete>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindUntilledTile>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagTillGroundTile>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AISubTaskTagPlantSeed>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskTill>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }
        
        [Test]
        public void TillGroundComplete()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskTill), typeof(AISubTaskTagPlantSeed), typeof(AISubTaskTagComplete));

            var tillGroundSystem = World.GetOrCreateSystem<AITillGroundSubTaskSystem>();
            Assert.IsTrue(tillGroundSystem.Enabled && tillGroundSystem.ShouldRunSystem());
            tillGroundSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagComplete>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindUntilledTile>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagTillGroundTile>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagPlantSeed>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskTill>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }


        [Test]
        public void ClearRockFindRock()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskClearRock));

            var clearRockSystem = World.GetOrCreateSystem<AIClearRockSubTaskSystem>();
            Assert.IsTrue(clearRockSystem.Enabled && clearRockSystem.ShouldRunSystem());
            clearRockSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagComplete>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AISubTaskTagFindRock>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagClearRock>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskClearRock>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }

        [Test]
        public void ClearRockDestroyRock()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskClearRock), typeof(AISubTaskTagFindRock), typeof(AISubTaskTagComplete));

            var clearRockSystem = World.GetOrCreateSystem<AIClearRockSubTaskSystem>();
            Assert.IsTrue(clearRockSystem.Enabled && clearRockSystem.ShouldRunSystem());
            clearRockSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagComplete>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindRock>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AISubTaskTagClearRock>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskClearRock>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }

        [Test]
        public void ClearRockComplete()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskClearRock), typeof(AISubTaskTagClearRock), typeof(AISubTaskTagComplete));

            var clearRockSystem = World.GetOrCreateSystem<AIClearRockSubTaskSystem>();
            Assert.IsTrue(clearRockSystem.Enabled && clearRockSystem.ShouldRunSystem());
            clearRockSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagComplete>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindRock>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagClearRock>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskClearRock>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }

        [Test]
        public void SellPlantFindPlant()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskDeliver));

            var sellPlantSystem = World.GetOrCreateSystem<AISellPlantkSubTaskSystem>();
            Assert.IsTrue(sellPlantSystem.Enabled && sellPlantSystem.ShouldRunSystem());
            sellPlantSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsTrue(World.EntityManager.HasComponent<AISubTaskTagFindPlant>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindShop>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskSellPlant>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskDeliver>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }

        [Test]
        public void SellPlantFindShop()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskDeliver), typeof(AISubTaskTagFindPlant), typeof(AISubTaskTagComplete));

            var sellPlantSystem = World.GetOrCreateSystem<AISellPlantkSubTaskSystem>();
            Assert.IsTrue(sellPlantSystem.Enabled && sellPlantSystem.ShouldRunSystem());
            sellPlantSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindPlant>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AISubTaskTagFindShop>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskSellPlant>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskDeliver>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }

        [Test]
        public void SellPlantSellPlant()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskDeliver),  typeof(AISubTaskTagFindShop), typeof(AISubTaskTagComplete));

            var sellPlantSystem = World.GetOrCreateSystem<AISellPlantkSubTaskSystem>();
            Assert.IsTrue(sellPlantSystem.Enabled && sellPlantSystem.ShouldRunSystem());
            sellPlantSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindPlant>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindShop>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AISubTaskSellPlant>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskDeliver>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }

        [Test]
        public void SellPlantComplete()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskDeliver),  typeof(AISubTaskSellPlant), typeof(AISubTaskTagComplete));

            var sellPlantSystem = World.GetOrCreateSystem<AISellPlantkSubTaskSystem>();
            Assert.IsTrue(sellPlantSystem.Enabled && sellPlantSystem.ShouldRunSystem());
            sellPlantSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindPlant>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskTagFindShop>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AISubTaskSellPlant>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskDeliver>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskNone>(e));
        }
    }
}