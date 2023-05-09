using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct StationSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<StationConfig>();
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        var stationConfig = SystemAPI.GetSingleton<StationConfig>();
        var stations = CollectionHelper.CreateNativeArray<Entity>(stationConfig.NumStations, Allocator.Temp);

        // query will not work as entities are not created right away with command buffer
        // var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        // var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        // ecb.Instantiate(stationConfig.StationEntity, stations);

        var em = state.EntityManager;
        em.Instantiate(stationConfig.StationEntity, stations);

        var i = 0;
        foreach (var transform in
            SystemAPI.Query<RefRW<LocalTransform>>()
            .WithAll<Components.StationIDComponent>())
        {
            transform.ValueRW.Position = new float3(i * stationConfig.Spacing, 0, 0);
            i++;
        }

        /*
        var query = SystemAPI.QueryBuilder().WithAll<StationConfig>().Build();
        var chunks = query.ToArchetypeChunkArray(Allocator.Temp);
        var localTransformHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>();
        foreach (var chunk in chunks)
        {
            var localTransformVals = chunk.GetNativeArray(localTransformHandle);
            for (int j = 0; j < chunk.Count; j++)
            {
                var v = localTransformVals[j];
            }
        }
        */

        state.Enabled = false;
    }
}
