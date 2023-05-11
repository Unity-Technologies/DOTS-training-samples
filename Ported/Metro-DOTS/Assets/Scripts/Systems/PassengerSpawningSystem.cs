using Components;
using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(StationSpawningSystem))]
public partial struct PassengerSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

        state.RequireForUpdate<Config>();
        state.RequireForUpdate<StationConfig>();
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;
        // Passenger spawn
        var config = SystemAPI.GetSingleton<Config>();

        var stationConfig = SystemAPI.GetSingleton<StationConfig>();

        var passengers = CollectionHelper.CreateNativeArray<Entity>(config.NumPassengersPerPlatform * stationConfig.NumStations * 2 * stationConfig.NumLines, Allocator.Temp);
        em.Instantiate(config.PassengerEntity, passengers);

        int passengersPerQueue = config.NumPassengersPerPlatform / stationConfig.NumQueingPointsPerPlatform;

        int queueId = 0;
        foreach (var (transform, queueComp, queueEntity) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<QueueComponent>>()
                     .WithEntityAccess())
        {
            var queuePassengersBuffer = state.EntityManager.GetBuffer<QueuePassengers>(queueEntity);
            queueComp.ValueRW.StartIndex = 0;
            
            for (int j = 0; j < passengersPerQueue; j++)
            {
                LocalTransform lc = new LocalTransform();
                lc = transform.ValueRO;
                lc.Position -= transform.ValueRO.Forward() * config.DistanceBetweenPassengers * j;
                
                queuePassengersBuffer.ElementAt(queueComp.ValueRW.StartIndex + queueComp.ValueRW.QueueLength).Passenger = passengers[queueId * passengersPerQueue + j];
                queueComp.ValueRW.QueueLength++;

                var passenger = passengers[queueId * passengersPerQueue + j];
                em.SetComponentData<LocalTransform>(passenger, lc);
                em.SetComponentData<PassengerTravel>(passenger, new PassengerTravel { Station = queueComp.ValueRW.Station });
            }
            queueId++;
        }

        state.Enabled = false;
    }
}
