using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public struct PheromoneDecayJob : IJob
{
    public float pheromoneDecayRate;
    public DynamicBuffer<Pheromone> pheromones;

    public void Execute()
    {
        for (var i = 0; i < pheromones.Length; i++)
        {
            var pheromone = pheromones[i];
            pheromone.strength *= pheromoneDecayRate;
            pheromones[i] = pheromone;
        }
    }
}
