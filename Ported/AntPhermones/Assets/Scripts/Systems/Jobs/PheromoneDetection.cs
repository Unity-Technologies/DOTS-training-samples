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
    public DynamicBuffer<Pheromone> Pheromones;

    public void Execute(ref Ant ant, in Position position, in Direction direction)
    {
	    var output = 0f;
	    var directionRadians = math.radians(direction.direction);

		for (var i=-1;i<=1;i+=2)
		{
			var angle = directionRadians + i * math.PI / 4;
			var testX = position.position.x + math.cos(angle) * distance;
			var testY = position.position.y + math.sin(angle) * distance;

			if (testX >= 0 && testY >= 0 && testX < mapSize && testY < mapSize)
			{
				var index = (int)testX + (int)testY * mapSize;
				index *= 2;
				if (ant.hasResource)
					index++;
				
				var value = 0f;
				value = Pheromones[index].strength;
				output += value * i;
			}
		}
		

		ant.pheroSteering = math.sign(output) * steeringStrength;
    }
}
