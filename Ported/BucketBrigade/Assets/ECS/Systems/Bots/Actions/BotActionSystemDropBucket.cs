using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BotActionSystemDropBucket : SystemBase
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

        var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbs.CreateCommandBuffer();

        Entities
            //.WithNativeDisableContainerSafetyRestriction(EntityManager.GetComponentTypeHandle<Translation>(false))
            .ForEach(
                (ref BotActionDropBucket action, ref TargetLocationComponent targetLocation, ref TargetBucket targetBucket, ref CarriedBucket carriedBucket,
                    in Translation trans, in BotDropOffLocation dropLocation) =>
                {
                    // Move to the drop off location
                    var botPos = trans.Value;
                    var dropPos = dropLocation.Value;
                    targetLocation.location = dropPos;
                    var distanceSq = math.distancesq(botPos.xz, dropPos);
                    if (distanceSq > distanceThresholdSq)
                        return;

                    // Drop the bucket
                    var bucket = carriedBucket.bucket;
                    if (bucket != Entity.Null)
                    {
                        ecb.SetComponent(bucket, new Translation() {Value = botPos});
                        ecb.SetComponent(bucket, new BucketActiveComponent() {active = false});
                    }
                    targetBucket.bucket = Entity.Null;
                    carriedBucket.bucket = Entity.Null;
                    
                    action.ActionDone = true;
                }
            ).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}