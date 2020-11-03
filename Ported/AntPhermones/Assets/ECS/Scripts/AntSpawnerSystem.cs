using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityRand = UnityEngine.Random;
using DOTSRand = Unity.Mathematics.Random;

public struct Config : IComponentData
{
	public float ObstacleRadius;
	public float ObstaclesPerRing;
	public int ObstacleRingCount;
	public float MapSize;
}

public class AntSpawnerSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem cmdBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        cmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var cmd = cmdBufferSystem.CreateCommandBuffer();

        var config = new Config
        {
            ObstacleRadius = 0.1f,
            ObstaclesPerRing = 0.8f,
            ObstacleRingCount = 3,
            MapSize = 8f,
        };

        Entities.ForEach((ref Entity spawnerEntity, ref AntSpawner spawner) =>
        {
            var rand = Unity.Mathematics.Random.CreateFromIndex(1);
            for (uint i = 0; i < spawner.NbAnts; ++i)
            {
                var ant = cmd.Instantiate(spawner.AntPrefab);
                var direction = Vector2.Angle(Vector2.right, UnityRand.insideUnitCircle);
                float3 position = spawner.Origin + UnityRand.insideUnitSphere;

                cmd.SetComponent(ant, new Translation { Value = position });

                cmd.AddComponent<Direction>(ant);
                cmd.SetComponent(ant, new Direction
                {
                    Value = (float)rand.NextFloat(0.0f, 1.0f) * 2.0f * Mathf.PI,
                });

                cmd.AddComponent<Speed>(ant);
                cmd.SetComponent(ant, new Speed
                {
                    Value = 0.2f + (float)rand.NextFloat(-0.05f, 0.05f)
                });

                cmd.AddComponent<RandState>(ant);
                cmd.SetComponent(ant, new RandState
                {
                    Random = Unity.Mathematics.Random.CreateFromIndex(i + 1),
                });
            }
            CreateColony(cmd, spawner.ColonyPrefab, spawner.ColonyPosition);
            CreateFood(cmd, spawner.FoodPrefab, spawner.FoodPosition);
            CreateObstacles(cmd, spawner.ObstaclePrefab, config);

            cmd.DestroyEntity(spawnerEntity);
        })
        .Run();
    }

    static Entity CreateColony(EntityCommandBuffer cmd, Entity prefab, Vector3 position)
    {
        var entity = cmd.Instantiate(prefab);

        cmd.SetComponent(entity, new Translation { Value = position });
        cmd.AddComponent<ColonyTag>(entity);

        return entity;
    }

    static Entity CreateFood(EntityCommandBuffer cmd, Entity prefab, Vector3 position)
    {
        var entity = cmd.Instantiate(prefab);

        cmd.SetComponent(entity, new Translation { Value = position });
        cmd.AddComponent<FoodTag>(entity);

        return entity;
    }

	static void CreateObstacles(EntityCommandBuffer cmd, in Entity prefab, in Config config)
	{
		var rand = new DOTSRand(7);

		for (int i = 1; i <= config.ObstacleRingCount; i++)
		{
			float ringRadius = (i / (config.ObstacleRingCount + 1f)) * (config.MapSize * .5f);
			float circumference = ringRadius * 2f * math.PI;
			int maxCount = Mathf.CeilToInt(circumference / (2f * config.ObstacleRadius) * 2f);
            int offset = rand.NextInt(0, maxCount);
			int holeCount = rand.NextInt(1, 3);

			for (int j = 0; j < maxCount; j++)
			{
				float t = j / (float)maxCount;
				if ((t * holeCount) % 1f < config.ObstaclesPerRing)
				{
                    float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                    float x = math.cos(angle) * ringRadius;
                    float z = math.sin(angle) * ringRadius;

                    var position2D = new float2(x, z);
                    var position3D = new float3(x, 0, z);
                    float radius = config.ObstacleRadius;

                    var obstacle = new Radius 
                    { 
                        Value = radius 
                    };

                    var entity = cmd.Instantiate(prefab);
                    cmd.SetComponent(entity, new Translation { Value = position3D });
                    cmd.AddComponent(entity, obstacle);
                    cmd.AddComponent<ObstacleAvoid>(entity);
				}
			}
		}
	}
}
