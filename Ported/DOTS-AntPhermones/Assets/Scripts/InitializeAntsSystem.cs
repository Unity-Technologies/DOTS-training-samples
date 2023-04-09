using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Transforms;
using Unity.Collections;


[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(ConvertToEntitySystem))]
public class InitializeAntsSystem : SystemBase
{
	EntityQuery initializeAntsQuery;
	protected override void OnCreate()
	{
		initializeAntsQuery = GetEntityQuery(ComponentType.ReadOnly<InitializeAnts>(), ComponentType.ReadOnly<AntParams>());
	}
	protected override void OnUpdate()
	{
		var entityManager = EntityManager;
		Entities
			.WithStructuralChanges()
			.ForEach((Entity entity, in AntParams antParams, in InitializeAnts initializeAnts) =>
			{
				{
					var obstacleRingCount = initializeAnts.ObstacleRingCount;
					var mapSize = antParams.MapSize;
					var obstacleRadius = antParams.ObstacleRadius;
					var random = new Random(initializeAnts.RandomSeed);
					var obstaclePrefabEntity = initializeAnts.ObstaclePrefab;
					var obstaclesPerRing = initializeAnts.ObstaclesPerRing;
					for (int i = 1; i <= obstacleRingCount; i++)
					{
						float ringRadius = (i / (obstacleRingCount + 1f)) * (mapSize * .5f);
						float circumference = ringRadius * 2f * math.PI;
						int maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius) * 2f);
						int offset = random.NextInt(0, maxCount);
						int holeCount = random.NextInt(1, 3);
						for (int j = 0; j < maxCount; j++)
						{
							float t = (float)j / maxCount;
							if ((t * holeCount) % 1f < obstaclesPerRing)
							{
								float angle = (j + offset) / (float)maxCount * (2f * math.PI);
								var obstacle = entityManager.Instantiate(obstaclePrefabEntity);
								entityManager.SetComponentData(obstacle, new Translation() { Value = new float3(mapSize * .5f + math.cos(angle) * ringRadius, mapSize * .5f + math.sin(angle) * ringRadius, 0) });
									//linkedEntities.Add(obstacle);
								}
						}
					}
					var colony = entityManager.Instantiate(initializeAnts.ColonyPrefab);
					entityManager.SetComponentData(colony, new Translation() { Value = new float3(new float2(mapSize * 0.5f), 0) });
						//linkedEntities.Add(colony);
						float resourceAngle = random.NextFloat(2f * math.PI);
					var resource = entityManager.Instantiate(initializeAnts.ResourcePrefab);
					entityManager.SetComponentData(resource, new Translation() { Value = new float3(new float2(mapSize * .5f) + new float2(math.cos(resourceAngle) * mapSize * .475f, math.sin(resourceAngle) * mapSize * .475f), 0) });
						//linkedEntities.Add(resource);
						var ants = entityManager.Instantiate(initializeAnts.AntPrefab, initializeAnts.AntCount, Allocator.Temp);
					for (int i = 0; i < ants.Length; i++)
					{
						var ant = ants[i];
						entityManager.SetComponentData(ant, new Translation() { Value = new float3(random.NextFloat2(-5f, 5f) + mapSize * 0.5f, 0) });
						entityManager.SetComponentData(ant, new AntData(ref random));
							//linkedEntities.Add(ant);
						}
					ants.Dispose();


				}
			}).Run();
		entityManager.RemoveComponent<InitializeAnts>(initializeAntsQuery);
	}


}


