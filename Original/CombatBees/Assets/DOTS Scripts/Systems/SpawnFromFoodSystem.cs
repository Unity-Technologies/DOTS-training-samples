using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpawnFromFoodSystem : SystemBase
{
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<SpawnZones>();
        RequireForUpdate(m_Query);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var zones = GetSingleton<SpawnZones>();
        var physicsTeam1Data = EntityManager.GetComponentData<PhysicsData>(zones.BeeTeam1Prefab);
        var physicsTeam2Data = EntityManager.GetComponentData<PhysicsData>(zones.BeeTeam2Prefab);
        var random = new Random((uint)UnityEngine.Random.Range(0, Int32.MaxValue));
        Entities
            .WithName("SpawnFromFood")
            .WithAll<FoodTag>()
            .WithNone<CarrierBee>()
            .WithStoreEntityQueryInField(ref m_Query)
            .ForEach((Entity e, in PhysicsData d, in Translation t) =>
            {
                if (t.Value.y <= zones.LevelBounds.Min.y)
                {
                    const float speed = 5000;
                    if (zones.Team1Zone.Contains(t.Value))
                    {
                        for (int i = 0; i < zones.BeesPerFood; ++i)
                        {
                            var newBee = ecb.Instantiate(zones.BeeTeam1Prefab);
                            ecb.SetComponent(newBee, new Translation
                            {
                                Value = t.Value,
                            });
                            var newPhysics = physicsTeam1Data;
                            newPhysics.a = random.NextFloat3Direction() * random.NextFloat() * speed;
                            ecb.SetComponent(newBee, newPhysics);
                        }

                        ecb.DestroyEntity(e);
                    }
                    if (zones.Team2Zone.Contains(t.Value))
                    {
                        for (int i = 0; i < zones.BeesPerFood; ++i)
                        {
                            var newBee = ecb.Instantiate(zones.BeeTeam2Prefab);
                            ecb.SetComponent(newBee, new Translation
                            {
                                Value = t.Value,
                            });
                            var newPhysics = physicsTeam2Data;
                            newPhysics.a = random.NextFloat3Direction() * random.NextFloat() * speed;
                            ecb.SetComponent(newBee, newPhysics);
                        }

                        ecb.DestroyEntity(e);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}