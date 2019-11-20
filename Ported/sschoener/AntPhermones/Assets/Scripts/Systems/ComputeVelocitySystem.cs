using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ResourceCarrySystem))]
public class ComputeVelocitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new Job().Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct Job : IJobForEach<SpeedComponent, FacingAngleComponent, VelocityComponent>
    {
        public void Execute([ReadOnly] ref SpeedComponent speed, [ReadOnly] ref FacingAngleComponent facingAngle, [WriteOnly] ref VelocityComponent velocity)
        {
            math.sincos(facingAngle.Value, out var sin, out var cos);
            velocity.Value = new float2(cos, sin) * speed.Value;
        }
    }
}
