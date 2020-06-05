using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(TrackInitialisationSystem))]
public class TrackProgressUpdateSystem : SystemBase
{
    EntityQuery m_TrackInfoQuery;

    protected override void OnCreate()
    {
        m_TrackInfoQuery = GetEntityQuery(ComponentType.ReadOnly<TrackInfo>());
    }

    protected override void OnUpdate()
    {
        TrackInfo trackInfo = m_TrackInfoQuery.GetSingleton<TrackInfo>();

        var progresses = trackInfo.Progresses;
        var lanes = trackInfo.Lanes;
        var speeds = trackInfo.Speeds;

        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        float dtime = Time.DeltaTime;

        Entities
            .ForEach((ref TrackPosition trackPosition, in Speed speed, in TargetLane targetLane) =>
        {
            trackPosition.TrackProgress += speed.Value * dtime;
            trackPosition.TrackProgress %= trackProperties.TrackLength;

            int lane = targetLane.Value;
            int index = (int) trackPosition.TrackProgress + (lane * (int) trackProperties.TrackLength);
            progresses[index] = trackPosition.TrackProgress;
            lanes[index] = lane;
            speeds[index] = speed.Value;
        }).ScheduleParallel();

        Entities
            .WithNone<TargetLane>()
            .ForEach((ref TrackPosition trackPosition, in Speed speed) =>
        {
            trackPosition.TrackProgress += speed.Value * dtime;
            trackPosition.TrackProgress %= trackProperties.TrackLength;

            int lane = Mathf.RoundToInt(trackPosition.Lane);
            int index = (int)trackPosition.TrackProgress + (lane * (int) trackProperties.TrackLength);
            progresses[index] = trackPosition.TrackProgress;
            lanes[index] = trackPosition.Lane;
            speeds[index] = speed.Value;
        }).ScheduleParallel();
    }
}