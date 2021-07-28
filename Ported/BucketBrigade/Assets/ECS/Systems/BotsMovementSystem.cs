using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BotsMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfigComponent>();
    }
    
    protected override void OnUpdate()
    {
        var config = GetSingleton<GameConfigComponent>();
        var botSpeed = config.BotSpeed;
        var threshold = config.TargetProximityThreshold;
        var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbs.CreateCommandBuffer();
        
        Entities.ForEach((ref Translation trans, in TargetLocationComponent target, in CarriedBucket carried) =>
        {
            var botPos = trans.Value;
            var targetPos = target.location;
            var diff = targetPos - botPos.xz;
            var distance = math.length(diff);
            if (distance > threshold)
            {
                var speed = math.min(botSpeed, distance);
                var move = (diff / distance) * speed;
                botPos += new float3(move.x, 0.0f, move.y);
                trans.Value = botPos;

                if (carried.bucket != Entity.Null)
                {
                    var t = new Translation() {Value = botPos + new float3(0, 1F, 0)};
                    ecb.SetComponent(carried.bucket, t);
                }
            }
        }).Schedule();
        
        ecbs.AddJobHandleForProducer(Dependency);
    }
}
