using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ApproachSystem : SystemBase
{
    private const float APPROACH_THRESHOLD = 0.001f;

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .WithName("commuter_movement")
            .ForEach((ref Translation translation, in TargetPoint target, in Speed speed) =>
            {
                float3 direction = target.CurrentTarget - translation.Value;
                
                float length = math.length(direction);
                
                if (length >= APPROACH_THRESHOLD)
                {
                    float movement = math.min(speed.Value * deltaTime, length);
                    translation.Value += direction / length * movement;
                }
                else
                {
                    // TODO Remove TargetPoint component
                }

            }).ScheduleParallel();
    }
}
