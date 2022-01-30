using Onboarding.BezierPath;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class SplineFollowerSystem : SystemBase
{
    protected override void OnUpdate() {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithAll<SplineFollower>()
            .ForEach((ref Translation translation,
                      ref Rotation rotation,
                      ref TrackProgress trackProgress,
                      ref SplineFollower splineFollower,
                      in CurrentRoute currentRoute,
                      in Ticker ticker,
                      in Speed speed,
                      in Spline spline) =>
            {
                if (currentRoute.state == TrainState.Stopped)
                    return;
                
                ref var splineData = ref spline.splinePath.Value;

                if (currentRoute.state == TrainState.Approaching || currentRoute.state == TrainState.Departing)
                {
                    float location = trackProgress.Value < currentRoute.routeStartLocation
                        ? trackProgress.Value + splineData.pathLength
                        : trackProgress.Value;

                    float distanceToNearestStation = math.min(location - currentRoute.routeStartLocation,
                        currentRoute.routeEndLocation - location);
                    
                    trackProgress.Value += speed.Value * deltaTime * math.clamp(math.pow(distanceToNearestStation * 0.02f, 0.5f), 0.01f, 1.0f);
                }
                else
                {
                    trackProgress.Value += speed.Value * deltaTime;
                }

                trackProgress.Value = math.clamp(trackProgress.Value, currentRoute.routeStartLocation, currentRoute.routeEndLocation);
                SplineInterpolationHelper.InterpolatePositionAndDirection(ref splineData, ref trackProgress.SplineLookupCache, trackProgress.Value, out var position, out var direction);
                translation.Value = position;
                rotation.Value = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));
                
            }).ScheduleParallel();
    }
}
