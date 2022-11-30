using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile, RequireMatchingQueriesForUpdate]
partial struct StationSpawnerSystem : ISystem
{
    // EntityQuery _baseColorQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // _baseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //var platformConfig = SystemAPI.GetSingleton<PlatformConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        int highestId = 0;
        
        // var queryMask = _baseColorQuery.GetEntityQueryMask();
        foreach (var stationId in SystemAPI.Query<StationId>())
        {
            highestId = math.max(stationId.Value, highestId);
            
        }
        
        state.Enabled = false;
    }
}
