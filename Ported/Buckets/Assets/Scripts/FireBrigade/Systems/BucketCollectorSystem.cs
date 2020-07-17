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
        struct BucketData
        {
            public Entity BucketEntity;
            public bool HasBucket;
            public bool InUse;
            public float3 Position;
            public float FillAmount;
        }

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

            var bucketData = new NativeArray<BucketData>(buckets.Length, Allocator.TempJob);
            for (int i = 0; i < buckets.Length; i++)
            {
                BucketData data = new BucketData
                {
                    BucketEntity = buckets[i],
                    HasBucket = HasComponent<Attached>(buckets[i]),
                    InUse = HasComponent<InUse>(buckets[i]),
                    FillAmount = GetComponent<FillAmount>(buckets[i]).Value,
                    Position = GetComponent<LocalToWorld>(buckets[i]).Position,
                };
                bucketData[i] = data;
            }
            buckets.Dispose();

            // Move bucket collectors towards a bucket
            Entities
                .WithDeallocateOnJobCompletion(bucketData)
                .WithAll<BucketCollector>().WithNone<HeldBucket>()
                .ForEach((Entity entity, ref GoalPosition goalPosition, in Translation translation) =>
                {
                    // Pick closest water to group position
                    var closestDistance = float.MaxValue;
                    float3 closestPosition = new float3();
                    bool foundBucket = false;
                    Entity bucketTarget = Entity.Null;
                    for (int bucketIndex = 0; bucketIndex < buckets.Length; bucketIndex++)
                    {
                        var indexData = bucketData[bucketIndex];
                        if (indexData.HasBucket || indexData.InUse || indexData.FillAmount > 0) continue;
                        
                        var distance = math.distancesq(indexData.Position, translation.Value);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestPosition = indexData.Position;
                            bucketTarget = indexData.BucketEntity;
                            foundBucket = true;
                        }
                    }

                    if (!foundBucket) return;
                    var bucketPosition = closestPosition;
                    bucketPosition.y = 0f;
                    goalPosition.Value = bucketPosition;
                    ecb.AddComponent(entity, new BucketTarget{entity = bucketTarget, Position = bucketPosition});
                }).Schedule();

            // Pick up water bucket if at its position
            // var translationLookup = GetComponentDataFromEntity<Translation>(false);
            Entities
                // .WithNativeDisableContainerSafetyRestriction(translationLookup)
                .ForEach((Entity entity, ref GoalPosition goalPosition,
                    in Translation translation, in BucketTarget bucketTarget, in WaterTarget waterTarget) =>
                {
                    if ((math.distance(bucketTarget.Position, translation.Value) > 0.1f)) return;
                    
                    var heldBucket = new HeldBucket();
                    heldBucket.Value = bucketTarget.entity;
                    ecb.AddComponent(entity, heldBucket);
                    ecb.RemoveComponent<BucketTarget>(entity);
                    // var bucketTranslation = translationLookup[bucketTarget.entity];
                    // bucketTranslation.Value = translation.Value;
                    // bucketTranslation.Value.y = 0.5f;
                    // translationLookup[bucketTarget.entity] = bucketTranslation;
                    ecb.AddComponent(bucketTarget.entity,
                        new Attached {Value = entity, Offset = new float3(0, 0.5f, 0)});
                    ecb.RemoveComponent<BucketTarget>(entity);
                    // ecb.RemoveComponent<HeldBucket>(entity);
                    goalPosition.Value = waterTarget.Position;

                }).Schedule();
            
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}