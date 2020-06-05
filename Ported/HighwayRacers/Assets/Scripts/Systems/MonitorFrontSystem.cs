using Unity.Entities;

[UpdateAfter(typeof(TrackProgressUpdateSystem))]
public class MonitorFrontSystem : SystemBase
{
    private const float LaneBlockThreshold = 0.9f;
    private EntityQuery m_TrackInfoQuery;

    protected override void OnCreate()
    {
        m_TrackInfoQuery = GetEntityQuery(ComponentType.ReadOnly<TrackInfo>());
    }

    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        int trackLength = (int) trackProperties.TrackLength;

        TrackInfo trackInfo = m_TrackInfoQuery.GetSingleton<TrackInfo>();
        var progresses = trackInfo.Progresses;
        var speeds = trackInfo.Speeds;

        Entities
            .WithReadOnly(progresses)
            .WithReadOnly(speeds)
            .WithNone<TargetLane>()
            .ForEach((Entity entity, int entityInQueryIndex, ref CarInFront carInFront, in TrackPosition car) =>
            {
                int lane = (int)car.Lane;

                int trackIndexStart = lane * trackLength;
                int trackIndexEnd = (lane + 1) * trackLength;

                int index = (int)car.TrackProgress + trackIndexStart;

                // loop over the track, find the next progress
                for (int i = 1; i <= trackLength; i++)
                {
                    int actualIndex = index + i;

                    if (actualIndex >= trackIndexEnd)
                    {
                        actualIndex -= trackLength;
                    }

                    float nextProgress = progresses[actualIndex];

                    if (nextProgress != 0)
                    {
                        carInFront.TrackProgressCarInFront = nextProgress;
                        carInFront.Speed = speeds[actualIndex];
                        break;
                    }
                }
            })
            .ScheduleParallel();

        Entities
            .WithReadOnly(progresses)
            .WithReadOnly(speeds)
            .ForEach((Entity entity, int entityInQueryIndex, ref CarInFront carInFront, in TrackPosition car, in TargetLane targetLane) =>
            {
                int lane = targetLane.Value;

                int trackIndexStart = lane * trackLength;
                int trackIndexEnd = (lane + 1) * trackLength;

                int index = (int) car.TrackProgress + trackIndexStart;

                // loop over the track, find the next progress
                for (int i = 1; i <= trackLength; i++)
                {
                    int actualIndex = index + i;

                    if (actualIndex >= trackIndexEnd)
                    {
                        actualIndex -= trackLength;
                    }

                    float nextProgress = progresses[actualIndex];

                    if (nextProgress != 0)
                    {
                        carInFront.TrackProgressCarInFront = nextProgress;
                        carInFront.Speed = speeds[actualIndex];
                        break;
                    }
                }
            })
            .ScheduleParallel();
    }
}
