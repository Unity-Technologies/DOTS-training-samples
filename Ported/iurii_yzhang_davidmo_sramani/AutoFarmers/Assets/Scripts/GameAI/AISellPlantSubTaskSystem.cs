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
            var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            var job = Entities
                .WithAll<AITaskTagDeliver>()
                .WithNone<AISubTaskTagFindPlant>()
                .WithNone<AISubTaskTagFindShop>()
                .WithNone<AISubTaskSellPlant>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.AddComponent<AISubTaskTagFindPlant>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job2 = Entities
                .WithAll<AITaskTagDeliver>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindPlant>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagFindPlant>(entityInQueryIndex, entity);
                    ecb.AddComponent<AISubTaskTagFindShop>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            var job3 = Entities
                .WithAll<AITaskTagDeliver>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindShop>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagFindShop>(entityInQueryIndex, entity);
                    ecb.AddComponent<AISubTaskSellPlant>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            var job4 = Entities
                .WithAll<AITaskTagDeliver>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskSellPlant>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskSellPlant>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AITaskTagDeliver>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job4);
            return inputDeps;
        }
    }
}