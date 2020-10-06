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
                    ecb.SetComponent(i, e, new Translation { Value = new float3(pos.x, 0f, pos.y) });
                }
                
                ecb.DestroyEntity(spawner.numberBots, spawnerEntity);
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
