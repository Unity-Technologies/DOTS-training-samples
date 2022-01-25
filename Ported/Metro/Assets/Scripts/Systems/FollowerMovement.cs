using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering;

public partial class FollowerMovement : SystemBase
{
    private const float CartLength = 3;
    protected override void OnUpdate()
    {

        Entities.ForEach((ref Translation translation, ref Rotation rotation, in Follower follower) =>
        {

            var leaderTrackProgress = GetComponent<TrackProgress>(follower.Leader);
            var trackData = GetComponent<TrackSimulatedData>(follower.TrackData);
            
            var followerProgress = leaderTrackProgress.Value - follower.CartIndexInTrain * CartLength;
            if (followerProgress < 0)
            {
                translation.Value = trackData.GetPositionForProgress(trackData.TotalLength + followerProgress);
                rotation.Value = trackData.GetQuaternionForProgress(trackData.TotalLength + followerProgress);
            }
            else
            {
                translation.Value = trackData.GetPositionForProgress(followerProgress);
                rotation.Value = trackData.GetQuaternionForProgress(followerProgress);
            }

        }).Schedule();
    }
}
