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
        var cmd = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((ref Entity spawnerEntity, ref AntSpawner spawner) =>
        {
            for (int i = 0; i < spawner.NbAnts; ++i)
            {
                var ant = cmd.Instantiate(spawner.AntPrefab);
                var direction = Vector2.Angle(Vector2.right, UnityRand.insideUnitCircle);
                float3 position = spawner.Origin + UnityRand.insideUnitSphere;

                cmd.SetComponent(ant, new Translation { Value = position });
            }

            cmd.DestroyEntity(spawnerEntity);
        })
        .Run();

        cmd.Playback(EntityManager);
    }
}
