using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;

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
            return Entities.ForEach((ref CarSpeedComponent speed, in OnSplineComponent onSpline, in LocalIntersectionComponent localIntersection, in InIntersectionComponent inIntersection) =>
            {
                float length;
                if (inIntersection.Value)
                    length = localIntersection.Length;
                else
                    length = TrackSplines.measuredLength[onSpline.Spline];
                speed.NormalizedSpeed = math.min(speed.NormalizedSpeed * dt * 2, 1);
                speed.SplineTimer += speed.NormalizedSpeed * maxSpeed / length * dt;
            }).WithoutBurst().Schedule(inputDeps);
        }
    }
}
