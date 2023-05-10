using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public struct PheromoneDecayJob : IJobParallelFor
{
    public float pheromoneDecayRate;
    [NativeDisableParallelForRestriction]
    public DynamicBuffer<Pheromone> pheromones;

    public void Execute(int index)
    {
        var pheromone = pheromones[index];
        pheromone.strength *= pheromoneDecayRate;
        pheromones[index] = pheromone;
    }
}
