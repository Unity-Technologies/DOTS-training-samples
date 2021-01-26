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
                    var newBee = ecb.Instantiate(s.BeePrefab);
                    ecb.SetComponent(newBee, new Translation
                    {
                        Value = random.NextFloat3(zones.Team1Zone.Min, zones.Team1Zone.Max),
                    });
                    ecb.AddComponent<Team1>(newBee);
                    ecb.AddComponent(newBee, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4(1, 1, 0, 1),
                    });
                }
                {
                    var newBee = ecb.Instantiate(s.BeePrefab);
                    ecb.SetComponent(newBee, new Translation
                    {
                        Value = random.NextFloat3(zones.Team2Zone.Min, zones.Team2Zone.Max),
                    });
                    ecb.AddComponent<Team2>(newBee);
                    ecb.AddComponent(newBee, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4(0, 1, 1, 1),
                    });
                }
            }

            ecb.RemoveComponent<InitializationSpawner>(e);
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
