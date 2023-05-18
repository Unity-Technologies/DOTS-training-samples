using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public struct PheromoneDecayJob : IJobParallelFor
{
    public float pheromoneDecayRate;
    [NativeDisableParallelForRestriction]
    public DynamicBuffer<LookingForFoodPheromone> lookingForFoodPheromones;
    [NativeDisableParallelForRestriction]
    public DynamicBuffer<LookingForHomePheromone> lookingForHomePheromones;

    public void Execute(int index)
    {
        var lookingForFoodPheromone = lookingForFoodPheromones[index];
        lookingForFoodPheromone.strength *= pheromoneDecayRate;
        lookingForFoodPheromones[index] = lookingForFoodPheromone;

        var lookingForHomePheromone = lookingForHomePheromones[index];
        lookingForHomePheromone.strength *= pheromoneDecayRate;
        lookingForHomePheromones[index] = lookingForHomePheromone;
    }
}
