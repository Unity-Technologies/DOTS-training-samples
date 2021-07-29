using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BotActionSystemFillBucket : SystemBase
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

        Entities
            .ForEach((ref BotActionFillBucket action, ref TargetLocationComponent targetLocation, ref TargetWater targetWater, ref CarriedBucket carriedBucket,
                    in Translation trans) =>
                {
                    if (targetWater.water == Entity.Null)
                        return;
                    if (carriedBucket.bucket == Entity.Null)
                        return;

                    // Action is done when bucket is full
                    if (GetComponent<BucketFullComponent>(carriedBucket.bucket).full)
                    {
                        action.ActionDone = true;
                        return;
                    }

                    // Move to the water source
                    var waterLocation = GetComponent<Translation>(targetWater.water).Value.xz;
                    targetLocation.location = waterLocation;
                    var botPos = trans.Value;
                    var distanceSq = math.distancesq(targetLocation.location, botPos.xz);
                    if (distanceSq > distanceThresholdSq)
                        return;

                    // Fill bucket
                    var bucket = carriedBucket.bucket;
                    if (GetComponent<BucketStartFill>(bucket).Water != targetWater.water)
                    {
                        SetComponent(carriedBucket.bucket, new BucketStartFill() {Water = targetWater.water});
                    }
                }
            ).Schedule();
    }
}