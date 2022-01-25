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

        Entities.ForEach((ref Translation translation, in Follower follower, in Rotation rotation) =>
        {

            var leaderTrackProgress = GetComponent<TrackProgress>(follower.Leader);

            translation.Value.x = leaderTrackProgress.Value - follower.CartIndexInTrain * CartLength;

        }).Schedule();
    }
}
