using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(TransportResourceSystem))]
    public class CalculateVelocitySystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job().Schedule(this, inputDeps);
        }
        
        private struct Job : IJobForEach<SpeedComponent, FacingAngleComponent, VelocityComponent>
        {
            public void Execute(
                [ReadOnly] ref SpeedComponent speed, 
                [ReadOnly] ref FacingAngleComponent facingAngle, 
                [WriteOnly] ref VelocityComponent velocity)
            {
                math.sincos(facingAngle.Value, out float sin, out float cos);
                velocity.Value = new float2(cos, sin) * speed.Value;
            }
        }
    }
}