using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FetchSplineDataComponentSystem))]
    public class UpdateSpeedSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float dt = Time.DeltaTime;
            const float maxSpeed = 2;
            return Entities.ForEach((ref CarSpeedComponent speed, in SplineDataComponent spline) =>
            {
                speed.NormalizedSpeed = math.min(speed.NormalizedSpeed + dt * 2, 1);
                speed.SplineTimer += speed.NormalizedSpeed * maxSpeed / spline.Length * dt;
            }).WithName("UpdateSpeed").Schedule(inputDeps);
        }
    }
}
