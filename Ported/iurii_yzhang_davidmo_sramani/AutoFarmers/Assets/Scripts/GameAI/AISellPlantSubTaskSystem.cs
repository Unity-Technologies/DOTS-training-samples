using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameAI
{
    public class AISellPlantkSubTaskSystem : JobComponentSystem
    {
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var ecb1 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var ecb3 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var ecb4 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            var job = Entities
                .WithAll<AITagTaskDeliver>()
                .WithNone<AISubTaskTagFindPlant>()
                .WithNone<AISubTaskTagFindShop>()
                .WithNone<AISubTaskSellPlant>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb1.AddComponent<AISubTaskTagFindPlant>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job2 = Entities
                .WithAll<AITagTaskDeliver>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindPlant>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb2.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb2.RemoveComponent<AISubTaskTagFindPlant>(entityInQueryIndex, entity);
                    ecb2.AddComponent<AISubTaskTagFindShop>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            var job3 = Entities
                .WithAll<AITagTaskDeliver>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindShop>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb3.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb3.RemoveComponent<AISubTaskTagFindShop>(entityInQueryIndex, entity);
                    ecb3.AddComponent<AISubTaskSellPlant>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            var job4 = Entities
                .WithAll<AITagTaskDeliver>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskSellPlant>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb4.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb4.RemoveComponent<AISubTaskSellPlant>(entityInQueryIndex, entity);
                    ecb4.RemoveComponent<AITagTaskDeliver>(entityInQueryIndex, entity);
                    ecb4.AddComponent<AITagTaskNone>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job4);

            return JobHandle.CombineDependencies(JobHandle.CombineDependencies(job, job2), JobHandle.CombineDependencies(job3, job4));
        }
    }
}