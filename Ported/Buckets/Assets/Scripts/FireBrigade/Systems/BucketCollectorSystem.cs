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
            var ecb = m_ECBSystem.CreateCommandBuffer();
            // Move bucket collectors towards a bucket
            Entities
                .WithDeallocateOnJobCompletion(buckets)
                .WithAll<BucketCollector>().WithNone<HeldBucket>()
                .ForEach((Entity entity, ref GoalPosition goalPosition, in Translation translation) =>
                {
                    // Pick closest water to group position
                    var closestDistance = float.MaxValue;
                    var closestIndex = -1;
                    for (int wellIndex = 0; wellIndex < buckets.Length; wellIndex++)
                    {
                        var distance = math.distancesq(GetComponent<LocalToWorld>(buckets[wellIndex]).Position,
                            translation.Value);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestIndex = wellIndex;
                        }
                    }
                    var bucketPosition = GetComponent<LocalToWorld>(buckets[closestIndex]).Position;
                    bucketPosition.y = 0f;

                    goalPosition.Value = bucketPosition;
                    ecb.AddComponent(entity, new BucketTarget{entity = buckets[closestIndex], Position = bucketPosition});
                }).Schedule();

            // Pick up water bucket if at its position
            var translationLookup = GetComponentDataFromEntity<Translation>(false);
            Entities.WithNativeDisableContainerSafetyRestriction(translationLookup)
                .ForEach((Entity entity, in Translation translation, in BucketTarget bucketTarget) =>
                {
                    if ((math.distance(bucketTarget.Position, translation.Value) > 0.1f)) return;
                    
                    var heldBucket = new HeldBucket();
                    heldBucket.Value = bucketTarget.entity;
                    ecb.AddComponent(entity, heldBucket);
                    ecb.RemoveComponent<BucketTarget>(entity);
                    var bucketTranslation = translationLookup[bucketTarget.entity];
                    bucketTranslation.Value = translation.Value;
                    bucketTranslation.Value.y = 0.5f;
                    translationLookup[bucketTarget.entity] = bucketTranslation;
                }).Schedule();
            
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}