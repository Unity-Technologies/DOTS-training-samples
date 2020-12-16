using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;

public struct ThrowerSpawner : IComponentData
{
    public Entity Prefab;
}

public class ThrowerSpawnerSystem : SystemBase
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();

        var xDim = FireSimConfig.xDim;
        var yDim = FireSimConfig.yDim;
        var maxTeams = FireSimConfig.maxTeams;

        Entities.ForEach((Entity entity, in ThrowerSpawner throwerSpawner) =>
        {
            ecb.DestroyEntity(entity);

            for (int i=0; i<maxTeams; ++i)
            {
                unsafe {
                    // jiv fixme: locate at water sources
                    int2* poss = stackalloc int2[]
                    {
                        new int2(0,      yDim-1),
                            new int2(xDim-1, yDim-1),
                            new int2(xDim-1, 0),
                            new int2(0,      0)
                            };
            
                    Entity throwerEntity = ecb.Instantiate(throwerSpawner.Prefab);
                    ecb.AddComponent<Thrower>(throwerEntity, new Thrower { TeamIndex = i, Coord = poss[i&3] });
                }
            }
        }).Run();
    }
}
