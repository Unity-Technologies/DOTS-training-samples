using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PassangerOnTrainSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        // onboarding
        foreach (var (train, trainTransform) in
                 SystemAPI.Query<RefRO<Train>, RefRO<LocalTransform>>())
        {
            DynamicBuffer<TrackPoint> trackPointsBuffer = state.EntityManager.GetBuffer<TrackPoint>(train.ValueRO.TrackEntity);
            TrackPoint trackPoint = trackPointsBuffer.ElementAt(train.ValueRO.TrackPointIndex);
            var station = trackPoint.Station;

            foreach (var (passengerOnboardedComp, transform, passenger) in
                SystemAPI.Query<RefRW<PassengerOnboarded>, RefRW<LocalTransform>, RefRW<PassengerComponent>>())
            {
                if (passengerOnboardedComp.ValueRO.StationTrackPointIndex == train.ValueRO.TrackPointIndex)
                {
                    // the passenger is on the same station as the train
                    // store the trainID
                    passenger.ValueRW.TrainId = train.ValueRO.TrainId;
                }

                if (passenger.ValueRO.TrainId == train.ValueRO.TrainId)
                {
                    transform.ValueRW.Position = trainTransform.ValueRO.Position;
                }
            }
        }

        //offboarding
        foreach (var train in SystemAPI.Query<RefRO<Train>>().WithAll<UnloadingComponent>())
        {
            foreach (var (passengerOnboardedComp, transform, passenger, entity) in
                SystemAPI.Query<RefRW<PassengerOnboarded>, RefRW<LocalTransform>, RefRW<PassengerComponent>>().WithEntityAccess())
            {
                state.EntityManager.SetComponentEnabled<PassengerOnboarded>(entity, false);
                transform.ValueRW.Position += new float3(0, 1, 0);
            }
        }
    }
}
