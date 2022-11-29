using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile, RequireMatchingQueriesForUpdate]
partial struct PlatformSpawner : ISystem
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
        var config = SystemAPI.GetSingleton<PlatformConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (railwayPoint, rpTransform) in SystemAPI.Query<RailwayPoint, TransformAspect>().WithAll<RailwayPoint>())
        {
            if (railwayPoint.RailwayPointType != RailwayPointType.Platform)
                continue;
            
            var instance = ecb.Instantiate(config.PlatformPrefab);
            ecb.SetComponent(instance, LocalTransform.FromPositionRotation(rpTransform.WorldPosition, rpTransform.WorldTransform.Rotation));
        }
        
        state.Enabled = false;
    }
}
