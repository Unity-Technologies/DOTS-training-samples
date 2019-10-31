using GameAI;
using NUnit.Framework;
using Unity.Entities.Tests;
using Unity.Mathematics;
using Unity.Entities;

namespace Tests
{
    public class TaskSelectionTest: ECSTestsFixture
    {
        [Test]
        public void SelectTaskForFarmer()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskNone), typeof(FarmerAITag));

            var taskSelectionSystem = World.GetOrCreateSystem<AiTaskSelectSys>();
            Assert.IsTrue(taskSelectionSystem.Enabled && taskSelectionSystem.ShouldRunSystem());
            taskSelectionSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskClearRock>(e) ||
                          World.EntityManager.HasComponent<AITagTaskTill>(e) ||
                          World.EntityManager.HasComponent<AITagTaskPlant>(e) || 
                          World.EntityManager.HasComponent<AITagTaskDeliver>(e));
        }
        
        [Test]
        public void SelectTaskForDrone()
        {
            BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

            var e = World.EntityManager.CreateEntity(typeof(AITagTaskNone));

            var taskSelectionSystem = World.GetOrCreateSystem<AiTaskSelectSys>();
            Assert.IsTrue(taskSelectionSystem.Enabled && taskSelectionSystem.ShouldRunSystem());
            taskSelectionSystem.Update();
            m_EntityCommandBufferSystem.Update();
            m_Manager.CompleteAllJobs();

            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskNone>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskClearRock>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskTill>(e));
            Assert.IsFalse(World.EntityManager.HasComponent<AITagTaskPlant>(e));

            // Drones can only have the deliver task
            Assert.IsTrue(World.EntityManager.HasComponent<AITagTaskDeliver>(e));
        }
    }
}