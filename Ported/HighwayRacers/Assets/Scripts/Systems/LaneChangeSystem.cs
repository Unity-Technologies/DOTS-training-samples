using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class LaneChangeSystem : SystemBase
{
    private const float Threshold = 0.1f;

    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    private CarSortingByLaneSystem m_CarSortingByLaneSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

        m_CarSortingByLaneSystem = World.GetExistingSystem<CarSortingByLaneSystem>();
    }

    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();

        var carSortingByLaneSystem = m_CarSortingByLaneSystem;
        var carInfoDatabase = carSortingByLaneSystem.GetDatabase();

        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Dependency = jobReadFromCarInfos = Entities
            .WithAll<BlockedState>()
            .WithNone<TargetLane>()
            .ForEach((Entity entity, int entityInQueryIndex, in TrackPosition car, in CarProperties properties, in CarInFront carInFront) =>
            {
                // overtake eagerness based on how slow is the car in front,
                // compared to how fast I should be going
                if (properties.OvertakeEagerness > carInFront.Speed / properties.DefaultSpeed)
                {
                    int lane = (int)car.Lane;
                    float progress = car.TrackProgress;

                    bool added = AddTargetLaneIfValidAtOffset(lane, progress, -1,
                                    carInfoDatabase, trackProperties, properties,
                                    ref commandBuffer, entityInQueryIndex, entity);
                    if (!added)
                    {
                        AddTargetLaneIfValidAtOffset(lane, progress, 1,
                            carInfoDatabase, trackProperties, properties,
                            ref commandBuffer, entityInQueryIndex, entity);
                    }
                }
            })
            .ScheduleParallel(Dependency);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    static bool IsValidToChange(float progress, float otherProgress, in TrackProperties trackProperties, in CarProperties properties)
    {
        if (otherProgress >= float.MaxValue)
            return true;

        float distanceBetweenCars = otherProgress - progress;
        float reverseDistanceBetweenCars = -distanceBetweenCars;
        distanceBetweenCars = (distanceBetweenCars + trackProperties.TrackLength) % trackProperties.TrackLength;
        reverseDistanceBetweenCars = (reverseDistanceBetweenCars + trackProperties.TrackLength) % trackProperties.TrackLength;

        // check if the closest car in front is closer than allowed merge space
        return !(distanceBetweenCars < properties.MergeSpace || reverseDistanceBetweenCars < properties.MergeSpace);
    }

    // Return true if targetLane is added
    static bool AddTargetLaneIfValidAtOffset(int lane, float progress, int offset, 
        in CarSortingByLaneSystem.Database carInfoDatabase, in TrackProperties trackProperties, in CarProperties carProperties,
        ref EntityCommandBuffer.Concurrent commandBuffer, int entityInQueryIndex, Entity entity)
    {
        bool validAtAdjacentLane = carInfoDatabase.GetCarOnAdjacentLane(lane, offset, progress, out var outCarInFront, out var outCarBehind);
        if (validAtAdjacentLane && IsValidToChange(progress, outCarInFront.position, trackProperties, carProperties)
            && IsValidToChange(progress, outCarBehind.position, trackProperties, carProperties))
        {
            commandBuffer.AddComponent(entityInQueryIndex, entity, new TargetLane { Value = lane + offset });
            return true;
        }

        return false;
    }

    JobHandle jobReadFromCarInfos;
    public JobHandle GetJobHandleReadFromCarInfos()
    {
        return jobReadFromCarInfos;
    }
}