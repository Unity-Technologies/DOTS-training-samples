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

        Entities.ForEach((ref Entity spawnerEntity, ref AntSpawner spawner) =>
        {
            for (int i = 0; i < spawner.NbAnts; ++i)
            {
                var ant = cmd.Instantiate(spawner.AntPrefab);
                var direction = Vector2.Angle(Vector2.right, UnityRand.insideUnitCircle);
                float3 position = spawner.Origin + UnityRand.insideUnitSphere;

                cmd.SetComponent(ant, new Translation { Value = position });
            }

            CreateColony(cmd, spawner.ColonyPrefab, spawner.ColonyPosition);
            CreateFood(cmd, spawner.FoodPrefab, spawner.FoodPosition);

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
}
