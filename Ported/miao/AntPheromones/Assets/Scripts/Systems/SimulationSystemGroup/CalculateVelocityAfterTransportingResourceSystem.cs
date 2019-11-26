using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(TransportResourceSystem))]
    public class CalculateVelocityAfterTransportingResourceSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job().Schedule(this, inputDeps);
        }
        
        private struct Job : IJobForEach<Speed, FacingAngle, Velocity>
        {
            public void Execute(
                [ReadOnly] ref Speed speed, 
                [ReadOnly] ref FacingAngle facingAngle, 
                [WriteOnly] ref Velocity velocity)
            {
                math.sincos(facingAngle.Value, out float sin, out float cos);
                velocity.Value = new float2(cos, sin) * speed.Value;
            }
        }
    }
}