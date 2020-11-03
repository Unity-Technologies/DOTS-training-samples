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
using UnityEditor.Rendering;

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

        var config = GetSingleton<AntSpawner>();

        Entities
        .WithAll<AntSpawnerUnused>()
        .ForEach((ref Entity spawnerEntity, ref AntSpawner spawner) =>
        {
            CreateAnts(cmd, spawner);
            CreateColony(cmd, spawner.ColonyPrefab, spawner.ColonyPosition);
            CreateFood(cmd, spawner.FoodPrefab, spawner.FoodPosition);
            CreateObstacles(cmd, spawner.ObstaclePrefab, config);

            cmd.RemoveComponent<AntSpawnerUnused>(spawnerEntity);
        })
        .Run();
    }

    static void CreateAnts(EntityCommandBuffer cmd, in AntSpawner spawner)
    {
        var rand = Unity.Mathematics.Random.CreateFromIndex(1);
        for (uint i = 0; i < spawner.NbAnts; ++i)
        {
            var ant = cmd.Instantiate(spawner.AntPrefab);
            var direction = (float)rand.NextFloat(0.0f, 1.0f) * 2.0f * Mathf.PI;
            var speed = 0.2f + (float)rand.NextFloat(-0.05f, 0.05f);

            float3 position = spawner.Origin + UnityRand.insideUnitSphere * 0.5f;
            position.y = 0;

            cmd.SetComponent(ant, new Translation
            {
                Value = position
            });

            cmd.AddComponent(ant, new Direction
            {
                Value = direction
            });

            cmd.AddComponent(ant, new Speed
            {
                Value = speed
            });

            cmd.AddComponent(ant, new RandState
            {
                Random = Unity.Mathematics.Random.CreateFromIndex(i + 1),
            });


            cmd.AddComponent(ant, new ObstacleAvoid());
        }
    }

    static void CreateColony(EntityCommandBuffer cmd, Entity prefab, Vector3 position)
    {
        var entity = cmd.Instantiate(prefab);

        cmd.SetComponent(entity, new Translation { Value = position });
        cmd.AddComponent<ColonyTag>(entity);
    }

    static void CreateFood(EntityCommandBuffer cmd, Entity prefab, Vector3 position)
    {
        var entity = cmd.Instantiate(prefab);

        cmd.SetComponent(entity, new Translation { Value = position });
        cmd.AddComponent<FoodTag>(entity);
    }

    static void CreateObstacles(EntityCommandBuffer cmd, in Entity prefab, in AntSpawner spawner)
    {
        var rand = new DOTSRand(7);

        for (int i = 1; i <= spawner.ObstacleRingCount; i++)
        {
            float ringRadius = (i / (spawner.ObstacleRingCount + 1f)) * (spawner.MapSize * .5f);
            float circumference = ringRadius * 2f * math.PI;
            int maxCount = Mathf.CeilToInt(circumference / spawner.ObstacleRadius);
            int offset = rand.NextInt(0, maxCount);
            int holeCount = rand.NextInt(1, 3);

            for (int j = 0; j < maxCount; j++)
            {
                float t = j / (float)maxCount;
                if ((t * holeCount) % 1f < spawner.ObstaclesPerRing)
                {
                    float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                    float x = math.cos(angle) * ringRadius;
                    float z = math.sin(angle) * ringRadius;

                    var position3D = new float3(x, 0, z);
                    float radius = spawner.ObstacleRadius;

                    var obstRadius = new Radius 
                    { 
                        Value = radius 
                    };

                    var entity = cmd.Instantiate(prefab);
                    cmd.SetComponent(entity, new Translation { Value = position3D });
                    cmd.AddComponent(entity, obstRadius);
				}
			}
		}
	}
}
