using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;

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
        int kJitter = 10;

        uint seed0 = (uint)Environment.TickCount;
        uint seed1 = (uint)(Time.DeltaTime*100000);
        uint kSeed = seed0 ^ seed1;

        Entities.ForEach((Entity entity, in ThrowerSpawner throwerSpawner) =>
        {
            ecb.DestroyEntity(entity);

            Unity.Mathematics.Random random = new Unity.Mathematics.Random(kSeed);
            int2 midpoint = new int2(xDim/2, yDim/2);

            for (int i=0; i<maxTeams; ++i)
            {
                unsafe {
#if false
                    // jiv fixme: locate at water sources
                    int2* poss = stackalloc int2[]
                    {
                        new int2(0,      yDim-1),
                        new int2(xDim-1, yDim-1),
                        new int2(xDim-1, 0),
                        new int2(0,      0)
                    };
#else
                    // distribute randomly around center
                    int2* poss = stackalloc int2[]
                    {
                        midpoint + new int2(random.NextInt(-kJitter, kJitter), random.NextInt(-kJitter, kJitter)),
                        midpoint + new int2(random.NextInt(-kJitter, kJitter), random.NextInt(-kJitter, kJitter)),
                        midpoint + new int2(random.NextInt(-kJitter, kJitter), random.NextInt(-kJitter, kJitter)),
                        midpoint + new int2(random.NextInt(-kJitter, kJitter), random.NextInt(-kJitter, kJitter))
                    };
#endif

                    Entity throwerEntity = ecb.Instantiate(throwerSpawner.Prefab);
                    ecb.AddComponent<Thrower>(throwerEntity, new Thrower { Coord = poss[i&3] });
                    ecb.AddComponent(throwerEntity, new TeamIndex {Value = i});
                }
            }
        }).Run();
    }
}
