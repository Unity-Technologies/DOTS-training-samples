using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ResourceCarrySystem))]
public class ComputeVelocitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((ref VelocityComponent velocity, in SpeedComponent speed, in FacingAngleComponent facingAngle) =>
        {
            math.sincos(facingAngle.Value, out var sin, out var cos);
            velocity.Value = new float2(cos, sin) * speed.Value;
        }).Schedule(inputDeps);
    }
}
