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
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            .WithName("SpawnBots")
            .ForEach((Entity spawnerEntity, int entityInQueryIndex,
                in BotSpawner spawner) =>
            {
                Random rand = new Random((uint)System.DateTime.UtcNow.Millisecond);
                for (int i = 0; i < spawner.numberBots; i++)
                {
                    Entity e = ecb.Instantiate(i, spawner.botPrefab);

                    float2 pos = rand.NextFloat2() * spawner.spawnRadius - new float2(spawner.spawnRadius * 0.5f);
                    ecb.AddComponent(i, e, new Pos { Value = pos });
                    ecb.AddComponent(i, e, new BotRole { Value = Role.None });    // Later we will read this from the Bot Spawner and make the right amount of Scooper, Thrower, passFull, passEmpty
                    
                    // This bot is being created with a Pos and it never rotates. Could we do this in the Prefab though?
                    ecb.RemoveComponent<Translation>(i, e);
                    ecb.RemoveComponent<Rotation>(i, e);
                }
                
                ecb.DestroyEntity(spawner.numberBots, spawnerEntity);
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}