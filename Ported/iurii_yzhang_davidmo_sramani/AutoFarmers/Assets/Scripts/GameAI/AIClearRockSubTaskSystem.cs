using Pathfinding;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

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

            var hashMap = World.GetOrCreateSystem<WorldCreatorSystem>().hashMap;

            var job2 = Entities
                .WithAll<AITagTaskClearRock>()
                .WithAll<AISubTaskTagFindRock>()
                .WithReadOnly(hashMap)
//                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex, in TilePositionable position, in AISubTaskTagComplete target) =>
                {
                    ecb2.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb2.RemoveComponent<AISubTaskTagFindRock>(entityInQueryIndex, entity);

                    Entity rockEntity;
                    var has = hashMap.TryGetValue(target.targetPos, out rockEntity);
                    //Debug.Log($"hashMap has entity {stonePosition.entity} at {position.Position}");
                    
                    ecb2.AddComponent(entityInQueryIndex, entity, new AISubTaskTagClearRock() { rockEntity = rockEntity });
                }).Schedule(inputDeps);
            
            var job3 = Entities
                .WithAll<AITagTaskClearRock>()
                .WithAll<AISubTaskTagComplete>()
                .WithAll<AISubTaskTagClearRock>()
                .ForEach((Entity entity, int entityInQueryIndex) =>
                {
                    ecb3.RemoveComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    ecb3.RemoveComponent<AISubTaskTagClearRock>(entityInQueryIndex, entity);
                    ecb3.RemoveComponent<AITagTaskClearRock>(entityInQueryIndex, entity);
                    ecb3.AddComponent<AITagTaskNone>(entityInQueryIndex, entity);
                }).Schedule(inputDeps);

            var jobQuery = GetEntityQuery(typeof(AISubTaskTagClearRock), typeof(AISubTaskTagComplete), typeof(AITagTaskClearRock));
            if (jobQuery.CalculateEntityCount() > 0) {
                World.GetOrCreateSystem<PathfindingSystem>().PlantOrStoneChanged();
            }
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            return JobHandle.CombineDependencies(job, job2, job3);
        }
    }
}