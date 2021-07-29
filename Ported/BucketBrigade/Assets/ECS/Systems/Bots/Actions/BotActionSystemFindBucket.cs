using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BotActionSystemFindBucket : SystemBase
{
    EntityQuery bucketsQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<GameConfigComponent>();
        bucketsQuery = GetEntityQuery(typeof(Translation), typeof(BucketActiveComponent), typeof(BucketFullComponent));
    }

    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfigComponent>();
        var distanceThresholdSq = gameConfig.TargetProximityThreshold * gameConfig.TargetProximityThreshold;

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
                (ref BotActionFindBucket action, ref TargetLocationComponent targetLocation, ref TargetBucket targetBucket, ref CarriedBucket carriedBucket, in Translation trans) =>
                {
                    var botPos = trans.Value;

                    // Look for the closest non active and non full bucket
                    if (targetBucket.bucket == Entity.Null)
                    {
                        var bestBucket = Entity.Null;
                        var bestBucketDistance = float.MaxValue;
                        var bestBucketPos = float3.zero;
                        for (int i = 0; i < buckets.Length; ++i)
                        {
                            if (bucketActiveArray[i].active || bucketFullArray[i].full)
                                continue;

                            var bucketPos = bucketTransArray[i].Value;
                            var distance = math.distancesq(botPos, bucketPos);
                            if (distance < bestBucketDistance)
                            {
                                bestBucketDistance = distance;
                                bestBucket = buckets[i];
                                bestBucketPos = bucketPos;
                            }
                        }

                        if (bestBucket != Entity.Null)
                        {
                            targetBucket.bucket = bestBucket;
                            targetLocation.location = bestBucketPos.xz;
                        }

                        return;
                    }
                    
                    // Bucket has been picked up => not a valid target
                    if (GetComponent<BucketActiveComponent>(targetBucket.bucket).active)
                    {
                        targetBucket.bucket = Entity.Null;
                        return;
                    }

                    // Go to the selected bucket
                    var distanceSq = math.distancesq(targetLocation.location, botPos.xz);
                    if (distanceSq > distanceThresholdSq)
                        return;
                    
                    // Pick up the bucket
                    carriedBucket.bucket = targetBucket.bucket;
                    SetComponent(carriedBucket.bucket, new BucketActiveComponent() {active = true});
                    action.ActionDone = true;
                }
            ).Schedule();
    }
}