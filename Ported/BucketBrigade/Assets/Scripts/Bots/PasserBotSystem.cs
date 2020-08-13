using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PasserBotSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        // Go to default position if no pickup/carrying happening
        Entities
            .WithName("Passer_GoToTarget")
            .WithAny<EmptyPasserInfo, FullPasserInfo>()
            .WithNone<CarriedBucket>()
            .WithNone<TargetBucket>()
            .ForEach((Entity e, ref Translation translation, in TargetPosition target) =>
                {
                    if (math.length(translation.Value - target.Value) >= 0.1f)
                    {
                        translation.Value = translation.Value + 
                                            math.normalize(target.Value - translation.Value) * 1 * deltaTime;
                    }
                }
            ).Run();
        
        // Go to target bucket
        Entities
            .WithName("Passer_BucketPickup")
            .WithAny<EmptyPasserInfo, FullPasserInfo>()
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
                        // Start carrying bucket
                        EntityManager.AddComponentData(e, new CarriedBucket()
                        {
                            Value = targetBucket.Value
                        });
                        EntityManager.RemoveComponent<TargetBucket>(e);
                    }
                }
            ).Run();

        // Carry & Deliver bucket
        Entities
            .WithName("Passer_BucketCarryToTarget")
            .WithAny<EmptyPasserInfo, FullPasserInfo>()
            .WithAll<CarriedBucket>()
            .WithStructuralChanges()
            .ForEach((Entity e, ref Translation translation, in CarriedBucket carriedBucket, in BrigadeGroup brigade, in TargetPosition targetPos, in NextBot nextBot) =>
                {
                    translation.Value = translation.Value + math.normalize(targetPos.Value - translation.Value) * 1 * deltaTime;
                    var bucketTranslation = translation.Value + new float3(0, 0.5f, 0);
                    EntityManager.SetComponentData(carriedBucket.Value, new Translation(){Value = bucketTranslation});

                    if (math.length(translation.Value - targetPos.Value) < 0.1f)
                    {
                        // Next bot targets bucket
                        EntityManager.AddComponentData(nextBot.Value, new TargetBucket
                        {
                            Value = carriedBucket.Value
                        });
                        
                        // Next bot owns bucket 
                        EntityManager.AddComponentData(carriedBucket.Value, new Owner
                        {
                            Value = nextBot.Value
                        });
                        
                        EntityManager.RemoveComponent<CarriedBucket>(e);
                    }
                }
            ).Run();
        
        var translations = GetComponentDataFromEntity<Translation>(true);
        
        // Carry & return empty bucket to water (special case for last empty passer, no next bot, just drop off at water)
        Entities
            .WithName("Passer_BucketCarryToWater")
            .WithAll<EmptyPasserInfo>()
            .WithAll<CarriedBucket>()
            .WithNone<NextBot>()  // No next but, unlike job above going "ToTarget"
            .WithStructuralChanges()
            .ForEach((Entity e, ref Translation translation, in CarriedBucket carriedBucket, in BrigadeGroup brigadeGroup, in TargetPosition targetPos) =>
                {
                    Brigade brigade = GetComponent<Brigade>(brigadeGroup.Value);
                    Translation waterTranslation = translations[brigade.waterEntity];
                    
                    var waterPosition = math.mul(GetComponent<LocalToWorld>(brigade.waterEntity).Value, new float4(translations[brigade.waterEntity].Value, 1)).xyz;

                    translation.Value = translation.Value + math.normalize(waterPosition - translation.Value) * 1 * deltaTime;
                    var bucketTranslation = translation.Value + new float3(0, 0.5f, 0);
                    EntityManager.SetComponentData(carriedBucket.Value, new Translation(){Value = bucketTranslation});

                    if (math.length(translation.Value - waterPosition) < 0.1f)
                    {
                        // Next bot owns bucket 
                        EntityManager.RemoveComponent<Owner>(carriedBucket.Value);
                        EntityManager.RemoveComponent<CarriedBucket>(e);
                    }
                }
            ).Run();
    }
}
