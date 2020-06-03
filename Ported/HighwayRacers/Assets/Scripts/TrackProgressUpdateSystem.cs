using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TrackProgressUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        float dtime = Time.DeltaTime;

        Entities.ForEach((ref TrackPosition trackPosition, in Speed speed) =>
        {
            trackPosition.TrackProgress += speed.Value * dtime;
            trackPosition.TrackProgress %= trackProperties.TrackLength;
        }).ScheduleParallel();
    }
}