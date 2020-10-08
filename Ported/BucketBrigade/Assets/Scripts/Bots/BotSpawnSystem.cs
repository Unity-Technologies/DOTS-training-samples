using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BotSpawnSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer.ParallelWriter ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Random rand = new Random((uint)System.DateTime.UtcNow.Millisecond);

        Entities
            .WithName("SpawnBots")
            .ForEach((Entity spawnerEntity, int entityInQueryIndex,
                in BotSpawner spawner) =>
            {
                int count = 0;
                for (int i = 0; i < spawner.numberBots - 2; i++)
                {
                    CreateBot(ecb, spawner, ref rand, count++, Role.PassFull);
                }
                CreateBot(ecb, spawner, ref rand, count++, Role.Scooper);
                CreateBot(ecb, spawner, ref rand, count++, Role.Thrower);

                ecb.DestroyEntity(spawner.numberBots, spawnerEntity);
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }

    static void CreateBot(in EntityCommandBuffer.ParallelWriter ecb, in BotSpawner spawner, ref Random rand, in int index, Role role)
    {
        Entity e = ecb.Instantiate(index, spawner.botPrefab);

        float2 pos = rand.NextFloat2() * spawner.spawnRadius - new float2(spawner.spawnRadius * 0.5f) + spawner.spawnCenter;
        ecb.AddComponent(index, e, new Pos { Value = pos });
        ecb.AddComponent(index, e, new BotRole { Value = role });    // Boris, you may want to use the Role for spawning later but we can remove it if it's of no use.
        ecb.AddBuffer<CommandBufferElement>(index, e); // prepare command buffer for bots

        // This bot is being created with a Pos and it never rotates. Could we do this in the Prefab though?
        ecb.RemoveComponent<Translation>(index, e);
        ecb.RemoveComponent<Rotation>(index, e);
    }
}