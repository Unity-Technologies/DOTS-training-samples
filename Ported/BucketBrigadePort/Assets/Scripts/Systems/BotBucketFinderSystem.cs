using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class BotBucketFinderSystem : SystemBase
{
    private EntityQuery m_bucketQuery;
    private EntityCommandBufferSystem m_ECBSystem;
    protected override void OnCreate()
    {
        m_bucketQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<WaterAmount>(),
                ComponentType.ReadWrite<BucketAvailable>()
            },

            None = new[]
            {
                ComponentType.ReadOnly<WaterRefill>()
            }
        });
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        var bucketEntities = m_bucketQuery.ToEntityArray(Allocator.TempJob);
        
        Entities
            .WithName("BotFinderSeekBucketSystem")
            .WithDisposeOnCompletion(bucketEntities)
            .WithNone<TargetPosition>()
            .WithNone<BucketRef>()
            .ForEach((int entityInQueryIndex, Entity entity, in BotRoleFinder botRoleFinder, in Translation translation) =>
            {
                int closestBucket = -1;
                var bucketRef = new BucketRef(); // NB: Need to validate this isn't used unless set
                float3 closestBucketLocation = translation.Value;
                float closestBucketMag = float.MaxValue;
                for (int i = 0; i < bucketEntities.Length; i++)
                {
                    //GetComponentDataFromEntity
                    var bucketTranslation = GetComponent<Translation>(bucketEntities[i]);
                    var mag = math.distance(bucketTranslation.Value, translation.Value);
                    if (mag < closestBucketMag)
                    {
                        closestBucketMag = mag;
                        closestBucketLocation = bucketTranslation.Value;
                        closestBucket = i;
                        bucketRef.Value = bucketEntities[i];
                    }
                    // Abort loop when one is found to clear race condition if two bots find the same bucket!

                }
                // Validate a change has been made
                if(closestBucket != -1)
                {
                    var targetPosition = new TargetPosition() { Value = closestBucketLocation };
                    ecb.AddComponent<TargetPosition>(entityInQueryIndex, entity);
                    ecb.SetComponent(entityInQueryIndex, entity, targetPosition);
                    ecb.AddComponent<BucketRef>(entityInQueryIndex, entity);
                    ecb.SetComponent(entityInQueryIndex, entity, bucketRef);
                }
            }).ScheduleParallel();

        var ecbPickup = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();



        Entities
            .WithName("BotFinderPickupBucketSystem")
            .ForEach((int entityInQueryIndex, Entity entity, ref TargetPosition targetPosition, in BotRoleFinder botRoleFinder, in BucketRef bucketRef, in Translation translation, in DependentEntity dependentEntity) =>
            {
                var bucketTranslation = GetComponent<Translation>(bucketRef.Value);
                var bucketIsAvailable = HasComponent<BucketAvailable>(bucketRef.Value);

                var magnitude = math.distance(bucketTranslation.Value, translation.Value);
                if (magnitude < 0.25f) // TODO: Base this on bucket dimension
                {
                    if (bucketIsAvailable == true)
                    {
                        // Claim the bucket
                        ecbPickup.RemoveComponent<BucketAvailable>(entityInQueryIndex, bucketRef.Value);
                        ecbPickup.AddComponent(entityInQueryIndex, entity, new BucketCarry());
                        // Target the line home
                        var botRootPosition = GetComponent<BotRootPosition>(dependentEntity.Value);
                        targetPosition.Value = botRootPosition.Value;
                    }
                    else
                    {
                        // Someone beat me to the bucket
                        ecbPickup.RemoveComponent<BucketRef>(entityInQueryIndex, entity);
                        ecbPickup.RemoveComponent<TargetPosition>(entityInQueryIndex, entity);
                    }
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);

    }
}