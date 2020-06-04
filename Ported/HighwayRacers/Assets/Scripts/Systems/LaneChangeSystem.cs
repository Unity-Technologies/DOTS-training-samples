using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class LaneChangeSystem : SystemBase
{
    private const float Threshold = 0.1f;
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();

        //setup all other native arrays
        NativeArray<TrackPosition> otherCars = GetEntityQuery(typeof(TrackPosition)).ToComponentDataArray<TrackPosition>(Allocator.TempJob);

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithAll<BlockedState>()
            .WithNone<TargetLane>()
            .ForEach((Entity entity, int entityInQueryIndex, in TrackPosition car, in CarProperties properties, in CarInFront carInFront) =>
            {
                // overtake eagerness based on how slow is the car in front,
                // compared to how fast I should be going
                if (properties.OvertakeEagerness > carInFront.Speed / properties.DefaultSpeed)
                {
                    // 1. get our lane
                    int lane = (int)car.Lane;
                    float progress = car.TrackProgress;

                    // 2. consider neighboring lanes and find empty one
                    // sort through others and find closest (in front) in neighboring lane(s)
                    // at lane 0, consider lane 1
                    // 1, 2 -> consider 0/2 or 1/3
                    // 3 -> consider 2
                    NativeArray<float> lanesToConsider = new NativeArray<float>(trackProperties.NumberOfLanes, Allocator.Temp);

                    for (int carIndex = 0; carIndex < otherCars.Length; carIndex++)
                    {
                        float otherProgress = otherCars[carIndex].TrackProgress;
                        float otherLane = otherCars[carIndex].Lane;

                        // calculate actual index for lane (taking lane switching threshold into account)
                        int actualLane = (int)otherLane;

                        if (otherLane - actualLane > Threshold)
                        {
                            ++actualLane;
                        }

                        // if the lane is not already blocked
                        if (lanesToConsider[actualLane] != -1)
                        {
                            float distanceBetweenCars = otherProgress - progress;
                            float reverseDistanceBetweenCars = -distanceBetweenCars;
                            distanceBetweenCars = (distanceBetweenCars + trackProperties.TrackLength) % trackProperties.TrackLength;
                            reverseDistanceBetweenCars = (reverseDistanceBetweenCars + trackProperties.TrackLength) % trackProperties.TrackLength;

                            // check if the closest car in front is closer than allowed merge space
                            if (distanceBetweenCars < properties.MergeSpace || reverseDistanceBetweenCars < properties.MergeSpace)
                            {
                                // mark lane blocked and move on
                                lanesToConsider[actualLane] = -1;
                            }
                        }
                    }

                    // start changing lanes
                    for (int i = 0; i < lanesToConsider.Length; i++)
                    {
                        // only look at nonblocked, neighboring lanes (difference == 1)
                        if (lanesToConsider[i] != -1 && math.abs(lane - i) == 1)
                        {
                            // add it to entity
                            commandBuffer.AddComponent(entityInQueryIndex, entity, new TargetLane { Value = i });
                        }
                    }
                }
            })
            .WithDeallocateOnJobCompletion(otherCars)
            .ScheduleParallel();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}