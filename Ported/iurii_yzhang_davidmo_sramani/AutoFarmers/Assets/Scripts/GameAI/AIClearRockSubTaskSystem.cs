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
            var ecb1 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var ecb3 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            var job = Entities
                .WithAll<AITagTaskClearRock>()
                .WithNone<AISubTaskTagFindRock>()
                .WithNone<AISubTaskTagClearRock>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb1.AddComponent<AISubTaskTagFindRock>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job2 = Entities
                .WithAll<AITagTaskTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagFindRock>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb2.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb2.RemoveComponent<AISubTaskTagFindRock>(entityInQueryIndex, entity);
                    ecb2.AddComponent<AISubTaskTagClearRock>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var job3 = Entities
                .WithAll<AITagTaskTill>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagClearRock>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb3.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb3.RemoveComponent<AISubTaskTagClearRock>(entityInQueryIndex, entity);
                    ecb3.RemoveComponent<AITagTaskClearRock>(entityInQueryIndex, entity);
                    ecb3.AddComponent<AITagTaskClearRock>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            
            return JobHandle.CombineDependencies(job, job2, job3);
        }
    }
}