using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
/*
[BurstCompile]
partial struct StationSpawnerSystem : ISystem
{
    //EntityQuery _platformQueueQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // _baseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        state.RequireForUpdate<StationId>();
        //_platformQueueQuery = state.GetEntityQuery(ComponentType.ReadOnly<PlatformQueue>());
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        //var platformConfig = SystemAPI.GetSingleton<PlatformConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        int highestId = 0;
        
        // var queryMask = _baseColorQuery.GetEntityQueryMask();
        foreach (var stationId in SystemAPI.Query<StationId>())
        {
            highestId = math.max(stationId.Value, highestId);
        }

        var stations = CollectionHelper.CreateNativeArray<Station>(highestId, Allocator.Temp);
        for (int i = 0; i < highestId; i++)
        {
            var stationComp = new Station{ Platforms = new NativeArray<Entity>() };
            var stationEntity = ecb.CreateEntity();
            ecb.SetComponent(stationEntity, stationComp);
            stations[i] = stationComp;
        }

        //var platformQueueQueryMask = _platformQueueQuery.ToEntityArray(Allocator.Persistent);;
        foreach (var (platform, stationId, entity) in SystemAPI.Query<Platform, StationId>().WithEntityAccess())
        {
            var stationComp = stations[stationId.Value];
            //state.GetComponentLookup<PlatformQueue>(entity);
            //stationComp.Platforms.Add(platform);
            // ecb.SetComponentObject(_platformQueueQuery, );
            // platform.PlatformQueues = platformQueueQueryMask;
        }
    }
}
*/