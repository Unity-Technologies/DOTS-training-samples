using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BrigadeRetargetSystem))]
public class BrigadeUpdateSystem : SystemBase
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
        // move the bots towards their targets
        var deltaTime = Time.DeltaTime;
        Entities
            .ForEach((ref Translation translation, in TargetPosition target) =>
            {
                Debug.DrawLine(target.Value, translation.Value);
                Debug.Log(target.Value.ToString());
                if (target.Value.Equals(translation.Value))
                {
                    return;
                }
                translation.Value = translation.Value + math.normalize(target.Value - translation.Value) * 1 * deltaTime;
            }).Schedule();
        
        if (m_scoopBucketSearchQuery.CalculateEntityCount() > 0)
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
                translation.Value = translation.Value + math.normalize(bucketTranslation.Value - translation.Value) * 1 * deltaTime;
                float3 steer = math.normalize(bucketTranslation.Value - translation.Value);
                if (math.length(translation.Value - bucketTranslation.Value) < 0.1f)
                {
                    EntityManager.AddComponentData(e, new CarriedBucket()
                    {
                        Value = targetBucket.Value
                    });
                    EntityManager.RemoveComponent<TargetBucket>(e);
                }
            }).WithoutBurst().Run();
        
        Entities
            .WithName("Scoop_BucketCarry")
            .WithAll<BotTypeScoop>()
            .WithAll<CarriedBucket>()
            .WithStructuralChanges()
            .ForEach((Entity e, ref Translation translation, in CarriedBucket carriedBucket, in BrigadeGroup brigade) =>
            {
                var brigadeTarget = EntityManager.GetComponentData<Brigade>(brigade.Value).waterTarget;
                translation.Value = translation.Value + math.normalize(brigadeTarget - translation.Value) * 1 * deltaTime;
                var bucketTranslation = translation.Value + new float3(0, 0.5f, 0);
                EntityManager.SetComponentData(carriedBucket.Value, new Translation(){Value = bucketTranslation});
            }).WithoutBurst().Run();

        Entities
            .WithName("Scoop_BucketDeliver")
            .WithAll<BotTypeScoop>()
            .WithAll<TargetBucket>()
            .WithNone<CarriedBucket>()
            .ForEach((Entity e, ref Translation translation, in TargetBucket targetBucket) =>
            {
                var bucketTranslation = EntityManager.GetComponentData<Translation>(targetBucket.Value);
                translation.Value = translation.Value + math.normalize(bucketTranslation.Value - translation.Value) * 1 * deltaTime;
            }).WithoutBurst().Run();

    }
}