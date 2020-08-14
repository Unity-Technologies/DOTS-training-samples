using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ScoopBotFilling : IComponentData {}

[UpdateAfter(typeof(BrigadeRetargetSystem))]
public class ScoopBotSystem : SystemBase
{
    private EntityQuery m_unownedBucketQuery;
    private EntityQuery m_scoopBucketSearchQuery;
    private EndSimulationEntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        m_unownedBucketQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Bucket>(),
                ComponentType.ReadOnly<Translation>(),
            },
            None = new []
            {
                ComponentType.ReadOnly<Owner>()
            }
        });
        m_scoopBucketSearchQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<BotTypeScoop>(),
            },
            None = new []
            {
                ComponentType.ReadOnly<TargetBucket>(),
                ComponentType.ReadOnly<CarriedBucket>()
            }
        });
    }

    protected override void OnUpdate()
    {
        
        var botSpeed = EntityManager.GetComponentData<BotConfig>(GetSingletonEntity<BotConfig>()).botSpeed;
        var deltaTime = Time.DeltaTime;
        if (m_scoopBucketSearchQuery.CalculateEntityCount() > 0 && m_unownedBucketQuery.CalculateEntityCount() > 0)
        {
            // TODO: OMG THIS IS BAD
            var bucketTranslations = m_unownedBucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var bucketEntities = m_unownedBucketQuery.ToEntityArray(Allocator.TempJob);

            Entities
                .WithName("Scoop_BucketSearch")
                .WithAll<BotTypeScoop>()
                .WithNone<TargetBucket>()
                .WithNone<CarriedBucket>()
                .WithStructuralChanges()
                .ForEach((Entity e, ref Translation translation) =>
                {
                    float minDistance = math.lengthsq(bucketTranslations[0].Value - translation.Value);
                    Entity bucket = bucketEntities[0];
                    // find a target bucket
                    for (int i = 1; i < bucketTranslations.Length; i++)
                    {
                        float distance = math.lengthsq(bucketTranslations[i].Value - translation.Value);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            bucket = bucketEntities[i];
                        }
                    }

                    EntityManager.AddComponentData(bucket, new Owner()
                    {
                        Value = e
                    });
                    EntityManager.AddComponentData(e, new TargetBucket()
                    {
                        Value = bucket
                    });
                    
                }).Run();

            bucketTranslations.Dispose();
            bucketEntities.Dispose();
        }
        Entities
            .WithName("Scoop_BucketPickup")
            .WithAll<BotTypeScoop>()
            .WithAll<TargetBucket>()
            .WithNone<CarriedBucket>()
            .WithStructuralChanges()
            .ForEach((Entity e, ref Translation translation, in TargetBucket targetBucket) =>
            {
                var bucketTranslation = EntityManager.GetComponentData<Translation>(targetBucket.Value);
                translation.Value = UtilityFunctions.BotHeightCorrect(translation.Value +
                                    math.normalize(bucketTranslation.Value - translation.Value) * botSpeed * deltaTime);
                if (UtilityFunctions.FlatOverlapCheck(translation.Value, bucketTranslation.Value))
                {
                    EntityManager.AddComponentData(e, new CarriedBucket()
                    {
                        Value = targetBucket.Value
                    });
                    EntityManager.RemoveComponent<TargetBucket>(e);
                }
            }).WithoutBurst().Run();

        var riverTranslation = GetComponentDataFromEntity<Translation>(true);
        var localToWorldComponents = GetComponentDataFromEntity<LocalToWorld>(true);

        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
            
        Entities
            .WithName("Scoop_BucketCarry")
            .WithAll<BotTypeScoop>()
            .WithAll<CarriedBucket>()
            .WithNone<ScoopBotFilling>()
            .WithStructuralChanges()
            .ForEach((Entity e, ref Translation translation, in CarriedBucket carriedBucket, in BrigadeGroup brigade,
                in NextBot nextBot) =>
            {
                var waterEntity = EntityManager.GetComponentData<Brigade>(brigade.Value).waterEntity;
                var brigadeTarget = localToWorldComponents[waterEntity].Value.c3.xyz;
                float3 toTarget = brigadeTarget - translation.Value;
                
                float distanceToTarget = math.length(toTarget);
                if (UtilityFunctions.FlatOverlapCheck(brigadeTarget, translation.Value))
                {
                    ecb.AddComponent<ScoopBotFilling>(e);
                }
                else
                {
                    translation.Value = UtilityFunctions.BotHeightCorrect(translation.Value +
                                        math.normalize(toTarget) * math.min(botSpeed * deltaTime, distanceToTarget));
                }
            }).WithoutBurst().Run();
        
        Entities
            .WithName("Scoop_BucketFill")
            .WithAll<BotTypeScoop>()
            .WithAll<CarriedBucket>()
            .WithAll<ScoopBotFilling>()
            .WithStructuralChanges()
            .ForEach((Entity e, ref Translation translation, in CarriedBucket carriedBucket, in BrigadeGroup brigade,
                in NextBot nextBot) =>
            {
                var waterEntity = EntityManager.GetComponentData<Brigade>(brigade.Value).waterEntity;
                var brigadeTarget = localToWorldComponents[waterEntity].Value.c3.xyz;
                float3 toTarget = brigadeTarget - translation.Value;

                float distanceToTarget = math.length(toTarget);
                var water = GetComponent<Water>(carriedBucket.Value);
                var newVolume = math.min(water.volume+deltaTime, 1);
                ecb.SetComponent(carriedBucket.Value, new Water()
                {
                    capacity = water.capacity,
                    volume = newVolume
                });
                
                if(newVolume == water.capacity)
                {
                    ecb.RemoveComponent<ScoopBotFilling>(e);
                    // we got to the target, mark the bucket as full, and pass to the next owner
                    // eventually this will transition to a fill state first
                    ecb.AddComponent(nextBot.Value, new TargetBucket()
                    {
                        Value = carriedBucket.Value
                    });
                    ecb.SetComponent(carriedBucket.Value, new Owner()
                    {
                        Value = nextBot.Value
                    });
                    ecb.RemoveComponent<CarriedBucket>(e);
                }
            }).WithoutBurst().Run();
    }
}

