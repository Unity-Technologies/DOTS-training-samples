using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(RadialMovementSystem))]
public class WriteVelocityToFacingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) =>
        Entities.ForEach((ref FacingAngleComponent facingAngle, in VelocityComponent velocity) =>
        {
            facingAngle.Value = math.atan2(velocity.Value.y, velocity.Value.x);
        }).Schedule(inputDeps);
}