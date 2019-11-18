using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(RadialMovementSystem))]
public class WriteVelocityToFacingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new Job().Schedule(this, inputDeps);
    }

    struct Job : IJobForEach<VelocityComponent, FacingAngleComponent>
    {
        public void Execute([ReadOnly]ref VelocityComponent velocity, [WriteOnly] ref FacingAngleComponent facingAngle)
        {
            facingAngle.Value = math.atan2(velocity.Value.x , velocity.Value.y);
        }
    }
}
