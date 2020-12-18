using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

public class TileUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        
        Entities
            .ForEach((Entity entity, ref Tile tile, in Translation translation) =>
            {
                var index = PositionToTileIndex(translation.Value, settings.GridSize.x);
                var currentTileState = tileBuffer[index].Value;

                if (tile.State != currentTileState)
                {
                    RemoveTileComponent(ecb, entity, tile.State);
                    SetTileComponent(ecb, entity, currentTileState);   
                    
                    tile.State = currentTileState;
                }
                
            }).Run();

        ecb.Playback(EntityManager);
    }

    static int PositionToTileIndex(float3 position, int gridDimension)
    {
        var tilePos = new int2((int)math.floor(position.x), (int)math.floor(position.z));
        return tilePos.x + tilePos.y * gridDimension;
    }
    
    static void RemoveTileComponent(EntityCommandBuffer ecb, Entity entity, ETileState state)
    {
        switch (state)
        {
            case ETileState.Empty:
                ecb.RemoveComponent<EmptyTile>(entity);
                break;
            case ETileState.Tilled:    
                ecb.RemoveComponent<TilledTile>(entity);
                break;
            case ETileState.Store:
                ecb.RemoveComponent<StoreTile>(entity);
                break;
            case ETileState.Rock:
                ecb.RemoveComponent<RockTile>(entity);
                break;
            case ETileState.Seeded:
                ecb.RemoveComponent<SeededTile>(entity);
                break;
            case ETileState.Grown:
                ecb.RemoveComponent<GrownTile>(entity);
                break;
        }
    }

    static void SetTileComponent(EntityCommandBuffer ecb, Entity entity, ETileState state)
    {
        switch (state)
        {
            case ETileState.Empty:
                ecb.AddComponent<EmptyTile>(entity);
                break;
            case ETileState.Tilled:    
                ecb.AddComponent<TilledTile>(entity);
                break;
            case ETileState.Store:
                ecb.AddComponent<StoreTile>(entity);
                break;
            case ETileState.Rock:
                ecb.AddComponent<RockTile>(entity);
                break;
            case ETileState.Seeded:
                ecb.AddComponent<SeededTile>(entity);
                break;
            case ETileState.Grown:
                ecb.AddComponent<GrownTile>(entity);
                break;
        }
    }
}
