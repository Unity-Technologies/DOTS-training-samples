using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FireIntializationSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        
        RequireSingletonForUpdate<FireSpawning>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        Entities.WithoutBurst().
            ForEach((Entity configEntity, in FireConfiguration config, in FireSpawning spawning) =>
        {
            var cellSize = config.CellSize;
            for (int z = 0; z < config.GridHeight; ++z)
            {
                for (int x = 0; x < config.GridWidth; ++x)
                {
                    var instance = ecb.Instantiate(spawning.Prefab);
                    var translation = new float3(x - (config.GridWidth - 1) / 2f, 0, z);
                    translation *= cellSize * 1.1f;

                    ecb.SetComponent(instance, new Translation {Value = translation});
                }
            }
        }).Schedule();

        Entities.ForEach((Entity fireSpawningEntity, in FireSpawning spawning) =>
            {
                ecb.DestroyEntity(fireSpawningEntity);
            }
            ).Schedule();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
