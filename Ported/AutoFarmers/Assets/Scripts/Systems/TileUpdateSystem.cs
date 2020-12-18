using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Unity.Jobs;

public class TileUpdateSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        var tileBuffer = GetBufferFromEntity<TileState>(true);

        var jobHandle = Entities
            .WithReadOnly(tileBuffer)
            .ForEach((Entity entity, int entityInQueryIndex, ref Tile tile, in Translation translation) =>
            {
                var tiles = tileBuffer[data];
                var index = PositionToTileIndex(translation.Value, settings.GridSize.x);
                var currentTileState = tiles[index].Value;

                if (tile.State != currentTileState)
                {
                    RemoveTileComponent(ecb, entityInQueryIndex, entity, tile.State);
                    SetTileComponent(ecb, entityInQueryIndex, entity, currentTileState);   
                    
                    tile.State = currentTileState;
                }
                
            }).Schedule(inputDeps);
        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(jobHandle);
        return default;
    }

    static int PositionToTileIndex(float3 position, int gridDimension)
    {
        var tilePos = new int2((int)math.floor(position.x), (int)math.floor(position.z));
        return tilePos.x + tilePos.y * gridDimension;
    }
    
    static void RemoveTileComponent(EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity entity, ETileState state)
    {
        switch (state)
        {
            case ETileState.Empty:
                ecb.RemoveComponent<EmptyTile>(sortKey, entity);
                break;
            case ETileState.Tilled:    
                ecb.RemoveComponent<TilledTile>(sortKey, entity);
                break;
            case ETileState.Store:
                ecb.RemoveComponent<StoreTile>(sortKey, entity);
                break;
            case ETileState.Rock:
                ecb.RemoveComponent<RockTile>(sortKey, entity);
                break;
            case ETileState.Seeded:
                ecb.RemoveComponent<SeededTile>(sortKey, entity);
                break;
            case ETileState.Grown:
                ecb.RemoveComponent<GrownTile>(sortKey, entity);
                break;
        }
    }

    static void SetTileComponent(EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity entity, ETileState state)
    {
        switch (state)
        {
            case ETileState.Empty:
                ecb.AddComponent<EmptyTile>(sortKey, entity);
                break;
            case ETileState.Tilled:    
                ecb.AddComponent<TilledTile>(sortKey, entity);
                break;
            case ETileState.Store:
                ecb.AddComponent<StoreTile>(sortKey, entity);
                break;
            case ETileState.Rock:
                ecb.AddComponent<RockTile>(sortKey, entity);
                break;
            case ETileState.Seeded:
                ecb.AddComponent<SeededTile>(sortKey, entity);
                break;
            case ETileState.Grown:
                ecb.AddComponent<GrownTile>(sortKey, entity);
                break;
        }
    }
}
