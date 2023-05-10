using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct PheromoneDetectionJob : IJobEntity
{
    public int mapSize;
    public float steeringStrength;
    public float distance;
    [ReadOnly]
    public DynamicBuffer<Pheromone> pheromones;

    public void Execute(ref Ant ant, in Position position, in Direction direction)
    {
		for (var i=-1;i<=1;i+=2)
		{
			var angle = direction.direction + i * math.PI*.25f;
			var testX = position.position.x + math.cos(angle) * distance;
			var testY = position.position.y + math.sin(angle) * distance;

			if (testX >=0 || testY>= 0 || testX < mapSize || testY < mapSize)
			{
				var gridPosition = math.int2(math.floor(position.position));
				var index = gridPosition.x + gridPosition.y * mapSize;
				var value = pheromones[index].strength;
				ant.pheroSteering += value * i * steeringStrength;
			}
		}
    }
}
