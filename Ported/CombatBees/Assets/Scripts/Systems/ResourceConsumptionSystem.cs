using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(MovementSystem))]
public class ResourceConsumptionSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var b = GetSingleton<BattleField>();

        Entities
            .WithAll<Resource>()
            .WithNone<Parent>()
            .ForEach((int entityInQueryIndex,Entity entity, in Translation translation) =>
        {
            if (math.abs(translation.Value.z) > math.abs(b.HiveDistance))
            {
                if (translation.Value.y < (-b.Bounds.y / 2) + 0.1)
                {
                    // destroy
                    ecb.DestroyEntity(entity);

                    var smokeSpawner = ecb.Instantiate(b.SmokeSpawner);
                    ecb.SetComponent<Translation>(smokeSpawner, translation);
                    
                    var beeSpawner = ecb.Instantiate(b.BeeSpawner);
                    float3 pos = translation.Value;
                    pos.y += 1;
                    ecb.SetComponent<Translation>(beeSpawner, new Translation{ Value = pos });

                    // spawn
                    if (translation.Value.z > 0f)
                    {
                        ecb.AddComponent<TeamB>(beeSpawner);
                    }
                    else
                    {
                        ecb.AddComponent<TeamA>(beeSpawner);
                    }
                }
            }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

