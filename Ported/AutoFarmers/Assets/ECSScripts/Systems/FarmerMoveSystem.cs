
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(AssignTaskSystem))]
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
                position.Value = targetEntity.targetPosition;
                //float2 targetPos = targetEntity.targetPosition;
                //float2 direction = math.normalize(targetPos - position.Value);
                //float2 stepSize = speed.Value * deltaTime;
                //float2 clampedStepSize = math.min(stepSize, math.length(targetPos - position.Value));
                //position.Value = position.Value + direction * clampedStepSize;

            }).ScheduleParallel();
    }
}
