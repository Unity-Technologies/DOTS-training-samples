using System;
using System.ComponentModel;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class AntSpawningSystem : ComponentSystem
{
    EntityArchetype m_AntArchetype;
    EntityQuery m_SpawnQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_AntArchetype = EntityManager.CreateArchetype(
            typeof(LocalToWorldComponent),
            typeof(VelocityComponent),
            typeof(PositionComponent),
            typeof(FacingAngleComponent),
            typeof(SpeedComponent),
            
            typeof(PheromoneSteeringComponent),
            typeof(WallSteeringComponent),

            typeof(RenderColorComponent),
            typeof(BrightnessComponent),
            
            typeof(HasResourcesComponent),
            
            typeof(UninitializedTagComponent)
        );
        m_SpawnQuery = GetEntityQuery(ComponentType.ReadWrite<AntSpawnComponent>());
    }

    protected override void OnUpdate()
    {
        int totalAmount = 0;
        Entities.ForEach((ref AntSpawnComponent spawn) =>
        {
            totalAmount += spawn.Amount;
        });

        using (var entities = new NativeArray<Entity>(totalAmount, Allocator.Temp))
        {
            EntityManager.CreateEntity(m_AntArchetype, entities);
        }
        EntityManager.DestroyEntity(m_SpawnQuery);
    }
}