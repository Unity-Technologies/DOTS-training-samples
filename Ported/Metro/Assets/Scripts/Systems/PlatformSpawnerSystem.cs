using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile, RequireMatchingQueriesForUpdate]
partial struct PlatformSpawnerSystem : ISystem
{
    EntityQuery _baseColorQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _baseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var platformConfig = SystemAPI.GetSingleton<PlatformConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var queryMask = _baseColorQuery.GetEntityQueryMask();

        foreach (var metroLine in SystemAPI.Query<MetroLine>())
        {
            var metroLineColor = new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)metroLine.Color };
            
            for (int i = 0, count = metroLine.RailwayPositions.Length; i < count; i++)
            {
                if(metroLine.RailwayType[i] != RailwayPointType.Platform)
                    continue;
                var platform = ecb.Instantiate(platformConfig.PlatformPrefab);
                ecb.SetComponent(platform, LocalTransform.FromPositionRotation(metroLine.RailwayPositions[i], metroLine.RailwayRotations[i]));
                ecb.SetComponentForLinkedEntityGroup(platform, queryMask, metroLineColor);
            }
        }
        
        state.Enabled = false;
    }
}
