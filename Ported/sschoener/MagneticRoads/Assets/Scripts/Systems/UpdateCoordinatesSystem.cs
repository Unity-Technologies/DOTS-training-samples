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
            return Entities.ForEach((ref CoordinateSystemComponent coords, in LocalIntersectionComponent localIntersection, in CarSpeedComponent speed, in OnSplineComponent spline, in SplineDataComponent splineData) =>
            {
                float2 extrudePoint;
                if (!spline.Value.InIntersection)
                {
                    extrudePoint = new float2(spline.Value.Side * spline.Value.Direction, spline.Value.Side);
                }
                else
                {
                    extrudePoint = new float2(localIntersection.Side, localIntersection.Side);
                }

                extrudePoint.x *= -roadSetup.TrackRadius * .5f;
                extrudePoint.y *= roadSetup.TrackThickness * .5f;

                float t = math.clamp(speed.SplineTimer, 0, 1);
                if (!spline.Value.InIntersection && spline.Value.Direction == -1)
                    t = 1f - t;
                
                // find our position and orientation
                var trackCoords = TrackUtils.Extrude(splineData.Bezier, splineData.Geometry, splineData.TwistMode, t, out _, out _);
                float3 splinePoint = trackCoords.Base + trackCoords.Right * extrudePoint.x + trackCoords.Up * extrudePoint.y;
                var up = trackCoords.Up * spline.Value.Side;
                coords.Up = up;
                coords.Position = splinePoint + up * .06f;
                coords.Forward = math.cross(trackCoords.Right, trackCoords.Up);
            }).WithName("UpdateCoordinateSystem").Schedule(inputDeps);
        }
    }
}
