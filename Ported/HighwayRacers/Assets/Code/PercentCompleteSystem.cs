using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PercentCompleteSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfo)));
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        LaneInfo laneInfo = GetSingleton<LaneInfo>();
        float totalDistance = math.abs(laneInfo.EndXZ.y - laneInfo.StartXZ.y);
        float deltaTime = Time.DeltaTime;

        //may not need road length
        Entities.ForEach((ref PercentComplete percentComplete, in Speed speed) =>
        {
            float relativeStartPosition = totalDistance * percentComplete.Value;
            var distance = speed.Value * deltaTime;
            float newPercentComplete = (relativeStartPosition + distance) / totalDistance;
            if (newPercentComplete > 1.0f)
                newPercentComplete = 0.0f;
            percentComplete.Value = newPercentComplete;

        }).Schedule();

        ecb.Playback(EntityManager);
    }
}
