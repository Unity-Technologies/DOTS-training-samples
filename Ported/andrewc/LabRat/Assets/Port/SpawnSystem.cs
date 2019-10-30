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
    float m_TimeSinceSpawn;
    const float k_TimeBetweenSpawns = 1.0f;
    int m_NumSpawned;

    EntityQuery m_SpawnerQuery;
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    struct SpawnJob : IJobForEachWithEntity<Spawner_FromEntity>
    {
        public bool IsMouse;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity e, int index, ref Spawner_FromEntity spawner)
        {
            var entity = CommandBuffer.Instantiate(index, IsMouse ? spawner.PrefabMouse : spawner.PrefabCat);
            CommandBuffer.SetComponent(index, entity, new Translation {Value = new float3(0.0f, 0.0f, 0.0f)});
            CommandBuffer.AddComponent(index, entity, new Direction {Value = eDirection.East});
            CommandBuffer.AddComponent(index, entity, new Speed {Value = 1.5f});
            if (IsMouse)
                CommandBuffer.AddComponent(index, entity, new MouseTag());
            else
                CommandBuffer.AddComponent(index, entity, new CatTag());
        }
    }

    protected override void OnCreate()
    {
        m_SpawnerQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Spawner_FromEntity>());
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle deps)
    {
        m_TimeSinceSpawn += Time.deltaTime;
        if (m_TimeSinceSpawn < k_TimeBetweenSpawns)
            return deps;

        bool isMouse = (m_NumSpawned & 1) != 0;
        m_TimeSinceSpawn = 0.0f;
        ++m_NumSpawned;

        var handle = new SpawnJob {IsMouse = isMouse, CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()};
        var ret = handle.Schedule(m_SpawnerQuery, deps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(ret);
        return ret;
    }
}
