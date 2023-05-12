using System.Runtime.CompilerServices;
using Components;
using Unity.Burst;
using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(PassengerSystemGroup))]
public partial struct PassangerOnTrainSystem : ISystem
{
    private Random random;
    NativeHashMap<TrainLookup, TrainComponents> m_trainMap;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<StationConfig>();
        random = Random.CreateFromIndex(930);
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (m_trainMap.IsCreated)
            m_trainMap.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;
        var stationConfig = SystemAPI.GetSingleton<StationConfig>();
        var config = SystemAPI.GetSingleton<Config>();
        
        if (!m_trainMap.IsCreated)
            m_trainMap = new NativeHashMap<TrainLookup, TrainComponents>(stationConfig.NumLines * 2, Allocator.Persistent);
        
        foreach (var (train, trainTransform, trainEntity) in
                 SystemAPI.Query<RefRO<Train>, RefRO<LocalTransform>>()
                     .WithEntityAccess())
        {
            var lookup = new TrainLookup()
            {
                LineID = train.ValueRO.LineID,
                OnPlatformA = train.ValueRO.OnPlatformA
            };
            var components = new TrainComponents()
            {
                Train = train,
                Transform = trainTransform,
                TrainEntity = trainEntity,
                IsUnloading = em.IsComponentEnabled<UnloadingComponent>(trainEntity)
            };
            m_trainMap[lookup] = components;
        }
        

        // onboarding
        foreach (var (passengerOnboardedComp, transform, passenger, travelInfo, passengerEntity) in
                 SystemAPI.Query<RefRW<PassengerOnboarded>, RefRW<LocalTransform>, RefRW<PassengerComponent>, RefRO<PassengerTravel>>()
                     .WithNone<PassengerWalkingToSeat>()
                     .WithNone<PassengerWaitingToExit>()
                     .WithEntityAccess())
        {
            if (!m_trainMap.TryGetValue(new TrainLookup() { LineID = travelInfo.ValueRO.LineID, OnPlatformA = travelInfo.ValueRO.OnPlatformA }, out var trainComponents))
                continue;

            var train = trainComponents.Train;
            var trainTransform = trainComponents.Transform;
            var trainEntity = trainComponents.TrainEntity;
            
            if (passengerOnboardedComp.ValueRO.StationTrackPointIndex == train.ValueRO.TrackPointIndex &&
                passenger.ValueRO.TrainId != train.ValueRO.TrainId &&
                travelInfo.ValueRO.LineID == train.ValueRO.LineID &&
                travelInfo.ValueRO.OnPlatformA == train.ValueRO.OnPlatformA)
            {
                // the passenger is on the same station as the train
                // store the trainID
                passenger.ValueRW.TrainId = train.ValueRO.TrainId;
                
                // find closest seat
                DynamicBuffer<SeatingComponentElement> seatBuffer = state.EntityManager.GetBuffer<SeatingComponentElement>(trainEntity);
                if (!passenger.ValueRW.HasSeat)
                {
                    var relativePosition = transform.ValueRO.Position - trainTransform.ValueRO.Position;
                    var closestSeatIndex = GetClosestSeatIndex(seatBuffer, relativePosition);
                    if (closestSeatIndex >= 0)
                    {
                        passenger.ValueRW.SeatPosition = seatBuffer[closestSeatIndex].SeatPosition;
                        passenger.ValueRW.SeatIndex = closestSeatIndex;
                        passenger.ValueRW.HasSeat = true;

                        var seat = seatBuffer.ElementAt(closestSeatIndex);
                        seat.Occupied = true;
                        seatBuffer[closestSeatIndex] = seat;

                        passenger.ValueRW.RelativePosition = relativePosition;
                    }
                }

                if (passenger.ValueRW.HasSeat)
                {
                    em.SetComponentEnabled<PassengerWalkingToSeat>(passengerEntity, true);
                }
                else
                {
                    // Offboarded
                    em.SetComponentEnabled<PassengerOnboarded>(passengerEntity, false);
                    em.SetComponentEnabled<PassengerOffboarded>(passengerEntity, true);
                }
            }
        }

        // onboard moving
        foreach (var (passengerOnboardedComp, transform, passenger, travelInfo) in
                 SystemAPI.Query<RefRW<PassengerOnboarded>, RefRW<LocalTransform>, RefRW<PassengerComponent>, RefRO<PassengerTravel>>())
        {
            if (!m_trainMap.TryGetValue(new TrainLookup() { LineID = travelInfo.ValueRO.LineID, OnPlatformA = travelInfo.ValueRO.OnPlatformA }, out var trainComponents))
                continue;

            var train = trainComponents.Train;
            var trainTransform = trainComponents.Transform;
            
            if ( passenger.ValueRW.TrainId == train.ValueRO.TrainId)
            {
                transform.ValueRW.Position = passenger.ValueRW.RelativePosition + trainTransform.ValueRO.Position;
                passenger.ValueRW.HasJustUnloaded = true;
            }
        }

        foreach (var (transform, passenger, travelInfo, entity) in
                 SystemAPI.Query< RefRW<LocalTransform>, RefRW<PassengerComponent>, RefRW<PassengerTravel>>()
                     .WithAll<PassengerWaitingToExit>()
                     .WithEntityAccess())
        {
            if (!m_trainMap.TryGetValue(new TrainLookup() { LineID = travelInfo.ValueRO.LineID, OnPlatformA = travelInfo.ValueRO.OnPlatformA }, out var trainComponents))
                continue;
            
            if (!trainComponents.IsUnloading)
                continue;

            var train = trainComponents.Train;
            var trainTransform = trainComponents.Transform;
            
            if (passenger.ValueRW.TrainId == train.ValueRO.TrainId)
            {
                transform.ValueRW.Position = trainTransform.ValueRO.Position + passenger.ValueRO.ExitPosition;
                travelInfo.ValueRW.Station = train.ValueRO.StationEntity;

                passenger.ValueRW.TrainId = -1;
                em.SetComponentEnabled<PassengerOnboarded>(entity, false);
                em.SetComponentEnabled<PassengerWaitingToExit>(entity, false);
                em.SetComponentEnabled<PassengerOffboarded>(entity, true);
            }
        }

        foreach (var (transform, passenger, travelInfo, entity) in
                 SystemAPI.Query< RefRW<LocalTransform>, RefRW<PassengerComponent>, RefRW<PassengerTravel>>()
                     .WithAll<PassengerWaitingToExit>()
                     .WithEntityAccess())
        {
            if (!m_trainMap.TryGetValue(new TrainLookup() { LineID = travelInfo.ValueRO.LineID, OnPlatformA = travelInfo.ValueRO.OnPlatformA }, out var trainComponents))
                continue;
            
            var train = trainComponents.Train;
            var trainTransform = trainComponents.Transform;
            
            if (!trainComponents.IsUnloading || !(train.ValueRO.Duration < 0.01f))
                continue;
            
            if (passenger.ValueRW.TrainId == train.ValueRO.TrainId)
            {
                transform.ValueRW.Position = trainTransform.ValueRO.Position + passenger.ValueRO.ExitPosition;

                travelInfo.ValueRW.Station = train.ValueRO.StationEntity;
                em.SetComponentEnabled<PassengerOnboarded>(entity, false);
                em.SetComponentEnabled<PassengerWaitingToExit>(entity, false);
                em.SetComponentEnabled<PassengerOffboarded>(entity, true);
            }
        }

        foreach (var kvPair in m_trainMap)
        {
            if (!kvPair.Value.IsUnloading || !(kvPair.Value.Train.ValueRO.Duration < 0.01f))
                continue;
            
            DynamicBuffer<SeatingComponentElement> seatBuffer = state.EntityManager.GetBuffer<SeatingComponentElement>(kvPair.Value.TrainEntity);
            for (int i = 0; i < seatBuffer.Length; i++)
            {
                if (random.NextBool())
                {
                    seatBuffer.ElementAt(i).Occupied = false;
                }
            }
        }

        foreach (var (transform, passenger, travelInfo, entity) in
                 SystemAPI.Query< RefRW<LocalTransform>, RefRW<PassengerComponent>, RefRO<PassengerTravel>>()
                     .WithAll<PassengerOnboarded>()
                     .WithNone<PassengerWalkingToSeat>()
                     .WithEntityAccess())
        {
            if (!m_trainMap.TryGetValue(new TrainLookup() { LineID = travelInfo.ValueRO.LineID, OnPlatformA = travelInfo.ValueRO.OnPlatformA }, out var trainComponents))
                continue;
            
            var train = trainComponents.Train;
            var trainTransform = trainComponents.Transform;
            
            if (!trainComponents.IsUnloading || !(train.ValueRO.Duration < 0.01f))
                continue;
            
            DynamicBuffer<SeatingComponentElement> seatBuffer = state.EntityManager.GetBuffer<SeatingComponentElement>(trainComponents.TrainEntity);
            if (passenger.ValueRW.TrainId == train.ValueRO.TrainId)
            {
                if (passenger.ValueRO.HasSeat && !seatBuffer.ElementAt(passenger.ValueRO.SeatIndex).Occupied)
                {
                    passenger.ValueRW.HasSeat = false;
                    passenger.ValueRW.SeatIndex = -1;

                    em.SetComponentEnabled<PassengerWalkingToDoor>(entity, true);
                    SetPassengerTargetExitPosition(stationConfig, config, passenger, trainTransform.ValueRO, train);
                }
            }
        }
    }

    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPassengerTargetExitPosition(StationConfig stationConfig, Config config, RefRW<PassengerComponent> passenger, LocalTransform trainLocalTransform, RefRO<Train> train)
    {
        var halfSpan = config.CarriageLength * (stationConfig.NumQueingPointsPerPlatform - 1f) / 2f;
        var minDistance = float.MaxValue;
        float3 targetExitPosition = float3.zero;

        var negX = 1f;
        if (train.ValueRO.OnPlatformA)
        {
            negX = train.ValueRO.Forward ? 1f: -1f;
        }
        else
        {
            negX = train.ValueRO.Forward ? -1f: 1f;
        }
        for (int i = 0; i < stationConfig.NumQueingPointsPerPlatform; i++)
        {
            var exitPosition = negX * 0.67f * trainLocalTransform.Right() + 1.6f * trainLocalTransform.Up() + 
                               (i * config.CarriageLength - halfSpan) * trainLocalTransform.Forward();
            var distToExit = math.distance(passenger.ValueRO.SeatPosition, exitPosition);
            
            if (distToExit <= minDistance)
            {
                minDistance = distToExit;
                targetExitPosition = exitPosition;
            }

            passenger.ValueRW.ExitPosition = targetExitPosition;
        }
        
    }
    
    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetClosestSeatIndex(DynamicBuffer<SeatingComponentElement> seatBuffer, float3 passengerRelativePosition)
    {
        var minDist = float.MaxValue;
        var seatIndex = -1;
        
        for (int i = 0; i < seatBuffer.Length; i++)
        {
            if (seatBuffer[i].Occupied)
            {
                continue;
            }

            var dist = math.distance(passengerRelativePosition, seatBuffer[i].SeatPosition);

            if (dist < minDist)
            {
                seatIndex = i;
                minDist = dist;
            }
        }
        
        return seatIndex;
    }
}


