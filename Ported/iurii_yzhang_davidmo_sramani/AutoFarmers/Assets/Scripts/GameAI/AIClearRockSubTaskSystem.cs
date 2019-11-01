using Pathfinding;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace GameAI
{
    [UpdateAfter(typeof(AIClearRockSubTaskSystem))]
    public class ClearRockEventSystem : ComponentSystem
    {
        private EntityQuery q;
        private WorldCreatorSystem _worldCreatorSystem;
        private PathfindingSystem _pathfinding;
        protected override void OnCreate()
        {
            _worldCreatorSystem = World.GetOrCreateSystem<WorldCreatorSystem>();
            _pathfinding = World.GetOrCreateSystem<PathfindingSystem>();
            q = GetEntityQuery(typeof(AISubTaskTagClearRock), typeof(AISubTaskTagComplete), typeof(AITagTaskClearRock));
            RequireForUpdate(q);
        }
        protected override void OnUpdate()
        {
            var pos = GetComponentDataFromEntity<TilePositionable>(true);
            var stones = GetComponentDataFromEntity<RockComponent>(true);
            var map = _worldCreatorSystem.hashMap;

            Entities
                   .WithAll<AITagTaskClearRock>()
                   .WithAll<AISubTaskTagComplete>()
                   .ForEach((Entity entity, ref AISubTaskTagClearRock subtask) =>
                   {
                       var position = pos[subtask.rockEntity].Position;
                       var size = stones[subtask.rockEntity].Size;

                       for (int _x = position.x; _x < position.x + size.x; _x++)
                           for (int _y = position.y; _y < position.y + size.y; _y++)
                               map.Remove(new int2(_x, _y));

                       EntityManager.DestroyEntity(subtask.rockEntity);

                       EntityManager.RemoveComponent<AISubTaskTagComplete>(entity);
                       EntityManager.RemoveComponent<AISubTaskTagClearRock>(entity);
                       EntityManager.RemoveComponent<AITagTaskClearRock>(entity);
                       EntityManager.AddComponent<AITagTaskNone>(entity);
                   });

            _pathfinding.PlantOrStoneChanged();
        }
    }

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
            
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            return JobHandle.CombineDependencies(job, job2);
        }
    }
}