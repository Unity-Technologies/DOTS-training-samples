using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct TrainSpawnerSystem : ISystem
    {
        EntityQuery _baseColorQuery;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _baseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
            state.RequireForUpdate<Train>();
            state.RequireForUpdate<TrainPositions>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

                
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetNextTrainID(ref NativeArray<int> indexes, int metroLineID, int trainAmount, int trainID)
        {
            var startIndex = indexes[metroLineID];
            var lastIndex = startIndex + trainAmount - 1;
            var nextIndexOnLine = trainID - 1;
            if (nextIndexOnLine < startIndex)
                nextIndexOnLine = lastIndex;
            return nextIndexOnLine;
        }

        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var trainPositions = SystemAPI.GetSingletonRW<TrainPositions>();
            var baseColorQueryMask = _baseColorQuery.GetEntityQueryMask();
            
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
            
            var metroLineColors = new NativeArray<float4>(amountOfMetroLines, Allocator.Temp);
            foreach (var (metroLineId, metroLine) in SystemAPI.Query<MetroLineID, MetroLine>())
            {
                metroLineColors[metroLineId.ID] = (UnityEngine.Vector4)metroLine.Color;
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
                counter += amountOfTrainsOnLine[i];
            }

            trainPositions.ValueRW.Trains = new NativeArray<Entity>(amountOfTrains, Allocator.Persistent);
            trainPositions.ValueRW.TrainsPositions = new NativeArray<float3>(amountOfTrains, Allocator.Persistent);
            trainPositions.ValueRW.TrainsRotations = new NativeArray<quaternion>(amountOfTrains, Allocator.Persistent);

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var trainConfig = SystemAPI.GetSingleton<TrainConfig>();
            foreach (var (train, entity) in SystemAPI.Query<UniqueTrainID>().WithEntityAccess())
            {
                var metroLine = SystemAPI.GetComponent<MetroLineID>(entity);
                var trainIndex = SystemAPI.GetComponent<TrainIndexOnMetroLine>(entity);
                var uniqueID = trainPositions.ValueRW.StartIndexForMetroLine[metroLine.ID] + trainIndex.IndexOnMetroLine;
                var nextTrainID = GetNextTrainID(ref trainPositions.ValueRW.StartIndexForMetroLine, metroLine.ID, trainIndex.AmountOfTrainsOnMetroLine, uniqueID);
                SystemAPI.SetComponent(entity, new UniqueTrainID
                {
                    ID = uniqueID,
                    NextTrainID = nextTrainID
                });
                
                var worldTransform = SystemAPI.GetComponent<WorldTransform>(entity);
                trainPositions.ValueRW.Trains[train.ID] = entity;
                trainPositions.ValueRW.TrainsPositions[train.ID] = worldTransform.Position;
                trainPositions.ValueRW.TrainsRotations[train.ID] = worldTransform.Rotation;

                var carriages = CollectionHelper.CreateNativeArray<Entity>(trainConfig.CarriageCount, Allocator.Temp);
                ecb.Instantiate(trainConfig.CarriagePrefab, carriages);

                for (int i = 0; i < carriages.Length; i++)
                {
                    var carriageEntity = carriages[i];
                    var carriage = new Carriage
                    {
                        Index = i,
                        uniqueTrainID = uniqueID,
                        Train = entity
                    };
                    ecb.AddComponent(carriageEntity, carriage);
                    ecb.SetComponentForLinkedEntityGroup(carriageEntity, baseColorQueryMask, new URPMaterialPropertyBaseColor { Value = metroLineColors[metroLine.ID]});
                }
            }

            state.Enabled = false;
        }
    }
}