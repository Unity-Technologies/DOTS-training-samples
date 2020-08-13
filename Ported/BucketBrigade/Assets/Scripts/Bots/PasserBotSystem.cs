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
                    if (!UtilityFunctions.FlatOverlapCheck(translation.Value, target.Value))
                    {
                        translation.Value = UtilityFunctions.BotHeightCorrect(translation.Value + 
                                                                              math.normalize(target.Value - translation.Value) * 1 * deltaTime);
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
                    translation.Value = UtilityFunctions.BotHeightCorrect(translation.Value +
                                                                          math.normalize(bucketTranslation.Value - translation.Value) * 1 * deltaTime);
                    if (UtilityFunctions.FlatOverlapCheck(translation.Value, bucketTranslation.Value))
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
                    translation.Value = UtilityFunctions.BotHeightCorrect(translation.Value + math.normalize(targetPos.Value - translation.Value) * 1 * deltaTime);
                    if (UtilityFunctions.FlatOverlapCheck(translation.Value, targetPos.Value))
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
                    var waterPosition = GetComponent<LocalToWorld>(brigade.waterEntity).Value.c3.xyz;

                    translation.Value = UtilityFunctions.BotHeightCorrect(translation.Value + math.normalize(waterPosition - translation.Value) * 1 * deltaTime);

                    if (UtilityFunctions.FlatOverlapCheck(translation.Value, waterPosition))
                    {
                        // Next bot owns bucket 
                        EntityManager.RemoveComponent<Owner>(carriedBucket.Value);
                        EntityManager.RemoveComponent<CarriedBucket>(e);
                    }
                }
            ).Run();
    }
}
