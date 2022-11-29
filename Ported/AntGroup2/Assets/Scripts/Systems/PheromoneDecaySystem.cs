using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(PheromoneSpawningSystem))]
[BurstCompile]
public partial struct PheromoneDecaySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) { }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        
        float pheromoneDecayAmount = config.PheromoneDecayRateSec * config.TimeScale * SystemAPI.Time.DeltaTime;

        var pheromoneMap = SystemAPI.GetSingletonBuffer<PheromoneMap>();
        for (int i = 0; i < pheromoneMap.Length; i++)
        {
            ref var cellRef = ref pheromoneMap.ElementAt(i);
            cellRef.amount = math.max(cellRef.amount - pheromoneDecayAmount, 0);
        }
    }
}
