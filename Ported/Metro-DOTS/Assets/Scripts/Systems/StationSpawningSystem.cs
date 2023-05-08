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
        var stationComponent = SystemAPI.GetSingleton<StationConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var stations = CollectionHelper.CreateNativeArray<Entity>(stationComponent.NumStations, Allocator.Temp);
        ecb.Instantiate(stationComponent.StationEntity, stations);

        int i = 0;
        foreach (var transform in
            SystemAPI.Query<RefRW<LocalTransform>>()
            .WithAll<Components.StationIDComponent>())
        {
            transform.ValueRW.Position = new float3(i * stationComponent.Spacing, 0, 0);
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
