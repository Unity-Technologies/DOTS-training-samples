using FireBrigade.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Water;

namespace FireBrigade.Systems
{
    public class BucketCollectorSystem : SystemBase
    {
        private EntityQuery bucketQuery;
        private EntityCommandBufferSystem m_ECBSystem;

        protected override void OnCreate()
        {
            bucketQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(BucketTag));
            m_ECBSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var buckets = bucketQuery.ToEntityArray(Allocator.TempJob);
            var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
            var ltwLookup = GetComponentDataFromEntity<LocalToWorld>(true);
            var fillLookup = GetComponentDataFromEntity<FillAmount>(true);
            // Move bucket collectors towards a bucket
            Entities
                .WithDeallocateOnJobCompletion(buckets)
                .WithReadOnly(ltwLookup).WithReadOnly(fillLookup)
                .WithAll<BucketCollector>().WithNone<HeldBucket>()
                .ForEach((Entity entity, int entityInQueryIndex, ref GoalPosition goalPosition,
                    in Translation translation) =>
                {
                    // Pick closest water to group position
                    var closestDistance = float.MaxValue;
                    var closestIndex = -1;
                    for (int bucketIndex = 0; bucketIndex < buckets.Length; bucketIndex++)
                    {
                        if (HasComponent<Attached>(buckets[bucketIndex])
                            || HasComponent<InUse>(buckets[bucketIndex])) continue;

                        var bucketLtw = ltwLookup[buckets[bucketIndex]];
                        var fill = fillLookup[buckets[bucketIndex]];
                        if (fill.Value > 0) continue;

                        var distance = math.distancesq(bucketLtw.Position,
                            translation.Value);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestIndex = bucketIndex;
                        }
                    }

                    if (closestIndex < 0) return;
                    var bucketPosition = ltwLookup[buckets[closestIndex]].Position;
                    bucketPosition.y = 0f;

                    goalPosition.Value = bucketPosition;
                    ecb.AddComponent(entityInQueryIndex, entity,
                        new BucketTarget {entity = buckets[closestIndex], Position = bucketPosition});
                }).ScheduleParallel();

            // Pick up water bucket if at its position
            Entities
                .ForEach((Entity entity, int entityInQueryIndex, ref GoalPosition goalPosition,
                    in Translation translation, in BucketTarget bucketTarget, in WaterTarget waterTarget, in FireTarget fireTarget) =>
                {
                    if ((math.distance(bucketTarget.Position, translation.Value) > 0.1f)) return;

                    var heldBucket = new HeldBucket {Value = bucketTarget.entity};
                    ecb.AddComponent(entityInQueryIndex, entity, heldBucket);
                    ecb.AddComponent(entityInQueryIndex, bucketTarget.entity,
                        new Attached {Value = entity, Offset = new float3(0, 0.5f, 0)});
                    ecb.RemoveComponent<BucketTarget>(entityInQueryIndex, entity);
                    goalPosition.Value = waterTarget.Position;
                    var bucketFireTarget = new FireTarget {entity = fireTarget.entity, Position = fireTarget.Position};
                    ecb.AddComponent(entityInQueryIndex, bucketTarget.entity, bucketFireTarget);
                    ecb.RemoveComponent<BucketTarget>(entityInQueryIndex, entity);

                }).ScheduleParallel();
            
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}