using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

        Entities.WithAll<Resource>().WithNone<Parent>().ForEach((int entityInQueryIndex,Entity entity, in Translation translation) =>
        {
            if (math.abs(translation.Value.z) > math.abs(b.HiveDistance))
            {
                if (translation.Value.y < (-b.Bounds.y / 2) + 0.1)
                {
                    // destroy
                    ecb.DestroyEntity(entityInQueryIndex, entity);

                    var beeSpawner = ecb.Instantiate(entityInQueryIndex, b.BeeSpawner);
                    ecb.SetComponent<Translation>(entityInQueryIndex, beeSpawner, translation);

                    // spawn
                    if (translation.Value.z > 0f)
                    {
                        ecb.AddComponent<TeamB>(entityInQueryIndex, beeSpawner);
                    }
                    else
                    {
                        ecb.AddComponent<TeamA>(entityInQueryIndex, beeSpawner);

                    }
                }
            }


        }).ScheduleParallel();
        // TODO: ask about this
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

