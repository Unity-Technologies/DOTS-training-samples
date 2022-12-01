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
        
        float pheromoneDecayAmount = math.max(config.PheromoneDecayRateSec * config.TimeScale * SystemAPI.Time.DeltaTime,0.0001f);

        #if false
            var pheromoneMap = SystemAPI.GetSingletonBuffer<PheromoneMap>();
            for (int i = 0; i < pheromoneMap.Length; i++)
            {
                ref var cellRef = ref pheromoneMap.ElementAt(i);
                cellRef.amount = math.max(cellRef.amount - pheromoneDecayAmount, 0);
            }
        #else
        var job = new PheromoneDecayJob() { pheromoneDecayAmount = pheromoneDecayAmount };
        job.Schedule();
        #endif
    }
}

[BurstCompile]
[WithAll(typeof(PheromoneMap))]
partial struct PheromoneDecayJob : IJobEntity
{
    public float pheromoneDecayAmount;
    
    public void Execute(ref DynamicBuffer<PheromoneMap> pheromoneMap)
    {
        for (int i = 0; i < pheromoneMap.Length; i++)
        {
            ref var cellRef = ref pheromoneMap.ElementAt(i);
            cellRef.amount = math.max(cellRef.amount - pheromoneDecayAmount, 0);
        }
    }

}