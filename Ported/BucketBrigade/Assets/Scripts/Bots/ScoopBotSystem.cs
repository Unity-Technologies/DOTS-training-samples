using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[UpdateAfter(typeof(BrigadeRetargetSystem))]
public class ScoopBotSystem : SystemBase
{
    private EntityQuery m_bucketQuery;
    private EntityQuery m_scoopBucketSearchQuery;
    protected override void OnCreate()
    {
        m_bucketQuery = GetEntityQuery(new EntityQueryDesc
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
        var deltaTime = Time.DeltaTime;
        if (m_scoopBucketSearchQuery.CalculateEntityCount() > 0 && m_bucketQuery.CalculateEntityCount() > 0)
        {
            Entities
                .WithName("Scoop_BucketSearch")
                .WithAll<BotTypeScoop>()
                .WithNone<TargetBucket>()
                .WithNone<CarriedBucket>()
                .WithStructuralChanges()
                .ForEach((Entity e, ref Translation translation) =>
                {
                    // TODO: OMG THIS IS BAD
                    var bucketTranslations = m_bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
                    var bucketEntities = m_bucketQuery.ToEntityArray(Allocator.TempJob);

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
                    
                    bucketTranslations.Dispose();
                    bucketEntities.Dispose();
                }).Run();
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
                translation.Value = translation.Value +
                                    math.normalize(bucketTranslation.Value - translation.Value) * 1 * deltaTime;
                if (math.length(translation.Value - bucketTranslation.Value) < 0.1f)
                {
                    EntityManager.AddComponentData(e, new CarriedBucket()
                    {
                        Value = targetBucket.Value
                    });
                    EntityManager.RemoveComponent<TargetBucket>(e);
                }
            }).WithoutBurst().Run();
        var riverTranslation = GetComponentDataFromEntity<Translation>(true);

        Entities
            .WithName("Scoop_BucketCarry")
            .WithAll<BotTypeScoop>()
            .WithAll<CarriedBucket>()
            .WithStructuralChanges()
            .ForEach((Entity e, ref Translation translation, in CarriedBucket carriedBucket, in BrigadeGroup brigade,
                in NextBot nextBot) =>
            {
                var waterEntity = EntityManager.GetComponentData<Brigade>(brigade.Value).waterEntity;
                var brigadeTarget = math.mul(GetComponent<LocalToWorld>(waterEntity).Value,new float4(riverTranslation[waterEntity].Value, 1)).xyz;
                float3 toTarget = brigadeTarget - translation.Value;
                
                float distanceToTarget = math.length(toTarget);
                if (distanceToTarget < float.Epsilon)
                {
                    // we got to the target, mark the bucket as full, and pass to the next owner
                    // eventually this will transition to a fill state first
                    EntityManager.SetComponentData(carriedBucket.Value, new Water()
                    {
                        capacity = 1,
                        volume = 1
                    });
                    EntityManager.AddComponentData(nextBot.Value, new TargetBucket()
                    {
                        Value = carriedBucket.Value
                    });
                    EntityManager.SetComponentData(carriedBucket.Value, new Owner()
                    {
                        Value = nextBot.Value
                    });
                    EntityManager.RemoveComponent<CarriedBucket>(e);
                }
                else
                {
                    translation.Value = translation.Value +
                                        math.normalize(toTarget) * math.min(1 * deltaTime, distanceToTarget);
                    var bucketTranslation = translation.Value + new float3(0, 0.5f, 0);
                    EntityManager.SetComponentData(carriedBucket.Value, new Translation() {Value = bucketTranslation});
                }
            }).WithoutBurst().Run();
    }
}

