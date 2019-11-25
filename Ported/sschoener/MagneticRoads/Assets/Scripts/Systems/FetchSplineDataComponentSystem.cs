using System;
using Unity.Entities;
using Unity.Jobs;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FetchSplineDataComponentSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return Entities.ForEach((ref SplineDataComponent spline, in OnSplineComponent onSpline, in LocalIntersectionComponent localIntersection, in InIntersectionComponent inIntersection) =>
            {
                if (inIntersection.Value)
                {
                    spline.Bezier = localIntersection.Bezier;
                    spline.Geometry = localIntersection.Geometry;
                    spline.TwistMode = 0;
                }
                else
                {
                    spline.Bezier = TrackSplines.bezier[onSpline.Spline];
                    spline.Geometry = TrackSplines.geometry[onSpline.Spline];
                    spline.TwistMode = TrackSplines.twistMode[onSpline.Spline];
                }
            }).WithoutBurst().Schedule(inputDeps);
        }
    }
}
