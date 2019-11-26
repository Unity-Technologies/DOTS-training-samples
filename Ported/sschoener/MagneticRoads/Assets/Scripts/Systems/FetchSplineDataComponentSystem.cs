using System;
using Unity.Entities;
using Unity.Jobs;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FetchSplineDataComponentSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var blob = TrackSplinesBlob.Instance;
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
                    ref var s = ref blob.Value.Splines[onSpline.Spline]; 
                    spline.Bezier = s.Bezier;
                    spline.Geometry = s.Geometry;
                    spline.TwistMode = s.TwistMode;
                    spline.Length = s.MeasuredLength;
                }
                onSpline.Dirty = false;
            }).WithName("FetchSplineData").Schedule(inputDeps);
        }
    }
}
