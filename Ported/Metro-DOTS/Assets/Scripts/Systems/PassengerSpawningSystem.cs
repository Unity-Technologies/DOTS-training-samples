using Components;
using Metro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(StationSpawningSystem))]
public partial struct PassengerSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();

        state.RequireForUpdate<Config>();
        state.RequireForUpdate<StationConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
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
                em.SetComponentData<PassengerTravel>(passenger, new PassengerTravel
                {
                    LineID = queueComp.ValueRO.LineID,
                    Station = queueComp.ValueRW.Station,
                    OnPlatformA = queueComp.ValueRW.OnPlatformA
                });
            }
            queueId++;
        }

        var random = Random.CreateFromIndex(78571);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var query = SystemAPI.QueryBuilder().WithAll<URPMaterialPropertyBaseColor>().Build();
        var queryMask = query.GetEntityQueryMask();

        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var hue = random.NextFloat();

        // Set the color of the station
        // Helper to create any amount of colors as distinct from each other as possible.
        // The logic behind this approach is detailed at the following address:
        // https://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
        URPMaterialPropertyBaseColor RandomColor()
        {
            // Note: if you are not familiar with this concept, this is a "local function".
            // You can search for that term on the internet for more information.

            // 0.618034005f == 2 / (math.sqrt(5) + 1) == inverse of the golden ratio
            hue = (hue + 0.618034005f) % 1;
            var color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (Vector4)color };
        }

        foreach (var passenger in passengers)
        {
            ecb.SetComponentForLinkedEntityGroup(passenger, queryMask, RandomColor());
        }

        state.Enabled = false;
    }
}
