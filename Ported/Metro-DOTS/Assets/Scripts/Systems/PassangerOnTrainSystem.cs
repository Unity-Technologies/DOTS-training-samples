using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PassangerOnTrainSystem : ISystem
{
    public void OnCreate(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        // onboarding
        foreach (var (train, trainTransform, entity) in
                 SystemAPI.Query<RefRO<Train>, RefRO<LocalTransform>>()
                 .WithAll<LoadingComponent>()
                 .WithEntityAccess())
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

                // find closest seat
                DynamicBuffer<SeatingComponentElement> seatBuffer = state.EntityManager.GetBuffer<SeatingComponentElement>(entity);
                for (int i = 0; i < seatBuffer.Length; i++)
                {
                    if (seatBuffer[i].Occupied || passenger.ValueRW.HasSeat)
                    {
                        continue;
                    }

                    passenger.ValueRW.SeatPosition = seatBuffer[i].SeatPosition;
                    passenger.ValueRW.HasSeat = true;

                    var seat = seatBuffer.ElementAt(i);
                    seat.Occupied = true;
                    seatBuffer[i] = seat;
                }

                if (passenger.ValueRW.HasSeat)
                {
                    transform.ValueRW.Position = passenger.ValueRW.SeatPosition + trainTransform.ValueRO.Position;
                }
                else
                {
                    // Offboarded
                    em.SetComponentEnabled<PassengerOnboarded>(entity, false);
                    em.SetComponentEnabled<PassengerOffboarded>(entity, true);
                }
            }
        }

        // onboard moving
        foreach (var (train, trainTransform) in
                 SystemAPI.Query<RefRO<Train>, RefRO<LocalTransform>>()
                 .WithAll<EnRouteComponent>())
        {
            foreach (var (passengerOnboardedComp, transform, passenger) in
                SystemAPI.Query<RefRW<PassengerOnboarded>, RefRW<LocalTransform>, RefRW<PassengerComponent>>())
            {
                if (passenger.ValueRW.HasSeat)
                {
                    transform.ValueRW.Position = passenger.ValueRW.SeatPosition + trainTransform.ValueRO.Position;
                    passenger.ValueRW.HasJustUnloaded = true;
                }
            }
        }

        //offboarding
        foreach (var (train, trainEntity) in SystemAPI.Query<RefRO<Train>>().WithAll<UnloadingComponent>().WithEntityAccess())
        {
            DynamicBuffer<SeatingComponentElement> seatBuffer = state.EntityManager.GetBuffer<SeatingComponentElement>(trainEntity);
            for (int i = 0; i < seatBuffer.Length; i++)
            {
                seatBuffer.ElementAt(i).Occupied = false;
            }

            foreach (var (passengerOnboardedComp, transform, passenger, entity) in
                     SystemAPI.Query<RefRW<PassengerOnboarded>, RefRW<LocalTransform>, RefRW<PassengerComponent>>().WithEntityAccess())
            {
                float minDistance = float.MaxValue;
                foreach (var (queue, queueTr, queueEntity) in
                    SystemAPI.Query<RefRW<QueueComponent>, RefRW<LocalTransform>>().WithEntityAccess())
                {
                    float dist = math.distance(queueTr.ValueRO.Position, transform.ValueRO.Position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        passenger.ValueRW.TargetPosition = queueTr.ValueRO.Position;
                        passenger.ValueRW.MoveToPosition = true;
                        passenger.ValueRW.HasSeat = false;
                    }
                }

                transform.ValueRW.Position = passenger.ValueRW.TargetPosition;

                var travelInfo = em.GetComponentData<PassengerTravel>(entity);
                travelInfo.Station = train.ValueRO.StationEntity;
                em.SetComponentData(entity, travelInfo);
                em.SetComponentEnabled<PassengerOnboarded>(entity, false);
                em.SetComponentEnabled<PassengerOffboarded>(entity, true);
            }
        }
    }
}


