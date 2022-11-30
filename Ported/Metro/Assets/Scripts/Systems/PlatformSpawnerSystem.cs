using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile, RequireMatchingQueriesForUpdate]
partial struct PlatformSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var metroLine in SystemAPI.Query<MetroLine>())
        {
            var config = SystemAPI.GetSingleton<PlatformConfig>();
            var counter = metroLine.RailwayPositions.Length;
            for (int i = 0; i < counter; i++)
            {
                if(metroLine.RailwayType[i] != RailwayPointType.Platform)
                    continue;
                var instance = ecb.Instantiate(config.PlatformPrefab);
                ecb.SetComponent(instance, LocalTransform.FromPositionRotation(metroLine.RailwayPositions[i], metroLine.RailwayRotations[i]));
            }
        }
        
        state.Enabled = false;
    }
}
