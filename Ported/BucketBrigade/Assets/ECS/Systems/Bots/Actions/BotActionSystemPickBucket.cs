using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BotActionSystemPickBucket : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<GameConfigComponent>();
    }

    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfigComponent>();
        var distanceThresholdSq = gameConfig.TargetProximityThreshold * gameConfig.TargetProximityThreshold; 
        
        var bucketsQuery = GetEntityQuery(typeof(Translation), typeof(BucketActiveComponent), typeof(BucketFullComponent));
        var buckets = bucketsQuery.ToEntityArray(Allocator.TempJob);
        var bucketTransArray = bucketsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketActiveArray = bucketsQuery.ToComponentDataArray<BucketActiveComponent>(Allocator.TempJob);
        var bucketFullArray = bucketsQuery.ToComponentDataArray<BucketFullComponent>(Allocator.TempJob);

        Entities
            .WithDisposeOnCompletion(buckets)
            .WithDisposeOnCompletion(bucketTransArray)
            .WithDisposeOnCompletion(bucketActiveArray)
            .WithDisposeOnCompletion(bucketFullArray)
            .ForEach(
                (ref BotActionPickBucket action, ref TargetLocationComponent targetLocation, ref TargetBucket targetBucket, ref CarriedBucket carriedBucket,
                    in Translation trans, in BotPickUpLocation pickUpLocation) =>
                {
                    // Go to the pick up location
                    targetLocation.location = pickUpLocation.Value;
                    var botPos = trans.Value;
                    var distanceSq = math.distancesq(targetLocation.location, botPos.xz);
                    if (distanceSq > distanceThresholdSq)
                        return;
                        
                    // Look for the closest non active and non full bucket
                    if (targetBucket.bucket == Entity.Null)
                    {
                        var pickUpPos = pickUpLocation.Value;
                        var bestBucket = Entity.Null;
                        var bestBucketDistance = distanceThresholdSq * 2.0f;
                        for (int i = 0; i < buckets.Length; ++i)
                        {
                            if (bucketActiveArray[i].active || !bucketFullArray[i].full)
                                continue;

                            var bucketPos = bucketTransArray[i].Value;
                            var distance = math.distancesq(pickUpPos, bucketPos.xz);
                            if (distance < bestBucketDistance)
                            {
                                bestBucketDistance = distance;
                                bestBucket = buckets[i];
                            }
                        }

                        if (bestBucket != Entity.Null)
                        {
                            targetBucket.bucket = bestBucket;
                            Debug.Log("BotActionSystemPickBucket => found Bucket");
                        }
                        else
                        {
                            return;
                        }
                    }

                    // Pick up the selected bucket
                    carriedBucket.bucket = targetBucket.bucket;
                    SetComponent(carriedBucket.bucket, new BucketActiveComponent() {active = true});
                    action.ActionDone = true;
                    Debug.Log("BotActionSystemPickBucket => reached Bucket");
                }
            ).Schedule();
    }
}