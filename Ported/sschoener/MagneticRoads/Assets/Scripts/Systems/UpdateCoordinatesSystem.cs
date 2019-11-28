using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateApproachSpeedSystem))]
    public class UpdateCoordinatesSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var roadSetup = GetSingleton<RoadSetupComponent>();
            return Entities.ForEach((ref CoordinateSystemComponent coords, in LocalIntersectionComponent localIntersection, in VehicleStateComponent vehicleState, in CarSpeedComponent speed, in OnSplineComponent spline, in SplineDataComponent splineData) =>
            {
                sbyte side = vehicleState == VehicleState.OnIntersection ? localIntersection.Side : spline.Value.Side;
                float2 extrudePoint = new float2(side, side);
                if (vehicleState != VehicleState.OnIntersection)
                    extrudePoint.x *= spline.Value.Direction;

                extrudePoint.x *= -roadSetup.TrackRadius * .5f;
                extrudePoint.y *= roadSetup.TrackThickness * .5f;

                float t = math.clamp(speed.SplineTimer, 0, 1);
                if (vehicleState != VehicleState.OnIntersection && spline.Value.Direction == -1)
                    t = 1f - t;
                
                // find our position and orientation
                var trackCoords = TrackUtils.Extrude(splineData.Bezier, splineData.Geometry, splineData.TwistMode, t, out _, out _);
                float3 splinePoint = trackCoords.Base + trackCoords.Right * extrudePoint.x + trackCoords.Up * extrudePoint.y;
                var up = trackCoords.Up * side;
                coords.Up = up;
                coords.Position = splinePoint + up * .06f;
                coords.Forward = math.cross(trackCoords.Right, trackCoords.Up);
            }).WithName("UpdateCoordinateSystem").Schedule(inputDeps);
        }
    }
}
