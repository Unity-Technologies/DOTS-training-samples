using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Transforms;

[AlwaysUpdateSystem]
public class SpawnSystem : JobComponentSystem
{
    // float m_TimeSinceSpawn;
    // const float k_TimeBetweenSpawns = 1.0f;
    // int m_NumSpawned;

    EntityQuery m_SpawnerQuery;
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    struct SpawnJob : IJobForEachWithEntity<Spawner_FromEntity>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public float DeltaTime;
        public void Execute(Entity e, int index, ref Spawner_FromEntity spawner)
        {
            spawner.CatCounter += DeltaTime;
            spawner.RatCounter += DeltaTime;
            if (spawner.CatCounter > spawner.CatFrequency && spawner.CatSpawned < spawner.CatMaxSpawn)
            {
                Entity cat = spawner.CatPrefab;
                // spwan cat
                var entity = CommandBuffer.Instantiate(index, cat);

                CommandBuffer.SetComponent(index, entity, new Translation { Value = spawner.SpawnPos });
                CommandBuffer.AddComponent(index, entity, new Direction { Value = spawner.CatSpawnDirection });
                CommandBuffer.AddComponent(index, entity, new Speed { Value = 1.5f });
                CommandBuffer.AddComponent(index, entity, new CatTag());
                // update spawner comp
                spawner.CatSpawned++;
                spawner.CatCounter -= spawner.CatFrequency;
            }

            if (spawner.RatCounter > spawner.RatFrequency && spawner.RatSpawned < spawner.RatMaxSpawn)
            {
                Entity rat = spawner.RatPrefab;
                // spwan rat
                var entity = CommandBuffer.Instantiate(index, rat);
                CommandBuffer.SetComponent(index, entity, new Translation { Value = spawner.SpawnPos });
                CommandBuffer.AddComponent(index, entity, new Direction { Value = spawner.RatSpawnDirection });
                CommandBuffer.AddComponent(index, entity, new Speed { Value = 1.5f });
                CommandBuffer.AddComponent(index, entity, new MouseTag());
                // update spawner comp
                spawner.RatSpawned++;
                spawner.RatCounter -= spawner.RatFrequency;
            }

            //CommandBuffer.AddComponent(index, entity, new CatTag());
        }
    }

    protected override void OnCreate()
    {
        m_SpawnerQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Spawner_FromEntity>());
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle deps)
    {
        //m_TimeSinceSpawn += Time.deltaTime;
        //if (m_TimeSinceSpawn < k_TimeBetweenSpawns)
        //    return deps;

        // bool isMouse = (m_NumSpawned & 1) != 0;
        // m_TimeSinceSpawn = 0.0f;
        // ++m_NumSpawned;

        var handle = new SpawnJob { DeltaTime = Time.deltaTime, CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()};
        var ret = handle.Schedule(m_SpawnerQuery, deps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(ret);
        return ret;
    }
}
