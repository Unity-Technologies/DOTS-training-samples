using Unity.Entities;
using Unity.Jobs;

namespace GameAI
{
    public class AITillGroundSubTaskSystem : JobComponentSystem
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
                .WithAll<AITagTaskTill>()
                .WithNone<AISubTaskTagFindUntilledTile>()
                .WithNone<AISubTaskTagTillGroundTile>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb1.AddComponent<AISubTaskTagFindUntilledTile>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job2 = Entities
                .WithAll<AITagTaskTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindUntilledTile>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb2.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb2.RemoveComponent<AISubTaskTagFindUntilledTile>(entityInQueryIndex, entity);
                    ecb2.AddComponent<AISubTaskTagTillGroundTile>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            var job3 = Entities
                .WithAll<AITagTaskTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagTillGroundTile>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb3.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb3.RemoveComponent<AISubTaskTagTillGroundTile>(entityInQueryIndex, entity);
                    ecb3.AddComponent<AISubTaskTagPlantSeed>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job4 = Entities
                .WithAll<AITagTaskTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagPlantSeed>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb4.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb4.RemoveComponent<AISubTaskTagPlantSeed>(entityInQueryIndex, entity);
                    ecb4.RemoveComponent<AITagTaskTill>(entityInQueryIndex, entity);
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