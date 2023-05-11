using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

        var passengers = CollectionHelper.CreateNativeArray<Entity>(config.NumPassengersPerStation * stationConfig.NumStations * stationConfig.NumLines, Allocator.Temp);
        em.Instantiate(config.PassengerEntity, passengers);

        int passengersPerQueue = config.NumPassengersPerStation / stationConfig.NumQueingPoints;
        float distanceBetweenPassenger = 1;

        int queueId = 0;
        foreach (var (transform, queueEntity) in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<QueueComponent>()
                     .WithEntityAccess())
        {
            var queuePassengersBuffer = state.EntityManager.GetBuffer<QueuePassengers>(queueEntity);
            var queueComp = state.EntityManager.GetComponentData<QueueComponent>(queueEntity);
            queueComp.StartIndex = 0;
            
            for (int j = 0; j < passengersPerQueue; j++)
            {
                LocalTransform lc = new LocalTransform();
                lc = transform.ValueRO;
                lc.Position += new float3(0, 0, distanceBetweenPassenger * j);
                
                queuePassengersBuffer.ElementAt(queueComp.StartIndex + queueComp.QueueLength).Passenger = passengers[queueId * passengersPerQueue + j];
                queueComp.QueueLength++;

                em.SetComponentData<LocalTransform>(passengers[queueId * passengersPerQueue + j], lc);
            }
            em.SetComponentData<QueueComponent>(queueEntity, queueComp);
            queueId++;
        }

        state.Enabled = false;

        // i = 0;
        // foreach (var transform in
        //             SystemAPI.Query<RefRO<LocalTransform>>()
        //             .WithAll<StationIDComponent>())
        // {
        //     ecb.SetComponent<LocalTransform>(passengers[i], transform.ValueRO);
        //     i++;
        // }

    }
}
