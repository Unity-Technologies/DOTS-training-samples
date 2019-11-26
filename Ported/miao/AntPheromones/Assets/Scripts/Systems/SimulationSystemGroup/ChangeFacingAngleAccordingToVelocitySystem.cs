using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(MoveInwardOrOutwardSystem))]
    public class ChangeFacingAngleAccordingToVelocitySystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job().Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Velocity, FacingAngle>
        {
            public void Execute(
                [ReadOnly] ref Velocity velocity,
                [WriteOnly] ref FacingAngle facingAngle)
            {
                facingAngle.Value = math.atan2(velocity.Value.y, velocity.Value.x);
            }
        }
    }
}