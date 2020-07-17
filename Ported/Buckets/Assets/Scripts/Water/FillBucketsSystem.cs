using FireBrigade.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Water
{
    public class FillBucketsSystem : SystemBase
    {
        private EntityQuery waterWellQuery;
        private EntityCommandBufferSystem m_ECBSystem;

        protected override void OnCreate()
        {
            waterWellQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(WellTag));
            m_ECBSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        protected override void OnUpdate()
        {
            var ecb = m_ECBSystem.CreateCommandBuffer();
            // fill buckets attached to a well
            // var wells = waterWellQuery.ToEntityArray(Allocator.TempJob);
            Entities
                // .WithDeallocateOnJobCompletion(wells)
                .ForEach((Entity entity, ref FillAmount fill, in Attached attachedEntity, in FireTarget fireTarget) =>
                {
                    if (HasComponent<WellTag>(attachedEntity.Value))
                    {
                        fill.Value += 0.1f;
                        fill.Value = math.max(1f, fill.Value);
                        if (fill.Value >= 1f)
                        {
                            ecb.AddComponent(entity, new BucketFullTag());
                            var speed = new MovementSpeed();
                            speed.Value = 1f;
                            var goal = new GoalPosition();
                            goal.Value = fireTarget.Position;
                            ecb.AddComponent(entity, goal);
                        }
                    }
                }).Schedule();
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}