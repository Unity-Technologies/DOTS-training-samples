using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using Random = UnityEngine.Random;

struct ObstacleBucket : IComponentData
{
	public int2 range;
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(ConvertToEntitySystem))]
public class InitializeSystem : JobComponentSystem
{
    bool init;

    protected override void OnCreate()
    {
        init = false; 
    }

	// TODO: move all of this
	static int instancesPerBatch = 1023;
	NativeArray<int2> ObstacleBuckets;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (init)
            return inputDeps;

        inputDeps.Complete();

        AntSettings settings = GetSingleton<AntSettings>();
        SpawnAnts(ref settings);
        GenerateObstacles(ref settings);
 
        init = true;

        return new JobHandle();
    }
    
    protected void SpawnAnts(ref AntSettings settings)
    {
        Translation colonyPosition = new Translation();
        colonyPosition.Value = new Vector3(.5f, .5f, 0);

        NonUniformScale antSize = new NonUniformScale();
        antSize.Value = settings.antSize;

        const float perimeter = 0.03f;

        for (int i = 0; i < 10; ++i)
        {
            Translation antPosition = new Translation();
            antPosition.Value = new Vector3(Random.Range(-perimeter, perimeter) + colonyPosition.Value.x, Random.Range(-perimeter, perimeter) + colonyPosition.Value.y, 0);

            Rotation antRotation = new Rotation();
            antRotation.Value = Quaternion.Euler(0f, 0f, Random.Range(0.0f, 360.0f));

            Entity ant = EntityManager.Instantiate(settings.antPrefab);
            EntityManager.SetComponentData(ant, antPosition);
            EntityManager.AddComponentData(ant, antSize);
            EntityManager.SetComponentData(ant, antRotation);
        }
    }

	void GenerateObstacles(ref AntSettings settings)
	{
        NonUniformScale prefabScale = new NonUniformScale();
        prefabScale.Value = Vector3.one * 2 * settings.obstacleRadius/(float)settings.mapSize;

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
                    float center = settings.mapSize * .5f;
                    float x = Mathf.Cos(angle) * ringRadius + center;
                    float y = Mathf.Sin(angle) * ringRadius + center;
					obstacle.position = new Vector2(x, y);
					obstacle.radius = settings.obstacleRadius;
					output.Add(obstacle);

                    var prefabEntity = EntityManager.Instantiate(settings.obstaclePrefab);
                    Translation prefabTranslation = new Translation();
                    
                    prefabTranslation.Value = new Vector3(x + .5f, y + .5f, 0) / (float)settings.mapSize;
                    EntityManager.SetComponentData(prefabEntity, prefabTranslation);
                    EntityManager.AddComponentData(prefabEntity, prefabScale);
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

		var obstacleArchetype = new ComponentType[] { typeof(Obstacle) };

		// sort obstacles and fill buckets
		int2 range = new int2(0, 0);

		for (int x = 0; x < res; x++)
		{
			for (int y = 0; y < res; y++)
			{
				int index = x + y * res;
				foreach (var o in tempObstacleBuckets[x,y])
				{
					var obstacle = o;
					obstacle.bucketIndex = index;
					
                    var entity = EntityManager.CreateEntity(obstacleArchetype);
					EntityManager.SetComponentData(entity, obstacle);
                    
                    range.y++;
				}
				ObstacleBuckets[index] = range;

				range.x += range.y;
				range.y = 0;
			}
		}

	}

}
