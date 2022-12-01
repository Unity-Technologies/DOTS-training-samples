using System.Security.Cryptography;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(TrainMovementSystem))]
    public partial struct MetroLineSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MetroLine>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var undesiredSpawnDirections1 = math.rotate(Quaternion.Euler(0,180,0), math.forward());
            var undesiredSpawnDirections2 = math.rotate(Quaternion.Euler(0,0,0), math.forward());
            
            var trainConfig = SystemAPI.GetSingleton<TrainConfig>();
            foreach (var (metroLine, id,entity) in SystemAPI.Query<MetroLine,MetroLineID>().WithEntityAccess())
            {
                var countTrains = 0;
                for (int i = 0; i < metroLine.RailwayPositions.Length; i++)
                {
                    if (metroLine.RailwayTypes[i] != RailwayPointType.Route)
                        continue;
                    countTrains++;
                }
                
                //Quick fix to not spawn on angles
                countTrains -= 2;

               
                var trainIndex = 0;
                for (int i = 0; i < metroLine.RailwayPositions.Length; i++)
                {
                    if (metroLine.RailwayTypes[i] != RailwayPointType.Route)
                        continue;

                    var pointRotation = metroLine.RailwayRotations[i].value;
                    var currentForward = math.rotate(pointRotation, math.forward());
                    if(math.dot(currentForward, undesiredSpawnDirections1) > 0.95f || 
                       math.dot(currentForward, undesiredSpawnDirections2)> 0.95f)
                        continue;
                    
                    var train = ecb.Instantiate(trainConfig.TrainPrefab);
                    ecb.SetComponent(train, LocalTransform.FromPositionRotation(metroLine.RailwayPositions[i], metroLine.RailwayRotations[i]));
                    var nextIndex = i - 1 < 0 ? metroLine.RailwayPositions.Length - 1 : i - 1;
                    ecb.SetComponent(train, new Train
                    {
                        DestinationIndex = nextIndex,
                        Destination = metroLine.RailwayPositions[nextIndex],
                        DestinationType = metroLine.RailwayTypes[nextIndex],
                        
                        MetroLine = entity
                    });
                    ecb.SetComponent(train, new TrainStateComponent
                    {
                        State = TrainState.EnRoute
                    });
                    ecb.SetComponent(train, new MetroLineID
                    {
                        ID = id.ID
                    });
                    ecb.SetComponent(train, new TrainIndexOnMetroLine
                    {
                        IndexOnMetroLine = trainIndex,
                        AmountOfTrainsOnMetroLine = countTrains,
                    });
                    trainIndex++;
                }
            }

            state.Enabled = false;
        }
    }
}