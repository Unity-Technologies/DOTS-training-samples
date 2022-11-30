using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct TrainSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Train>();
            state.RequireForUpdate<TrainPositions>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var trainPositions = SystemAPI.GetSingletonRW<TrainPositions>();
            var amountOfTrains = 0;
            foreach (var train in SystemAPI.Query<Train>())
            {
                amountOfTrains++;
            }

            var amountOfMetroLines = 0;
            foreach (var metroLine in SystemAPI.Query<MetroLineID>().WithAll<MetroLine>())
            {
                amountOfMetroLines++;
            }

            var amountOfTrainsOnLine = new NativeArray<int>(amountOfMetroLines, Allocator.Temp);
            foreach (var id in SystemAPI.Query<MetroLineID>().WithAll<Train>())
            {
                amountOfTrainsOnLine[id.ID]++;
            }

            trainPositions.ValueRW.StartIndexForMetroLine = new NativeArray<int>(amountOfMetroLines, Allocator.Persistent);
            var counter = 0;
            for (int i = 0; i < amountOfMetroLines; i++)
            {
                trainPositions.ValueRW.StartIndexForMetroLine[i] = counter;
                counter = amountOfTrainsOnLine[i];
            }

            trainPositions.ValueRW.Trains = new NativeArray<Entity>(amountOfTrains, Allocator.Persistent);
            trainPositions.ValueRW.TrainsPositions = new NativeArray<float3>(amountOfTrains, Allocator.Persistent);
            trainPositions.ValueRW.TrainsRotations = new NativeArray<quaternion>(amountOfTrains, Allocator.Persistent);

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var trainConfig = SystemAPI.GetSingleton<TrainConfig>();
            foreach (var (train, entity) in SystemAPI.Query<Train>().WithEntityAccess())
            {
                var worldTransform = SystemAPI.GetComponent<WorldTransform>(entity);
                trainPositions.ValueRW.Trains[train.UniqueTrainID] = entity;
                trainPositions.ValueRW.TrainsPositions[train.UniqueTrainID] = worldTransform.Position;
                trainPositions.ValueRW.TrainsRotations[train.UniqueTrainID] = worldTransform.Rotation;

                var carriages = CollectionHelper.CreateNativeArray<Entity>(trainConfig.CarriageCount, Allocator.Temp);
                ecb.Instantiate(trainConfig.CarriagePrefab, carriages);

                for (int i = 0; i < carriages.Length; i++)
                {
                    var carriageEntity = carriages[i];
                    var carriage = new Carriage
                    {
                        Index = i,
                        uniqueTrainID = train.UniqueTrainID,
                        Train = entity
                    };
                    ecb.AddComponent(carriageEntity, carriage);
                }
            }

            state.Enabled = false;
        }
    }
}