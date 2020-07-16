using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class ApplyVelocity : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        // Take velocity over time and apply to translation
        Entities.ForEach((ref Translation translation, in Velocity velocity) => translation.Value += velocity.Value * deltaTime).ScheduleParallel();
    }
}
