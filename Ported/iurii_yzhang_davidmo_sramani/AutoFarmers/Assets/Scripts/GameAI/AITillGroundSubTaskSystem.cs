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
                .WithNone<AISubTaskTagFindUntilledTile>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagFindUntilledTile>(entityInQueryIndex, entity);
                    ecb.AddComponent<AISubTaskTagTillGroundTile>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job3 = Entities
                .WithAll<AITaskTagTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithNone<AISubTaskTagTillGroundTile>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagTillGroundTile>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AITaskTagTill>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            return inputDeps;
        }
    }
}