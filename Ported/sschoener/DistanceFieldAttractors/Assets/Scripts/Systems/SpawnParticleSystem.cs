using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SpawnParticleSystem : ComponentSystem
    {
        EntityArchetype m_ParticleArchetype;
        EntityQuery m_SpawnParticleQuery;

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
        }

        protected override void OnUpdate()
        {
            // this has to happen on the main thread anyway, might as well use a ComponentSystem
            Entities.ForEach((ref SpawnParticleComponent spawn) =>
                {
                    using (var entities = new NativeArray<Entity>(spawn.Count, Allocator.Temp))
                    {
                        EntityManager.CreateEntity(m_ParticleArchetype, entities);
                    }
                }
            );
            EntityManager.DestroyEntity(m_SpawnParticleQuery);
        }
    }
}
