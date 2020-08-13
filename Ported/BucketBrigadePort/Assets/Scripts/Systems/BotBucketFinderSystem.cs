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
            .WithName("BotBucketFinderSystem")
            .WithDisposeOnCompletion(bucketEntities)
            .WithNone<TargetPosition>()
            .ForEach((int entityInQueryIndex, Entity entity, in BotRoleFinder botRoleFinder, in Translation translation) =>
            {
                int closestBucket = -1;
                float3 closestBucketLocation = translation.Value;
                float closestBucketMag = float.MaxValue;
                for (int i = 0; i < bucketEntities.Length; i++)
                {
                    var bucketTranslation = GetComponent<Translation>(bucketEntities[i]);
                    var mag = math.distance(bucketTranslation.Value, translation.Value);
                    if (mag < closestBucketMag)
                    {
                        closestBucketMag = mag;
                        closestBucketLocation = bucketTranslation.Value;
                        closestBucket = i;
                    }
                    // Abort loop when one is found to clear race condition if two bots find the same bucket!

                }
                // Validate a change has been made
                if(closestBucket != -1)
                {
                    var targetPosition = new TargetPosition() { Value = closestBucketLocation };
                    ecb.AddComponent<TargetPosition>(entityInQueryIndex, entity);
                    ecb.SetComponent(entityInQueryIndex, entity, targetPosition);
                    ecb.RemoveComponent<BucketAvailable>(entityInQueryIndex, bucketEntities[closestBucket]);
                }
                // Deal with the buckets

            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}