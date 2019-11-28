using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SpawnParticleSystem : JobComponentSystem
    {
        EntityArchetype m_ParticleArchetype;
        EntityQuery m_SpawnParticleQuery;
        EndInitializationEntityCommandBufferSystem m_EndInitEcbSystem;
        EntityQuery m_UninitializedTagQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_ParticleArchetype = EntityManager.CreateArchetype(
                typeof(PositionComponent),
                typeof(VelocityComponent),
                typeof(PositionInDistanceFieldComponent),
                typeof(LocalToWorldComponent),
                typeof(RenderColorComponent),
                typeof(UninitializedTagComponent)
            );
            m_SpawnParticleQuery = GetEntityQuery(
                ComponentType.ReadWrite<SpawnParticleComponent>()
            );
            m_UninitializedTagQuery = GetEntityQuery(typeof(UninitializedTagComponent));
            m_EndInitEcbSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            int particleCount = 0;
            Entities.ForEach((ref SpawnParticleComponent spawn) => particleCount += spawn.Count).Run();
            using (var entities = new NativeArray<Entity>(particleCount, Allocator.Temp))
            {
                EntityManager.CreateEntity(m_ParticleArchetype, entities);
            }
            
            var entityCommandBuffer = m_EndInitEcbSystem.CreateCommandBuffer();
            entityCommandBuffer.DestroyEntity(m_SpawnParticleQuery);
            entityCommandBuffer.RemoveComponent(m_UninitializedTagQuery, typeof(UninitializedTagComponent));

            var seed = (1 + (uint)UnityEngine.Time.frameCount) * 104729;
            const float sphereRadius = 50;
            return Entities.WithAll<UninitializedTagComponent>().ForEach((Entity entity, ref PositionComponent position) =>
            {
                var rng = new Random(seed * (uint)(1 + entity.Index));
                position.Value = sphereRadius * rng.NextFloat3Direction() * rng.NextFloat();
            }).Schedule(inputDeps);
        }
    }
}
