using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile, RequireMatchingQueriesForUpdate]
partial struct PlatformSpawnerSystem : ISystem
{
    EntityQuery _baseColorQuery;
    EntityQuery _platformIdQuery;
    EntityQuery _stationIdQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _baseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        _platformIdQuery = state.GetEntityQuery(ComponentType.ReadOnly<PlatformId>());
        _stationIdQuery = state.GetEntityQuery(ComponentType.ReadOnly<StationId>());
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
        
        var baseColorQueryMask = _baseColorQuery.GetEntityQueryMask();
        var stationIdQueryMask = _stationIdQuery.GetEntityQueryMask();
        var platformIdQueryMask = _platformIdQuery.GetEntityQueryMask();
        int platformId = 0;
        
        foreach (var (metroLine, entity) in SystemAPI.Query<MetroLine>().WithEntityAccess())
        {
            var metroLineColor = new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)metroLine.Color };
            var platforms = new NativeArray<int>(metroLine.RailwayPositions.Length, Allocator.Persistent);
            for (int i = 0, count = metroLine.RailwayPositions.Length; i < count; i++)
            {
                if(metroLine.RailwayTypes[i] != RailwayPointType.Platform)
                    continue;
                var platform = ecb.Instantiate(platformConfig.PlatformPrefab);
                ecb.SetComponent(platform, LocalTransform.FromPositionRotation(metroLine.RailwayPositions[i], metroLine.RailwayRotations[i]));
                ecb.SetComponentForLinkedEntityGroup(platform, baseColorQueryMask, metroLineColor);
                ecb.SetComponentForLinkedEntityGroup(platform, platformIdQueryMask, new PlatformId{ Value = platformId });
                ecb.SetComponentForLinkedEntityGroup(platform, stationIdQueryMask, new StationId{ Value = metroLine.StationIds[i] });
                platforms[i] = platformId;
                platformId++;
            }
            
            //Sorry I have been lazy to add ids... Let's do it tomorrow.
            var newMetroLine = new MetroLine
            {
                Color = metroLine.Color,
                Platforms = platforms,
                RailwayPositions = metroLine.RailwayPositions,
                RailwayTypes = metroLine.RailwayTypes,
                RailwayRotations = metroLine.RailwayRotations,
                StationIds = metroLine.StationIds
            };
            ecb.SetComponent(entity, newMetroLine);
        }
        
        
        state.Enabled = false;
    }
}
