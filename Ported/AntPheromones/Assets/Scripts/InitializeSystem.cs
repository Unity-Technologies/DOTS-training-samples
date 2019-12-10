using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(ConvertToEntitySystem))]
public class InitializeSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
    }

	// TODO: move all of this
	static int instancesPerBatch = 1023;
	NativeArray<Obstacle> Obstacles;
	NativeArray<int2> ObstacleBuckets;
	bool created = false;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();

        AntSettings settings = GetSingleton<AntSettings>();

		if (!created)
		{
			GenerateObstacles(ref settings);
			created = true;
		}
        return new JobHandle();
    }

	void GenerateObstacles(ref AntSettings settings)
	{
		List<Obstacle> output = new List<Obstacle>();
		for (int i = 1; i <= settings.obstacleRingCount; i++)
		{
			float ringRadius = (i / (settings.obstacleRingCount + 1f)) * (settings.mapSize * .5f);
			float circumference = ringRadius * 2f * Mathf.PI;
			int maxCount = Mathf.CeilToInt(circumference / (2f * settings.obstacleRadius) * 2f);
			int offset = Random.Range(0, maxCount);
			int holeCount = Random.Range(1, 3);
			for (int j = 0; j < maxCount; j++)
			{
				float t = (float)j / maxCount;
				if ((t * holeCount) % 1f < settings.obstaclesPerRing)
				{
					float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
					Obstacle obstacle = new Obstacle();
					obstacle.position = new Vector2(settings.mapSize * .5f + Mathf.Cos(angle) * ringRadius, settings.mapSize * .5f + Mathf.Sin(angle) * ringRadius);
					obstacle.radius = settings.obstacleRadius;
					output.Add(obstacle);
				}
			}
		}
		List<Obstacle>[,] tempObstacleBuckets = new List<Obstacle>[settings.bucketResolution, settings.bucketResolution];

		for (int x = 0; x < settings.bucketResolution; x++)
		{
			for (int y = 0; y < settings.bucketResolution; y++)
			{
				tempObstacleBuckets[x, y] = new List<Obstacle>();
			}
		}

		var res = settings.bucketResolution;
		ObstacleBuckets = new NativeArray<int2>(res * res, Allocator.Persistent);

		int obstacleCount = 0;
		foreach (var obstacle in output)
		{
			Vector2 pos = obstacle.position;
			float radius = obstacle.radius;

			var startX = math.clamp((int)((pos.x - radius) / settings.mapSize * res), 0, res - 1);
			var endX = math.clamp((int)((pos.x + radius) / settings.mapSize * res), 0, res - 1);

			var startY = math.clamp((int)((pos.y - radius) / settings.mapSize * res), 0, res - 1);
			var endY = math.clamp((int)((pos.y + radius) / settings.mapSize * res), 0, res - 1);

			for (int x = startX; x <= endX; x++)
			{
				for (int y = startY; y <= endY; y++)
				{
					tempObstacleBuckets[x, y].Add(obstacle);
					obstacleCount++;
				}
			}
		}

		// sort obstacles and fill buckets
		Obstacles = new NativeArray<Obstacle>(obstacleCount, Allocator.Persistent);
		int2 range = new int2(0, 0);

		for (int x = 0; x < res; x++)
		{
			for (int y = 0; y < res; y++)
			{
				int index = x + y * res;
				foreach (var o in tempObstacleBuckets[x,y])
				{
					Obstacles[range.x + range.y] = o;
					range.y++;
				}
				ObstacleBuckets[index] = range;

				range.x += range.y;
				range.y = 0;
			}
		}

	}

}
