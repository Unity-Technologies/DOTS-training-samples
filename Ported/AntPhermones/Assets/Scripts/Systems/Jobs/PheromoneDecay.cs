using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public struct PheromoneDecayJob : IJobParallelFor
{
    public float pheromoneDecayRate;
    [NativeDisableParallelForRestriction]
    public DynamicBuffer<Pheromone> Pheromones;

    public void Execute(int index)
    {
        var pheromone = Pheromones[index];
        pheromone.strength *= pheromoneDecayRate;
        Pheromones[index] = pheromone;
    }
}
