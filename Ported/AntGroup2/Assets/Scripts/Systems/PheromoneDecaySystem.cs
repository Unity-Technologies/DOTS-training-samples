using Unity.Burst;
using Unity.Entities;

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
        foreach( var map in SystemAPI.Query<DynamicBuffer<PheromoneMap>>())
        {
            for (int i = 0; i < map.Length; i++)
            {
                ref var cellRef = ref map.ElementAt(i);
                cellRef.amount *= 0.99f /* dt*/; // TODO: Decay config? time from config? Time component?
            }
        }
    }
}
