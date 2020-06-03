using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class CarSpawningSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();

        var barrier = m_EntityCommandBufferSystem;
        var commandBuffer = barrier.CreateCommandBuffer();

        var random = new Unity.Mathematics.Random(1);
        var jobHandle = Entities.ForEach((in Entity entity, in CarSpawner spawner) => 
        {
            var numberToBeSpawned = spawner.InstancesToSpawn;
            var carToBeSpawned = spawner.CarPrefab;

            for (int i = 0; i < numberToBeSpawned; ++i)
            {
                var newCar = commandBuffer.Instantiate(carToBeSpawned);
                commandBuffer.SetComponent(newCar, new CarProperties
                {
                    DefaultSpeed = spawner.MinDefaultSpeed + random.NextFloat(spawner.MaxDefaultSpeed - spawner.MinDefaultSpeed),
                    OvertakeSpeed = spawner.MinOvertakeSpeed + random.NextFloat(spawner.MaxOvertakeSpeed - spawner.MinOvertakeSpeed),
                    DistanceToCarBeforeOvertaking = spawner.MinDistanceToCarBeforeOvertaking + random.NextFloat(spawner.MaxDistanceToCarBeforeOvertaking - spawner.MinDistanceToCarBeforeOvertaking),
                    OvertakeEagerness = spawner.MinOvertakeEagerness + random.NextFloat(spawner.MaxOvertakeEagerness - spawner.MinOvertakeEagerness),
                    MergeSpace = spawner.MinMergeSpace + random.NextFloat(spawner.MaxMergeSpace - spawner.MinMergeSpace),
                    Acceleration = spawner.Acceleration,
                });

                commandBuffer.SetComponent(newCar, new Speed
                {
                    Value = 0
                });

                var pointOnInterval = (float)i / (float)numberToBeSpawned;
                UniformDistributionOverTrack(pointOnInterval, trackProperties.NumberOfLanes, trackProperties.TrackLength, out var targetLane, out var targetTrackProgress);
                commandBuffer.SetComponent(newCar, new TrackPosition
                {
                    Lane = (float)targetLane,
                    TrackProgress = targetTrackProgress,
                });

                commandBuffer.AddComponent(newCar, new CarInFront());

                commandBuffer.DestroyEntity(entity);
            }
        }).Schedule(Dependency);

        Dependency = jobHandle;

        barrier.AddJobHandleForProducer(jobHandle);
    }

    static void UniformDistributionOverTrack(float value, int numLanes, float trackLength, out int lane, out float trackProgress)
    {
        lane = (int)math.floor(value * numLanes);
        trackProgress = (value * numLanes - lane) * trackLength;
    }
}
