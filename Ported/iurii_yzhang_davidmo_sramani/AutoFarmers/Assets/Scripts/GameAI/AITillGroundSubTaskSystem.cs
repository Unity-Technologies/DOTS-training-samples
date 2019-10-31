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
            var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            var job = Entities
                .WithAll<AITaskTagTill>()
                .WithNone<AISubTaskTagFindUntilledTile>()
                .WithNone<AISubTaskTagTillGroundTile>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.AddComponent<AISubTaskTagFindUntilledTile>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job2 = Entities
                .WithAll<AITaskTagTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindUntilledTile>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagFindUntilledTile>(entityInQueryIndex, entity);
                    ecb.AddComponent<AISubTaskTagTillGroundTile>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            var job3 = Entities
                .WithAll<AITaskTagTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagTillGroundTile>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagTillGroundTile>(entityInQueryIndex, entity);
                    ecb.AddComponent<AISubTaskTagPlantSeed>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job4 = Entities
                .WithAll<AITaskTagTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagPlantSeed>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagPlantSeed>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AITaskTagTill>(entityInQueryIndex, entity);
                    ecb.AddComponent<AITaskTagNone>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job4);
            return inputDeps;
        }
    }
}