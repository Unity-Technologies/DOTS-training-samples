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
            var station = trackPoint.Station;
            var queuesBuffer = state.EntityManager.GetBuffer<StationQueuesElement>(station);

            foreach (var queueEntityElement in queuesBuffer)
            {
                // move passengers in queue
                var queue = queueEntityElement.Queue;
                var passengerElements = state.EntityManager.GetBuffer<QueuePassengers>(queue);
                var queueComponent = state.EntityManager.GetComponentData<QueueComponent>(queue);
                var queueLocation = state.EntityManager.GetComponentData<LocalTransform>(queue);

                // get queue direction
                var queueLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(queue);
                var queueDirection = queueLocalTransform.Forward();

                var passengerId = queueComponent.StartIndex;
                var queueLengthBeforeUpdate = queueComponent.QueueLength;

                for (var cnt = 0; cnt < queueLengthBeforeUpdate; cnt++)
                {
                    var passenger = passengerElements.ElementAt(passengerId).Passenger;
                    var passengerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(passenger);

                    // update passenger position
                    passengerLocalTransform.Position += queueDirection * config.PassengerSpeed * SystemAPI.Time.DeltaTime;
                    state.EntityManager.SetComponentData(passenger, passengerLocalTransform);
                    
                    // if passenger has passed queue location
                    if (math.dot(passengerLocalTransform.Position - queueLocation.Position, queueDirection) > 0)
                    {
                        // state.EntityManager.SetComponentEnabled<PassengerOnboarded>(passenger, true);
                        state.EntityManager.SetComponentEnabled<PassengerOffboarded>(passenger, true);
                        queueComponent.StartIndex = (queueComponent.StartIndex + 1) % config.MaxPassengerPerQueue;
                        queueComponent.QueueLength -= 1;
                    }
                    passengerId = (passengerId + 1) % config.MaxPassengerPerQueue;
                }

                state.EntityManager.SetComponentData(queue, queueComponent);
            }
        }
    }
}
