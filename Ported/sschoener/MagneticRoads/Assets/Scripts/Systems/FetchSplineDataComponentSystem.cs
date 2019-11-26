using System;
using Unity.Entities;
using Unity.Jobs;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FetchSplineDataComponentSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return Entities.ForEach((ref SplineDataComponent spline, ref OnSplineComponent onSpline, in LocalIntersectionComponent localIntersection) =>
            {
                if (!onSpline.Dirty)
                    return;
                if (onSpline.InIntersection)
                {
                    spline.Bezier = localIntersection.Bezier;
                    spline.Geometry = localIntersection.Geometry;
                    spline.TwistMode = 0;
                    spline.Length = localIntersection.Length;
                }
                else
                {
                    spline.Bezier = TrackSplines.bezier[onSpline.Spline];
                    spline.Geometry = TrackSplines.geometry[onSpline.Spline];
                    spline.TwistMode = TrackSplines.twistMode[onSpline.Spline];
                    spline.Length = TrackSplines.measuredLength[onSpline.Spline];
                }
                onSpline.Dirty = false;
            }).WithoutBurst().WithName("FetchSplineData").Schedule(inputDeps);
        }
    }
}
