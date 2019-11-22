using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ChangeVelocityAfterObstacleCollisionSystem))]
    public class ChangeFacingAngleAccordingToVelocitySystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job().Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<VelocityComponent, FacingAngleComponent>
        {
            public void Execute(
                [ReadOnly] ref VelocityComponent velocity,
                [WriteOnly] ref FacingAngleComponent facingAngle)
            {
                facingAngle.Value = math.atan2(velocity.Value.x , velocity.Value.y);
            }
        }
    }
}