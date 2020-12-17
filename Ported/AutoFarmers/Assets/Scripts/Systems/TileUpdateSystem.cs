using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class TileUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var settings = GetSingleton<CommonSettings>();
        var data = GetSingletonEntity<CommonData>();
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        
        // NOTE: We can't turn this into a generic method. :( 
        Entities
           .WithAll<EmptyTile>()
           .ForEach((Entity entity, in Translation translation) =>
           {
               var index = PositionToTileIndex(translation.Value, settings.GridSize.x);
               var tileState = tileBuffer[index].Value;

               if (tileState != ETileState.Empty)
               {
                   ecb.RemoveComponent<EmptyTile>(entity);
                   SetTileComponent(ecb, entity, tileState);   
               }
           }).Run();
        
        Entities
            .WithAll<TilledTile>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                var index = PositionToTileIndex(translation.Value, settings.GridSize.x);
                var tileState = tileBuffer[index].Value;

                if (tileState != ETileState.Tilled)
                {
                    ecb.RemoveComponent<EmptyTile>(entity);
                    SetTileComponent(ecb, entity, tileState);   
                }
            }).Run();
        
        Entities
            .WithAll<SeededTile>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                var index = PositionToTileIndex(translation.Value, settings.GridSize.x);
                var tileState = tileBuffer[index].Value;

                if (tileState != ETileState.Seeded)
                {
                    ecb.RemoveComponent<EmptyTile>(entity);
                    SetTileComponent(ecb, entity, tileState);   
                }
            }).Run();
        
        Entities
            .WithAll<GrownTile>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                var index = PositionToTileIndex(translation.Value, settings.GridSize.x);
                var tileState = tileBuffer[index].Value;

                if (tileState != ETileState.Grown)
                {
                    ecb.RemoveComponent<EmptyTile>(entity);
                    SetTileComponent(ecb, entity, tileState);   
                }
            }).Run();
        
        Entities
            .WithAll<StoreTile>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                var index = PositionToTileIndex(translation.Value, settings.GridSize.x);
                var tileState = tileBuffer[index].Value;

                if (tileState != ETileState.Store)
                {
                    ecb.RemoveComponent<EmptyTile>(entity);
                    SetTileComponent(ecb, entity, tileState);   
                }
            }).Run();
        
        Entities
            .WithAll<RockTile>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                var index = PositionToTileIndex(translation.Value, settings.GridSize.x);
                var tileState = tileBuffer[index].Value;

                if (tileState != ETileState.Rock)
                {
                    ecb.RemoveComponent<EmptyTile>(entity);
                    SetTileComponent(ecb, entity, tileState);   
                }
            }).Run();
    }

    static int PositionToTileIndex(float3 position, int gridDimension)
    {
        var tilePos = new int2((int)math.floor(position.x), (int)math.floor(position.z));
        return tilePos.x + tilePos.y * gridDimension;
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
