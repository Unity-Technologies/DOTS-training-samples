using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CarSpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
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
                    DefaultSpeed = random.NextFloat(spawner.MaxDefaultSpeed),
                    OvertakeSpeed = random.NextFloat(spawner.MaxOvertakeSpeed),
                    DistanceToCarBeforeOvertaking = random.NextFloat(spawner.MaxDistanceToCarBeforeOvertaking),
                    OvertakeEagerness = random.NextFloat(spawner.MaxOvertakeEagerness),
                    MergeSpace = random.NextFloat(spawner.MaxMergeSpace),
                });

                commandBuffer.SetComponent(newCar, new Speed
                {
                    Value = 0
                });

                var pointOnInterval = (float)i / (float)numberToBeSpawned;
                UniformDistributionOverTrack(pointOnInterval, 4, out var targetLane, out var targetTrackProgress);
                commandBuffer.SetComponent(newCar, new TrackPosition
                {
                    Lane = (float)targetLane,
                    TrackProgress = targetTrackProgress,
                });

                commandBuffer.DestroyEntity(entity);
            }
        }).Schedule(Dependency);

        Dependency = jobHandle;

        barrier.AddJobHandleForProducer(jobHandle);
    }

    static void UniformDistributionOverTrack(float value, int numLanes, out int lane, out float trackProgress)
    {
        lane = (int)math.floor(value * numLanes);
        trackProgress = value * numLanes - lane;
    }
}
