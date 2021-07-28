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
        var threshold = 0.05f;
        
        Entities.ForEach((ref Translation trans, in TargetLocationComponent target) =>
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
            }
        }).Schedule();
    }
}
