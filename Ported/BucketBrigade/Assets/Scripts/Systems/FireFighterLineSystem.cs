using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct FireFighterLineSystem : ISystem
{
    private HeatMap heatMap;

    private int GridSize;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // See note above regarding the [BurstCompile] attribute.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var config = SystemAPI.GetSingleton<Config>();
        var heatmap = SystemAPI.GetSingleton<HeatMap>();
        
        // query all tyupes fo
        foreach (var ffline in SystemAPI.Query<RefRW<FireFighterLine>>())
        {
            var closestPoint = new float2(999999, 999999);
            int index = 0;
            foreach (var heatValue in heatMap.HeatValues)
            {
                if (heatValue >= config.FireThreshold)
                {
                    var newPoint = new float2(index % config.GridSize, index / config.GridSize);
                    if (math.distance(ffline.ValueRO.StartPosition, closestPoint) >
                        math.distance(ffline.ValueRO.StartPosition, newPoint))
                    {
                        closestPoint = newPoint;
                    }
                }
                index++;
            }

            ffline.ValueRW.EndPosition = closestPoint;
        }
    }
}