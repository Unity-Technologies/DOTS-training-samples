using System;
using Unity.Entities;
using Unity.Jobs;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FetchSplineDataComponentSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var blob = GetSingleton<RoadSetupComponent>().Splines;
            return Entities.ForEach((ref SplineDataComponent spline, ref OnSplineComponent onSpline, in LocalIntersectionComponent localIntersection) =>
            {
                if (!onSpline.Value.Dirty)
                    return;
                if (onSpline.Value.InIntersection)
                {
                    spline.Bezier = localIntersection.Bezier;
                    spline.Geometry = localIntersection.Geometry;
                    spline.TwistMode = 0;
                    spline.Length = localIntersection.Length;
                }
                else
                {
                    ref var s = ref blob.Value.Splines[onSpline.Value.Spline]; 
                    spline.Bezier = s.Bezier;
                    spline.Geometry = s.Geometry;
                    spline.TwistMode = s.TwistMode;
                    spline.Length = s.MeasuredLength;
                }
                onSpline.Value.Dirty = false;
            }).WithName("FetchSplineData").Schedule(inputDeps);
        }
    }
}
