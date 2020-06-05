using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(TrackProgressUpdateSystem))]
public class LaneChangeSystem : SystemBase
{
    private const float Threshold = 0.1f;
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    EntityQuery m_TrackInfoQuery;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        m_TrackInfoQuery = GetEntityQuery(ComponentType.ReadOnly<TrackInfo>());
    }

    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        TrackInfo trackInfo = m_TrackInfoQuery.GetSingleton<TrackInfo>();

        var progresses = trackInfo.Progresses;

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithReadOnly(progresses)
            .WithAll<BlockedState>()
            .WithNone<TargetLane>()
            .ForEach((Entity entity, int entityInQueryIndex, in TrackPosition car, in CarProperties properties, in CarInFront carInFront) =>
            {
                // overtake eagerness based on how slow is the car in front,
                // compared to how fast I should be going
                //if (properties.OvertakeEagerness > carInFront.Speed / properties.DefaultSpeed)
                {
                    // 1. get our lane
                    int lane = (int)car.Lane;
                    float progress = car.TrackProgress;
                    float mergeSpace = properties.MergeSpace;

                    NativeArray<float> lanesToConsider = new NativeArray<float>(trackProperties.NumberOfLanes, Allocator.Temp);
                    lanesToConsider[0] = -1;
                    lanesToConsider[1] = -1;
                    lanesToConsider[2] = -1;
                    lanesToConsider[3] = -1;

                    if (lane == 0)
                    {
                        lanesToConsider[lane + 1] = ConsiderLane(lane + 1, trackProperties.TrackLength, progress, mergeSpace, progresses);
                    }
                    else if (lane == 3)
                    {
                        lanesToConsider[lane - 1] = ConsiderLane(lane - 1, trackProperties.TrackLength, progress, mergeSpace, progresses);
                    }
                    else
                    {
                        lanesToConsider[lane - 1] = ConsiderLane(lane - 1, trackProperties.TrackLength, progress, mergeSpace, progresses);
                        lanesToConsider[lane + 1] = ConsiderLane(lane + 1, trackProperties.TrackLength, progress, mergeSpace, progresses);
                    }

                    // start changing lanes
                    for (int i = lanesToConsider.Length - 1; i >= 0; --i)
                    {
                        // only look at nonblocked, neighboring lanes (difference == 1)
                        if (lanesToConsider[i] != -1 && math.abs(lane - i) == 1)
                        {
                            // add it to entity
                            commandBuffer.AddComponent(entityInQueryIndex, entity, new TargetLane { Value = i });
                            break;
                        }
                    }
                }
            })
            .ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    private static int ConsiderLane(int lane, float trackLength, float progress, float mergeSpace, NativeArray<float> progresses)
    {
        // consider lane 1
        int start = (int)(lane * trackLength + (progress - mergeSpace));
        int end = (int)(lane * trackLength + (progress + mergeSpace));

        // walk start to end, if any has values, blocked
        for (int i = start; i <= end; i++)
        {
            int index = i;

            if (index >= ((lane + 1) * trackLength))
            {
                index -= (int) trackLength;
            }
            else if (index < lane * trackLength)
            {
                index += (int)trackLength;
            }

            if (progresses[index] != 0)
            {
                // blocked, stop
                return -1;
            }
        }

        return lane;
    }
}