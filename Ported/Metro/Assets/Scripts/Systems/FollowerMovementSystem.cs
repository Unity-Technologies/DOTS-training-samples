using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering;
using UnityEngine;

public partial class FollowerMovementSystem : SystemBase
{
    private const float CartLength = 5.5f;
    protected override void OnUpdate()
    {

        Entities.ForEach((ref Translation translation, ref Rotation rotation, in Follower follower) =>
        {

            var leaderTrackProgress = GetComponent<TrackProgress>(follower.Leader);
            var spline = GetComponent<Spline>(follower.TrackData);
            ref var splineData = ref spline.splinePath.Value;
            
            var followerProgress = leaderTrackProgress.Value - follower.CartIndexInTrain * CartLength;
            if (followerProgress < 0)
            {
                followerProgress += splineData.pathLength;
            }
            
            float3 position;
            float3 direction;
            SplineInterpolationHelper.InterpolatePositionAndDirection(ref splineData, ref leaderTrackProgress.SplineLookupCache, followerProgress, out position, out direction);
            translation.Value = position;
            rotation.Value = quaternion.LookRotationSafe(direction, new float3(0, 1 ,0));
            

        }).ScheduleParallel();
    }
}
