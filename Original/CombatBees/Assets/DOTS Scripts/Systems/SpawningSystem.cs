using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawningSystem : SystemBase
{
    EntityQuery m_Query;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SpawnZones>();
        RequireForUpdate(m_Query);
    }

    protected override void OnUpdate()
    {
        var zones = GetSingleton<SpawnZones>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random(1234);
        Entities
            .WithStoreEntityQueryInField(ref m_Query)
            .ForEach((Entity e, in InitializationSpawner s) =>
        {
            for (int i = 0; i < s.NumberOfBees; ++i)
            {
                {
                    var newBee = ecb.Instantiate(zones.BeeTeam1Prefab);
                    ecb.SetComponent(newBee, new Translation
                    {
                        Value = random.NextFloat3(zones.Team1Zone.Min, zones.Team1Zone.Max),
                    });
                }
                {
                    var newBee = ecb.Instantiate(zones.BeeTeam2Prefab);
                    ecb.SetComponent(newBee, new Translation
                    {
                        Value = random.NextFloat3(zones.Team2Zone.Min, zones.Team2Zone.Max),
                    });
                }
            }

            for (int i = 0; i < s.NumberOfFood; ++i)
            {
                var newFood = ecb.Instantiate(zones.FoodPrefab);
                ecb.SetComponent(newFood, new Translation
                {
                    Value = random.NextFloat3(s.FoodSpawnBox.Min, s.FoodSpawnBox.Max),
                });
            }

            ecb.RemoveComponent<InitializationSpawner>(e);
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
