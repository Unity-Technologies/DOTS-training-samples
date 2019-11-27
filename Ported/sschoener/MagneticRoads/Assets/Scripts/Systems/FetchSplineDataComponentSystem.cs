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
            return Entities.ForEach((ref SplineDataComponent spline, ref OnSplineComponent onSpline, in VehicleStateComponent vehicleState, in LocalIntersectionComponent localIntersection) =>
            {
                if (!onSpline.Value.Dirty)
                    return;
                if (vehicleState == VehicleState.EnteringIntersection)
                {
                    spline.Bezier = localIntersection.Bezier;
                    spline.Geometry = localIntersection.Geometry;
                    spline.TwistMode = 0;
                    spline.Length = localIntersection.Length;
                }
                else if (vehicleState == VehicleState.OnRoad)
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
