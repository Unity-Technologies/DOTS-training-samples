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

public struct BotSpawner : IComponentData
{
    public Entity FetcherPrefab;
    public Entity ThrowerPrefab;
    public Entity BotPrefab;
}

public class FetcherSpawnerSystem : SystemBase
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<WaterSourceVolume>()));
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<BotSpawner>()));
    }

    protected override void OnUpdate()
    {
        var xDim = FireSimConfig.xDim;
        var yDim = FireSimConfig.yDim;
        var maxTeams = FireSimConfig.maxTeams;

        uint seed0 = (uint)Environment.TickCount;
        uint seed1 = (uint)(Time.DeltaTime*100000);
        uint kSeed = seed0 ^ seed1;

        var numBucketEmpty = FireSimConfig.numEmptyBots;
        var numBucketFull = FireSimConfig.numFullBots;

        float4 fullBotColor = VisualConfig.kEmptyBotColor;
        float4 emptyBotColor = VisualConfig.kFullBotColor;

        int nWaterSources = WaterConfig.nWaterSources;

        NativeArray<int2> waterCoords = new NativeArray<int2>(nWaterSources, Allocator.TempJob);
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, in WaterSourceVolume waterSourceVolume, in Position position) =>
        {
            waterCoords[entityInQueryIndex] = new int2(position.coord);
        }).Run();

        EntityCommandBuffer ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithDisposeOnCompletion(waterCoords)
            .ForEach((Entity entity, in BotSpawner botSpawner) =>
        {
            ecb.DestroyEntity(entity);

            Unity.Mathematics.Random random = new Unity.Mathematics.Random(kSeed);
            int2 midpoint = new int2(xDim/2, yDim/2);

            for (int i=0; i<maxTeams; ++i)
            {
                int2 waterSourceCoord = waterCoords[i % nWaterSources];

                Entity fetcherEntity = ecb.Instantiate(botSpawner.FetcherPrefab);
                ecb.AddComponent<Fetcher>(fetcherEntity, new Fetcher {});
                ecb.AddComponent<Position>(fetcherEntity, new Position {coord = waterSourceCoord});
                ecb.AddComponent<TeamIndex>(fetcherEntity, new TeamIndex {Value = i});
                ecb.AddComponent<FetcherFindBucket>(fetcherEntity, new FetcherFindBucket {});

                Entity throwerEntity = ecb.Instantiate(botSpawner.BotPrefab);
                ecb.AddComponent<Thrower>(throwerEntity, new Thrower
                {
                    Coord = waterSourceCoord,
                    TargetCoord = waterSourceCoord,
                    GridPosition = new float2(waterSourceCoord)
                });
                ecb.AddComponent(throwerEntity, new TeamIndex {Value = i});

                for (int j=0; j<numBucketEmpty; ++j)
                {
                    Entity bb1 = ecb.Instantiate(botSpawner.BotPrefab);
                    ecb.AddComponent<BucketEmptyBot>(bb1, new BucketEmptyBot { Index = j, Position = float2.zero });
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(bb1, new URPMaterialPropertyBaseColor { Value = emptyBotColor });
                    ecb.AddComponent(bb1, new TeamIndex {Value = i});
                }

                for (int j=0; j<numBucketFull; ++j)
                {
                    Entity bb0 = ecb.Instantiate(botSpawner.BotPrefab);
                    ecb.AddComponent<BucketFullBot>(bb0,  new BucketFullBot  { Index = j, Position = float2.zero });
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(bb0, new URPMaterialPropertyBaseColor { Value = fullBotColor });
                    ecb.AddComponent(bb0, new TeamIndex {Value = i});
                }
            }
        }).Run();
    }
}
