
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FarmerMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var gameTime = GetSingleton<GameTime>();
        var deltaTime = gameTime.DeltaTime;
        Entities
            .WithAll<Farmer>()
            .ForEach((ref Position position, in TargetEntity targetEntity, in Speed speed) =>
            {
                float2 targetPos = targetEntity.targetPosition;
                float2 direction = targetPos - position.Value;

                if(math.length(direction) > 0)
                {
                    float stepSize = speed.Value * deltaTime;
                    stepSize = math.min(stepSize, math.length(direction));
                    position.Value = position.Value + math.normalize(direction) * stepSize;
                }
            }).ScheduleParallel();

    }
}
