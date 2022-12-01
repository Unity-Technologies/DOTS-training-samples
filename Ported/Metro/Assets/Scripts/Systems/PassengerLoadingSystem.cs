using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(TrainMovementSystem))]
    public partial struct PassengerLoadingSystem : ISystem
    {
        BufferLookup<Waypoint> m_WaypointLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Train>();
            state.RequireForUpdate<TrainPositions>();
            m_WaypointLookup = state.GetBufferLookup<Waypoint>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_WaypointLookup.Update(ref state);
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (trainState, train, uniqueTrainID, trainEntity) in SystemAPI.Query<TrainStateComponent, Train, UniqueTrainID>().WithEntityAccess())
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
                            ecb.SetComponent(platformEntity, new TrainOnPlatform
                            {
                                Train = trainEntity
                            });
                            ecb.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.Unloading });
                            break;
                        }

                        break;
                    }
                    case TrainState.Unloading:
                    {
                        var trainConfig = SystemAPI.GetSingleton<TrainConfig>();
                        var carriageInfos = new NativeArray<(Carriage, float3, DynamicBuffer<CarriagePassengers>, quaternion)>(trainConfig.CarriageCount, Allocator.Temp);
                        var counter = 0;
                        foreach (var (carriage, transform, seats) in SystemAPI.Query<Carriage, WorldTransform, DynamicBuffer<CarriagePassengers>>())
                        {
                            if (carriage.Train != trainEntity) continue;
                            carriageInfos[counter] = (carriage, transform.Position, seats, transform.Rotation);
                            counter++;
                        }

                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];

                        Debug.Log($"Train: {uniqueTrainID.ID} PlatformID: {platformID}");

                        var random = new Random(4221);
                        for (int i = 0; i < carriageInfos.Length; i++)
                        {
                            var carriagePosition = carriageInfos[i].Item2;
                            var carriageRotation = carriageInfos[i].Item4;
                            var doorPosition = carriagePosition + math.rotate(carriageRotation, math.right());
                            var passengers = carriageInfos[i].Item3;
                            var passengerFromCart = passengers.AsNativeArray();

                            for (int j = 0; j < passengerFromCart.Length; j++)
                            {
                                var passengerEntity = passengerFromCart[j].Value;
                                var waypoints = m_WaypointLookup[passengerEntity];
                                m_WaypointLookup.SetBufferEnabled(passengerEntity, true);
                                ecb.SetComponent(passengerEntity, new Passenger { State = PassengerState.OffBoarding });
                                ecb.SetComponent(passengerEntity, new PlatformId { Value = platformID });
                                waypoints.Add(new Waypoint { Value = carriagePosition }); //carriage center
                                waypoints.Add(new Waypoint { Value = doorPosition }); //door
                                waypoints.Add(new Waypoint { Value = carriagePosition + math.rotate(carriageRotation, new float3(2, 0, random.NextFloat(1, 2))) });
                            }

                            passengerFromCart.Dispose();
                            passengers.Clear();
                        }

                        ecb.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.WaitingForUnloading });
                        break;
                    }
                    case TrainState.WaitingForUnloading:
                    {
                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        var someStillUnload = false;
                        foreach (var (passenger, platformId) in SystemAPI.Query<Passenger, PlatformId>())
                        {
                            if (platformId.Value == platformID && passenger.State == PassengerState.OffBoarding)
                            {
                                someStillUnload = true;
                                break;
                            }
                        }

                        if (!someStillUnload)
                            ecb.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.Loading });
                        break;
                    }
                    case TrainState.Loading:
                    {
                        var trainConfig = SystemAPI.GetSingleton<TrainConfig>();
                        var carriageInfos = new NativeArray<(Carriage, float3, NativeArray<float3>, quaternion, int)>(trainConfig.CarriageCount, Allocator.Temp);
                        var counter = 0;
                        foreach (var (carriage, transform, seatsPositions) in SystemAPI.Query<Carriage, WorldTransform, CarriageSeatsPositions>())
                        {
                            if (carriage.Train != trainEntity) continue;
                            carriageInfos[counter] = (carriage, transform.Position, seatsPositions.Seats, transform.Rotation, 0);
                            counter++;
                        }

                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        foreach (var (passenger, platformId, queueId, waypoints, passengerEntity)
                                 in SystemAPI.Query<Passenger, PlatformId, PlatformQueueId, DynamicBuffer<Waypoint>>()
                                     .WithEntityAccess())
                        {
                            if (platformId.Value != platformID || passenger.State != PassengerState.InQueue) continue;

                            var carriageInfo = carriageInfos[queueId.Value];
                            var seatPositions = carriageInfo.Item3;
                            if (carriageInfo.Item5 == seatPositions.Length)
                                continue;
                            var freeSeatPosition = seatPositions[carriageInfo.Item5];
                            ecb.SetComponent(passengerEntity, new PassengerSeatIndex { Index = carriageInfo.Item5 });
                            ecb.SetComponent(passengerEntity, new Passenger { State = PassengerState.OnBoarding });
                            waypoints.Add(new Waypoint { Value = carriageInfo.Item2 }); //carriage center
                            waypoints.Add(new Waypoint { Value = carriageInfo.Item2 + math.rotate(carriageInfo.Item4, freeSeatPosition) }); //seat pos

                            carriageInfo.Item5++;
                            carriageInfos[queueId.Value] = carriageInfo;
                        }

                        ecb.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.WaitingForLoading });
                        break;
                    }
                    case TrainState.WaitingForLoading:
                    {
                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        var someStillOnBoard = false;
                        foreach (var (passenger, platformId) in SystemAPI.Query<Passenger, PlatformId>())
                        {
                            if (platformId.Value == platformID && passenger.State == PassengerState.OnBoarding)
                            {
                                someStillOnBoard = true;
                                break;
                            }
                        }

                        if (!someStillOnBoard)
                            ecb.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.WaitingOnPlatform });
                        break;
                    }
                    case TrainState.WaitingOnPlatform:
                    {
                        var trainConfig = SystemAPI.GetSingleton<TrainConfig>();
                        var carriageInfos = new NativeArray<(Carriage, DynamicBuffer<CarriagePassengers>)>(trainConfig.CarriageCount, Allocator.Temp);
                        var counter = 0;
                        foreach (var (carriage, buffer) in SystemAPI.Query<Carriage, DynamicBuffer<CarriagePassengers>>())
                        {
                            if (carriage.Train != trainEntity) continue;
                            carriageInfos[counter] = (carriage, buffer);
                            counter++;
                        }

                        var metroLine = SystemAPI.GetComponent<MetroLine>(train.MetroLine);
                        var platformID = metroLine.Platforms[train.DestinationIndex];
                        foreach (var (passenger, platformId, queueId, seatIndex, passengerEntity) in SystemAPI.Query<Passenger, PlatformId, PlatformQueueId, PassengerSeatIndex>().WithEntityAccess())
                        {
                            if (platformId.Value == platformID && passenger.State == PassengerState.FinishBoarding)
                            {
                                var carriageInfo = carriageInfos[queueId.Value];
                                carriageInfo.Item2.Insert(seatIndex.Index, new CarriagePassengers { Value = passengerEntity });
                                ecb.SetComponent(passengerEntity, new Passenger { State = PassengerState.Seated });
                            }
                        }

                        ecb.SetComponent(trainEntity, new TrainStateComponent { State = TrainState.Departing });
                        foreach (var (platformId, platformEntity) in SystemAPI.Query<PlatformId>().WithAll<Platform>().WithEntityAccess())
                        {
                            if (platformId.Value != platformID)
                                continue;
                            ecb.SetComponent(platformEntity, new TrainOnPlatform { Train = Entity.Null });
                        }

                        break;
                    }
                }
            }
        }
    }
}