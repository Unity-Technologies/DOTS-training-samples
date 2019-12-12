
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Random = UnityEngine.Random;
using Unity.Rendering;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(ConvertToEntitySystem))]
public class InitializeSystem : JobComponentSystem
{
	private NativeArray<int2> m_ObstacleBuckets;
	private NativeArray<RuntimeManager.CachedObstacle> m_CachedObstacles;

    bool init;

    protected override void OnCreate()
    {
        init = false; 
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (init)
            return inputDeps;

        inputDeps.Complete();

        AntSettings settings = GetSingleton<AntSettings>();
        SpawnAnts(ref settings);
        SpawnObstacles(ref settings);
        SpawnTargets(ref settings);
 
        init = true;

        return new JobHandle();
    }

	protected void SpawnAnts(ref AntSettings settings)
	{
		Translation colonyPosition = new Translation();
		colonyPosition.Value = new Vector3(0.5f, 0.5f, 0);

		NonUniformScale antSize = new NonUniformScale();
		antSize.Value = settings.antSize;

		const float perimeter = 0.03f;

		for (int i = 0; i < settings.antCount; ++i)
		{
			Translation antPosition = new Translation();
			antPosition.Value = new Vector3(Random.Range(-perimeter, perimeter) + colonyPosition.Value.x, Random.Range(-perimeter, perimeter) + colonyPosition.Value.y, 0);

			float angleInDegrees = Random.Range(0.0f, 360.0f);
			Rotation antRotation = new Rotation();
			antRotation.Value = Quaternion.Euler(0f, 0f, angleInDegrees);

			AntComponent ant = new AntComponent()
			{
				acceleration = settings.antAccel,
				facingAngle = math.radians(angleInDegrees),
				brightness = Random.Range(.75f, 1.25f),
				index = i,
				state = 0
			};
			MaterialColor color = new MaterialColor()
			{
				Value = (Vector4)settings.searchColor
			};
			AntSteeringComponent antSteering = new AntSteeringComponent();

			Entity newEntity = EntityManager.Instantiate(settings.antPrefab);
			EntityManager.SetComponentData(newEntity, antPosition);
			EntityManager.AddComponentData(newEntity, antSize);
			EntityManager.SetComponentData(newEntity, antRotation);
			EntityManager.SetComponentData(newEntity, ant);
			EntityManager.AddComponentData(newEntity, color);
			EntityManager.AddComponentData(newEntity, antSteering);
		}	
	}
	
	protected void SpawnObstacles(ref AntSettings settings)
	{
		float radius = settings.obstacleRadius / settings.mapSize;
		NonUniformScale prefabScale = new NonUniformScale() {Value = new float3(radius, radius, radius)};

		List<RuntimeManager.CachedObstacle> cachedObstacles = new List<RuntimeManager.CachedObstacle>();
		
		for (int i = 1; i <= settings.obstacleRingCount; i++)
		{
			float ringRadius = (i / (settings.obstacleRingCount + 1f)) * (settings.mapSize * 0.5f);
			float circumference = ringRadius * 2f * Mathf.PI;
			int maxCount = Mathf.CeilToInt(circumference / (2f * settings.obstacleRadius) * 4f);
			int offset = Random.Range(0, maxCount);
			int holeCount = Random.Range(1, 3);
			for (int j = 0; j < maxCount; j++)
			{
				float t = (float)j / maxCount;
				if ((t * holeCount) % 1f < settings.obstaclesPerRing)
				{
					float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                    float center = settings.mapSize * 0.5f;
                    float x = Mathf.Cos(angle) * ringRadius + center;
                    float y = Mathf.Sin(angle) * ringRadius + center;
                    ObstacleComponent obstacleComponent = new ObstacleComponent() {radius = radius};
                    Translation obstacleTranslation = new Translation() {Value = new float3(x / settings.mapSize, y / settings.mapSize, 0.0f)};
                    RuntimeManager.CachedObstacle cachedObstacle = new RuntimeManager.CachedObstacle() {position = obstacleTranslation.Value, radius = obstacleComponent.radius,};
                    
                    cachedObstacles.Add(cachedObstacle);

                    var prefabEntity = EntityManager.Instantiate(settings.obstaclePrefab);
                    EntityManager.SetComponentData(prefabEntity, obstacleComponent);
                    EntityManager.SetComponentData(prefabEntity, obstacleTranslation);
                    EntityManager.AddComponentData(prefabEntity, prefabScale);
				}
			}
		}
		List<RuntimeManager.CachedObstacle>[,] tempCachedObstacleBuckets = new List<RuntimeManager.CachedObstacle>[settings.bucketResolution, settings.bucketResolution];

		for (int x = 0; x < settings.bucketResolution; x++)
		{
			for (int y = 0; y < settings.bucketResolution; y++)
			{
				tempCachedObstacleBuckets[x, y] = new List<RuntimeManager.CachedObstacle>();
			}
		}

		int numObstacles = cachedObstacles.Count;
		var res = settings.bucketResolution;
		int numBucketedObstacles = 0;

		m_ObstacleBuckets = new NativeArray<int2>(res * res, Allocator.Persistent);

		for(int i = 0; i < numObstacles; i++)
		{
			RuntimeManager.CachedObstacle cachedObstacle = cachedObstacles[i];
			float3 obstaclePosition = cachedObstacle.position;
			float obstacleRadius = cachedObstacle.radius;

			var startX = math.clamp((int)((obstaclePosition.x - obstacleRadius) * res), 0, res - 1);
			var endX = math.clamp((int)((obstaclePosition.x + obstacleRadius) * res), 0, res - 1);

			var startY = math.clamp((int)((obstaclePosition.y - obstacleRadius) * res), 0, res - 1);
			var endY = math.clamp((int)((obstaclePosition.y + obstacleRadius) * res), 0, res - 1);

			for (int x = startX; x <= endX; x++)
			{
				for (int y = startY; y <= endY; y++)
				{
					tempCachedObstacleBuckets[x, y].Add(cachedObstacle);
					numBucketedObstacles++;
				}
			}
		}

		// sort obstacles and fill buckets
		int2 range = new int2(0, 0);
		List<RuntimeManager.CachedObstacle> bucketedCachedObjects = new List<RuntimeManager.CachedObstacle>();

		for (int x = 0; x < res; x++)
		{
			for (int y = 0; y < res; y++)
			{
				int index = x + y * res;
				foreach (var o in tempCachedObstacleBuckets[x,y])
				{
					bucketedCachedObjects.Add(o);
                    range.y++;
				}
				
				m_ObstacleBuckets[index] = range;

				range.x += range.y;
				range.y = 0;
			}
		}
		
		m_CachedObstacles = new NativeArray<RuntimeManager.CachedObstacle>(bucketedCachedObjects.ToArray(), Allocator.Persistent);

		RuntimeManager.instance.cachedObstacles = m_CachedObstacles;
		RuntimeManager.instance.obstacleBuckets = m_ObstacleBuckets;
		RuntimeManager.instance.obstacleBucketDimensions = new int2(res, res);
	}

	protected void SpawnTargets(ref AntSettings settings)
    {
        NonUniformScale scale = new NonUniformScale();
        scale.Value = Vector3.one * 2 * settings.obstacleRadius / (float)settings.mapSize;

        Translation colonyTranslation = new Translation();
        colonyTranslation.Value = new Vector3(0.5f, 0.5f, 0);
		RuntimeManager.instance.colonyPosition = colonyTranslation.Value;

        var colonyEntity = EntityManager.Instantiate(settings.colonyPrefab);
        EntityManager.SetComponentData(colonyEntity, colonyTranslation);
        EntityManager.AddComponentData(colonyEntity, scale);
        
        float resourceAngle = Random.value * 2f * Mathf.PI;
        Translation resourceTranslation = new Translation();
        resourceTranslation.Value = new Vector3(Mathf.Cos(resourceAngle) * .475f + 0.5f, Mathf.Sin(resourceAngle) * .475f + 0.5f, 0.0f);
		RuntimeManager.instance.resourcePosition = resourceTranslation.Value;

        var resourceEntity = EntityManager.Instantiate(settings.resourcePrefab);
        EntityManager.SetComponentData(resourceEntity, resourceTranslation);
        EntityManager.AddComponentData(resourceEntity, scale);
    }

}
