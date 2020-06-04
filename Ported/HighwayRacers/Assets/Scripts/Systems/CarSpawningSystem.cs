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
        CarConfigurations carConfig = GetSingleton<CarConfigurations>();

        var barrier = m_EntityCommandBufferSystem;
        var commandBuffer = barrier.CreateCommandBuffer();

        var random = new Unity.Mathematics.Random(1);
        var jobHandle = Entities.ForEach((in Entity entity, in CarSpawnRequest request) => 
        {
            var numberToBeSpawned = request.InstancesToSpawn;
            var carToBeSpawned = carConfig.CarPrefab;

            for (int i = 0; i < numberToBeSpawned; ++i)
            {
                var newCar = commandBuffer.Instantiate(carToBeSpawned);
                commandBuffer.SetComponent(newCar, new CarProperties
                {
                    DefaultSpeed = carConfig.MinDefaultSpeed + random.NextFloat(carConfig.MaxDefaultSpeed - carConfig.MinDefaultSpeed),
                    OvertakeSpeed = carConfig.MinOvertakeSpeed + random.NextFloat(carConfig.MaxOvertakeSpeed - carConfig.MinOvertakeSpeed),
                    DistanceToCarBeforeOvertaking = carConfig.MinDistanceToCarBeforeOvertaking + random.NextFloat(carConfig.MaxDistanceToCarBeforeOvertaking - carConfig.MinDistanceToCarBeforeOvertaking),
                    OvertakeEagerness = carConfig.MinOvertakeEagerness + random.NextFloat(carConfig.MaxOvertakeEagerness - carConfig.MinOvertakeEagerness),
                    MergeSpace = carConfig.MinMergeSpace + random.NextFloat(carConfig.MaxMergeSpace - carConfig.MinMergeSpace),
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

                commandBuffer.AddSharedComponent(newCar, new TrackGroup
                {
                    Index = TrackGroup.LaneValueToTrackGroupIdx(targetLane)
                });

                commandBuffer.AddComponent(newCar, new CarInFront
                {
                    TrackProgressCarInFront = float.MaxValue,
                    Speed = float.MaxValue
                });

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
