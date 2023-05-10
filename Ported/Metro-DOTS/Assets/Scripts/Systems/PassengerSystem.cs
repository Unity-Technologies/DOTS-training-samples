using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(StationSpawningSystem))]
public partial struct PassengerSystem : ISystem
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
        // Passenger spawn
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var stationConfig = SystemAPI.GetSingleton<StationConfig>();

        var passengers = CollectionHelper.CreateNativeArray<Entity>(config.NumPassengersPerStation * stationConfig.NumStations, Allocator.Temp);
        ecb.Instantiate(config.PassengerEntity, passengers);


        int passengersPerQueue = config.NumPassengersPerStation / stationConfig.NumQueingPoints;
        float distanceBetweenPassenger = 1;
        
        int queueId = 0;
        foreach (var transform in
                 SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<QueueComponent>())
        {
            for (int j = 0; j < passengersPerQueue; j++)
            {
                LocalTransform lc = new LocalTransform();
                lc = transform.ValueRO;
                lc.Position += new float3(0, 0, distanceBetweenPassenger * j);

                ecb.SetComponent<LocalTransform>(passengers[queueId * passengersPerQueue + j], lc);
            }
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
