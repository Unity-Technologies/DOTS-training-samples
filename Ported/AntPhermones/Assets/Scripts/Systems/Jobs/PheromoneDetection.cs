using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct PheromoneDetectionJob : IJobEntity
{
    public int mapSize;
    public float steeringStrength;
    public float distance;
    [ReadOnly]
    public DynamicBuffer<LookingForFoodPheromone> lookingForFoodPheromones;
    [ReadOnly]
    public DynamicBuffer<LookingForHomePheromone> lookingForHomePheromones;

    public void Execute(ref Ant ant, in Position position, in Direction direction)
    {
	    var output = 0f;
	    var directionRadians = direction.direction / 180f * math.PI;

		for (var i=-1;i<=1;i+=2)
		{
			var angle = directionRadians + i * math.PI * 0.25f;
			var testX = position.position.x + math.cos(angle) * distance;
			var testY = position.position.y + math.sin(angle) * distance;

			if (testX >= 0 && testY >= 0 && testX < mapSize && testY < mapSize)
			{
				var index = (int)testX + (int)testY * mapSize;

				var value = 0f;
				if (ant.hasResource)
					value = lookingForHomePheromones[index].strength;
				else				
					value = lookingForFoodPheromones[index].strength;
				
				output += value * i;
			}
		}

		ant.pheroSteering = math.sign(output) * steeringStrength;
    }
}
