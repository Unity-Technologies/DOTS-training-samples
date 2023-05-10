using Components;
using Metro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PassengerSpawningSystem))]
public partial struct QueingPassengersSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        
        foreach (var train in
                 SystemAPI.Query<RefRO<Train>>()
                     .WithAll<LoadingComponent>())
        {
            var trackPointsBuffer = state.EntityManager.GetBuffer<TrackPoint>(train.ValueRO.TrackEntity);
            var trackPoint = trackPointsBuffer.ElementAt(train.ValueRO.TrackPointIndex);
            var station = trackPoint.StationEntity;
            var queuesBuffer = state.EntityManager.GetBuffer<StationQueuesElement>(station);

            foreach (var queueEntityElement in queuesBuffer)
            {
                // move passengers in queue
                var queue = queueEntityElement.Queue;
                var passengerElements = state.EntityManager.GetBuffer<QueuePassengers>(queue);
                var queueComponent = state.EntityManager.GetComponentData<QueueComponent>(queue);
                var queueLocation = state.EntityManager.GetComponentData<LocalTransform>(queue);

                // get passenger number in queue
                var passengerNumInQueue = queueComponent.StartIndex <= queueComponent.EndEndex
                    ? queueComponent.EndEndex - queueComponent.StartIndex + 1
                    : 16 - queueComponent.StartIndex + queueComponent.EndEndex + 1;
                
                // get queue direction
                var queueLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(queue);
                var queueDirection = queueLocalTransform.Forward();

                var passengerId = 0;
                for (var cnt = 0; cnt < passengerNumInQueue; cnt++)
                {
                    passengerId = (queueComponent.StartIndex + cnt) % 16;
                    var passenger = passengerElements.ElementAt(passengerId).Passenger;
                    var passengerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(passenger);

                    passengerLocalTransform.Position += queueDirection * config.PassengerSpeed * SystemAPI.Time.DeltaTime;

                    state.EntityManager.SetComponentData(passenger, passengerLocalTransform);
                }

                passengerId = queueComponent.StartIndex;
                for (var cnt = 0; cnt < passengerNumInQueue; cnt++)
                {
                    var passenger = passengerElements.ElementAt(passengerId).Passenger;
                    var passengerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(passenger);

                    if (math.dot(passengerLocalTransform.Position - queueLocation.Position, queueDirection) > 0)
                    {
                        state.EntityManager.SetComponentEnabled<PassengerOnboarded>(passenger, true);
                        queueComponent.StartIndex = (queueComponent.StartIndex + 1) % 16;
                    }

                    passengerId = (passengerId + 1) % 16;
                }
                state.EntityManager.SetComponentData(queue, queueComponent);
            }
        }
    }
}
