using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct FireFighterLineJob : IJobEntity
{
    // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
    public EntityCommandBuffer.ParallelWriter ECB;
    public HeatMap heatMap;
    public int gridSize;

    public float fireThreshold;


    void Execute(ref FireFighterLineAspect fireFighterLineJob)
    {
        var closestPoint = new float2(999999, 999999);
        int index = 0;
        foreach (var heatValue in heatMap.HeatValues)
        {
            if (heatValue >= fireThreshold)
            {
                var newPoint = new float2(index % 3, index / 3);
                if (math.distance(fireFighterLineJob.StartPosition, closestPoint) >
                    math.distance(fireFighterLineJob.StartPosition, newPoint))
                {
                    closestPoint = newPoint;
                }
            }
        }

        fireFighterLineJob.EndPosition = closestPoint;

    }
}

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
        
        var fireFighterLineJobJob = new FireFighterLineJob
        {
            // Note the function call required to get a parallel writer for an EntityCommandBuffer.
            ECB = ecb.AsParallelWriter(),
            gridSize = config.GridSize,
            heatMap = heatmap,
            fireThreshold = config.FireThreshold
        };
        fireFighterLineJobJob.ScheduleParallel();
    }
}