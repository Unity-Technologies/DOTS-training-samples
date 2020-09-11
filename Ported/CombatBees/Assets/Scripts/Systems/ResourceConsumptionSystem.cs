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
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var b = GetSingleton<BattleField>();
        float floor = (-b.Bounds.y / 2f) + 0.1f;
        Entities
            .WithAll<Resource>()
            .WithNone<Parent>()
            .ForEach((int entityInQueryIndex, Entity entity, in Translation translation) =>
        {
            if (math.abs(translation.Value.z) > math.abs(b.HiveDistance))
            {
                if (translation.Value.y < floor)
                {
                    // destroy
                    ecb.DestroyEntity(entityInQueryIndex, entity);

                    // TODO why not do the smoke spawning here?
                    var smokeSpawnerInstance = ecb.Instantiate(entityInQueryIndex, b.SmokeSpawner);
                    ecb.SetComponent<Translation>(entityInQueryIndex, smokeSpawnerInstance, translation);
                    
                    var beeSpawnerInstance = ecb.Instantiate(entityInQueryIndex, b.BeeSpawner);
                    ecb.SetComponent<Translation>(entityInQueryIndex, beeSpawnerInstance, 
                        new Translation{ Value = new float3( translation.Value.x, translation.Value.y + 1, translation.Value.z ) });

                    // spawn
                    if (translation.Value.z > 0f)
                    {
                        ecb.AddComponent<TeamB>(entityInQueryIndex, beeSpawnerInstance);
                    }
                    else
                    {
                        ecb.AddComponent<TeamA>(entityInQueryIndex, beeSpawnerInstance);
                    }
                }
            }
        }).ScheduleParallel();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

