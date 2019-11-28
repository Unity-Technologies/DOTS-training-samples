using System;
using System.ComponentModel;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class AntSpawningSystem : JobComponentSystem
{
    EntityArchetype m_AntArchetype;
    EntityQuery m_SpawnQuery;
    EndInitializationEntityCommandBufferSystem m_EndInitializationEntityCommandBufferSystem;
    EntityQuery m_RemoveUninitTagQuery;
    EntityQuery m_MapSettingsQuery;

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
            typeof(RandomSteeringComponent),

            typeof(RenderColorComponent),
            typeof(BrightnessComponent),
            
            typeof(HasResourcesComponent),
            
            typeof(UninitializedTagComponent)
        );
        m_SpawnQuery = GetEntityQuery(ComponentType.ReadWrite<AntSpawnComponent>());
        m_EndInitializationEntityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        m_RemoveUninitTagQuery = GetEntityQuery(ComponentType.ReadWrite<UninitializedTagComponent>());
        m_MapSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        int totalAmount = 0;
        Entities.ForEach((ref AntSpawnComponent spawn) =>
        {
            totalAmount += spawn.Amount;
        }).Run();

        using (var entities = new NativeArray<Entity>(totalAmount, Allocator.Temp))
        {
            EntityManager.CreateEntity(m_AntArchetype, entities);
        }

        var ecb = m_EndInitializationEntityCommandBufferSystem.CreateCommandBuffer();
        ecb.DestroyEntity(m_SpawnQuery);
        ecb.RemoveComponent(m_RemoveUninitTagQuery, typeof(UninitializedTagComponent));
        
        uint seed = 1 + (uint)UnityEngine.Time.frameCount;
        var mapSize = m_MapSettingsQuery.GetSingleton<MapSettingsComponent>().MapSize;
        return Entities.WithAll<UninitializedTagComponent>().ForEach((Entity entity, ref BrightnessComponent brightness, ref FacingAngleComponent facingAngle, ref PositionComponent position, ref RandomSteeringComponent random) =>
        {
            var rng = new Random(((uint)entity.Index + 1) * seed * 100151);
            facingAngle.Value = rng.NextFloat() * 2 * math.PI;
            brightness.Value = rng.NextFloat(0.75f, 1.25f);
            position.Value = .5f * mapSize + new float2(rng.NextFloat(-5, 5), rng.NextFloat(-5, 5));
            random.Rng = rng;
        }).Schedule(inputDeps);
    }
}