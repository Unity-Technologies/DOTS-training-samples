using Unity.Entities;

[UpdateAfter(typeof(MonitorFrontSystem))]
[UpdateAfter(typeof(LaneChangeSystem))]
public class TrackInfoResetSystem : SystemBase
{
    EntityQuery m_TrackInfoQuery;

    protected override void OnCreate()
    {
        m_TrackInfoQuery = GetEntityQuery(ComponentType.ReadWrite<TrackInfo>());
    }
    protected override void OnUpdate()
    {
        TrackInfo trackInfo = m_TrackInfoQuery.GetSingleton<TrackInfo>();

        var progresses = trackInfo.Progresses;
        var speeds = trackInfo.Speeds;
        var lanes = trackInfo.Lanes;

        Entities
            .WithAll<TrackInfo>()
            .ForEach((Entity e) =>
            {
                for (int i = 0; i < progresses.Length; i++)
                {
                    progresses[i] = 0;
                }
            }).ScheduleParallel();

        Entities
            .WithAll<TrackInfo>()
            .ForEach((Entity e) =>
            {
                for (int i = 0; i < speeds.Length; i++)
                {
                    speeds[i] = 0;
                }
            }).ScheduleParallel();

        Entities
            .WithAll<TrackInfo>()
            .ForEach((Entity e) =>
            {
                for (int i = 0; i < lanes.Length; i++)
                {
                    lanes[i] = 0;
                }
            }).ScheduleParallel();
    }
}
