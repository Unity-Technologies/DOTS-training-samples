using Unity.Entities;
using Unity.Jobs;

namespace GameAI
{
    public class AIClearRockSubTaskSystem : JobComponentSystem
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
                .WithAll<AITagTaskClearRock>()
                .WithNone<AISubTaskTagFindRock>()
                .WithNone<AISubTaskTagClearRock>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.AddComponent<AISubTaskTagFindRock>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job2 = Entities
                .WithAll<AITagTaskTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindRock>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagFindRock>(entityInQueryIndex, entity);
                    ecb.AddComponent<AISubTaskTagClearRock>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job3 = Entities
                .WithAll<AITagTaskTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagClearRock>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AISubTaskTagClearRock>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AITagTaskClearRock>(entityInQueryIndex, entity);
                    ecb.AddComponent<AITagTaskClearRock>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            
            return JobHandle.CombineDependencies(job, job2, job3);
        }
    }
}