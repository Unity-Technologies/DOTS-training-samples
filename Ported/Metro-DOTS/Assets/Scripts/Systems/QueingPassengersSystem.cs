using Unity.Entities;

[UpdateAfter(typeof(PassengerSpawningSystem))]
public partial struct QueingPassengersSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var train in
             SystemAPI.Query<RefRO<Train>>())
        {
            foreach (var (queueComponent, queuePassengers) in
                     SystemAPI.Query<RefRW<QueueComponent>, DynamicBuffer<QueuePassengers>>())
            {
            }

        }
    }
}
