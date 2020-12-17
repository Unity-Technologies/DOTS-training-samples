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

public struct BucketTeamSpawner : IComponentData
{
    public Entity Prefab;
}

public class BucketTeamSpawnerSystem : SystemBase
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

        var numBucketEmpty = FireSimConfig.numEmptyBots;
        var numBucketFull = FireSimConfig.numFullBots;

        float4 fullBotColor = FireSimConfig.emptyBotColor;
        float4 emptyBotColor = FireSimConfig.fullBotColor;

        Entities.ForEach((Entity entity, in BucketTeamSpawner bucketTeamSpawner) =>
        {
            ecb.DestroyEntity(entity);

            for (int i=0; i<maxTeams; ++i)
            {
                for (int j=0; j<numBucketEmpty; ++j)
                {
                    Entity bb1 = ecb.Instantiate(bucketTeamSpawner.Prefab);
                    ecb.AddComponent<BucketEmptyBot>(bb1, new BucketEmptyBot { Index = j, Position = float2.zero });
					ecb.AddComponent<URPMaterialPropertyBaseColor>(bb1, new URPMaterialPropertyBaseColor { Value = emptyBotColor });
                    ecb.AddComponent(bb1, new TeamIndex {Value = i});
                }

                for (int j=0; j<numBucketFull; ++j)
                {
                    Entity bb0 = ecb.Instantiate(bucketTeamSpawner.Prefab);
                    ecb.AddComponent<BucketFullBot>(bb0,  new BucketFullBot  { Index = j, Position = float2.zero });
					ecb.AddComponent<URPMaterialPropertyBaseColor>(bb0, new URPMaterialPropertyBaseColor { Value = fullBotColor });
                    ecb.AddComponent(bb0, new TeamIndex {Value = i});
                }
            }
        }).Run();
    }
}
