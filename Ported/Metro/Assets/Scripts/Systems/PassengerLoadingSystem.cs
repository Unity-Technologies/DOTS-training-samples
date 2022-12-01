using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(TrainMovementSystem))]
    public partial struct PassengerLoadingSystem : ISystem
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
            foreach (var (trainState, train, uniqueTrainID,trainEntity) in SystemAPI.Query<TrainStateComponent, Train, UniqueTrainID>().WithEntityAccess())
            {
                switch (trainState.State)
                {
                    case TrainState.Arrived:
                    {
                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        foreach (var (platformId, platformEntity) in SystemAPI.Query<PlatformId>().WithAll<Platform>().WithEntityAccess())
                        {
                            if (platformId.Value != platformID) continue;
                            SystemAPI.SetComponent(platformEntity, new TrainOnPlatform
                            {
                                Train = trainEntity
                            });
                            SystemAPI.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.Unloading });
                            break;
                        }

                        break;
                    }
                    case TrainState.Unloading:
                    {
                        SystemAPI.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.Loading });
                        break;
                    }
                    case TrainState.Loading:
                    {
                        var trainConfig = SystemAPI.GetSingleton<TrainConfig>();
                        var carriageInfos = new NativeArray<(Carriage, float3, NativeArray<float3>, DynamicBuffer<CarriagePassengers>)>(trainConfig.CarriageCount, Allocator.Temp);
                        var counter = 0;
                        foreach (var (carriage, transform,seatsPositions, seats) in SystemAPI.Query<Carriage, WorldTransform,  CarriageSeatsPositions, DynamicBuffer<CarriagePassengers>>())
                        {
                            if (carriage.Train != trainEntity) continue;
                            carriageInfos[counter] = (carriage, transform.Position,seatsPositions.Seats, seats);
                            counter++;
                        }
                        
                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        foreach (var (passenger, platformId, queueId,waypoints, passengerEntity) in SystemAPI.Query<Passenger, PlatformId, PlatformQueueId,DynamicBuffer<Waypoint>>().WithEntityAccess())
                        {
                            if (platformId.Value != platformID || passenger.State != PassengerState.InQueue) continue;
                            
                            var carriageInfo = carriageInfos[queueId.Value];
                            var seats = carriageInfo.Item4;
                            var occupiedSeats = seats.Length;
                            if (occupiedSeats >= 27)
                                continue;
                            var seatPositions = carriageInfo.Item3;
                            var freeSeatPosition = seatPositions[occupiedSeats];
                            waypoints.Add(new Waypoint { Value = carriageInfo.Item2 }); //carriage center
                            waypoints.Add(new Waypoint { Value = carriageInfo.Item2 +  freeSeatPosition}); //seat pos
                                
                            SystemAPI.SetComponent(passengerEntity, new Passenger { State = PassengerState.OnBoarding });
                        }

                        SystemAPI.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.WaitingOnPlatform });
                        break;
                    }
                    case TrainState.WaitingOnPlatform:
                    {
                        SystemAPI.SetComponent(trainEntity, new TrainStateComponent
                        {
                            State = TrainState.Departing
                        });
                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        foreach (var (platformId, platformEntity) in SystemAPI.Query<PlatformId>().WithAll<Platform>().WithEntityAccess())
                        {
                            if (platformId.Value != platformID)
                                continue;
                            SystemAPI.SetComponent(platformEntity, new TrainOnPlatform { Train = Entity.Null });
                        }

                        break;
                    }
                }
            }
        }
    }
}