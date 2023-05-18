using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct PheromoneDropJob : IJobEntity
{
    public float deltaTime;
    public int mapSize;
    public float antTargetSpeed;
    public float pheromoneGrowthRate;
    [NativeDisableParallelForRestriction]
    public NativeArray<LookingForFoodPheromone> lookingForFoodPheromones;
    [NativeDisableParallelForRestriction]
    public NativeArray<LookingForHomePheromone> lookingForHomePheromones;

    public void Execute(in Ant ant, in Position position, in Speed speed)
    {
        var strength = ant.hasResource ? 1.0f : 0.3f;
        strength *= speed.speed / antTargetSpeed;

        var gridPosition = math.int2(math.floor(position.position));
        if (gridPosition.x < 0 || gridPosition.y < 0 || gridPosition.x >= mapSize || gridPosition.y >= mapSize)
        {
            return;
        }

        var index = gridPosition.x + gridPosition.y * mapSize;

        if (ant.hasResource)
        {
            var lookingForFoodPheromone = lookingForFoodPheromones[index];
            lookingForFoodPheromone.strength += pheromoneGrowthRate * strength * (1f - lookingForFoodPheromone.strength) * deltaTime;
            lookingForFoodPheromones[index] = lookingForFoodPheromone;
        }
        else
        {
            var lookingForHomePheromone = lookingForHomePheromones[index];
            lookingForHomePheromone.strength += pheromoneGrowthRate * strength * (1f - lookingForHomePheromone.strength) * deltaTime;
            lookingForHomePheromones[index] = lookingForHomePheromone;
        }
    }
}
