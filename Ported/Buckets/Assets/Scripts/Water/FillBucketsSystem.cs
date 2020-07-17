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
            var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
            var deltaTime = Time.DeltaTime;
            // fill buckets attached to a well
            Entities
                .WithNone<BucketFullTag>()
                .ForEach((Entity entity, int entityInQueryIndex, 
                    ref FillAmount fill, ref LocalToWorld transform, 
                    in Attached attachedEntity, in FireTarget fireTarget) =>
                {
                    if (!HasComponent<WellTag>(attachedEntity.Value)) return;

                    fill.Value += 0.1f * deltaTime;
                    // fill.Value = math.max(1f, fill.Value);
                    
                    // Start moving full buckets toward the fire
                    if (fill.Value >= 1f)
                    {
                        ecb.AddComponent(entityInQueryIndex, entity, new BucketFullTag());
                        var speed = new MovementSpeed();
                        speed.Value = 1f;
                        var goal = new GoalPosition();
                        goal.Value = fireTarget.Position;
                        if (HasComponent<MovementSpeed>(entity))
                        {
                            ecb.SetComponent(entityInQueryIndex, entity, speed);
                        }
                        else
                        {
                            ecb.AddComponent(entityInQueryIndex, entity, speed);
                        }

                        if (HasComponent<GoalPosition>(entity))
                        {
                            ecb.SetComponent(entityInQueryIndex, entity, goal);
                        }
                        else
                        {
                            ecb.AddComponent(entityInQueryIndex, entity, goal);
                        }

                        if (HasComponent<Attached>(entity))
                        {
                            ecb.RemoveComponent<Attached>(entityInQueryIndex, entity);
                        }
                    }
                }).ScheduleParallel();

            // if bucket is targeting a fire and has reached the fire, dump the bucket, then move it back to water
            Entities
                .WithAll<BucketFullTag,MovementSpeed>()
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref FillAmount fill, ref GoalPosition goalPosition,
                    in FireTarget fireTarget, in LocalToWorld transform, in WaterTarget waterTarget) =>
                {
                    if (math.distance(transform.Position, fireTarget.Position) > 1f) return;

                    Debug.Log("Extinguish");
                    fill.Value = 0f;
                    ecb.AddComponent(entityInQueryIndex, fireTarget.Value, new ExtinguishAmount {Value = 1f, Propagate = true});
                    ecb.RemoveComponent<BucketFullTag>(entityInQueryIndex, entity);
                    
                    // Set it moving back to the well to refill
                    goalPosition.Value = waterTarget.Position;

                }).ScheduleParallel();
            
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}