
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class MapSpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Unity.Mathematics.Random(1234);

        Entities
            .ForEach((Entity entity, in MapSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int y = 0; y < spawner.MapHeight; ++y)
                {
                    for (int x = 0; x < spawner.MapWidth; ++x)
                    {
                        var tile = ecb.Instantiate(spawner.TilePrefab);
                        ecb.SetComponent(tile, new Translation
                        {
                            Value = new float3(x, 0, y)
                        });
                        var walls = (y == 0 ? DirectionEnum.North : DirectionEnum.None) |
                                    (y == spawner.MapHeight - 1 ? DirectionEnum.South : DirectionEnum.None) |
                                    (x == 0 ? DirectionEnum.West : DirectionEnum.None) |
                                    (x == spawner.MapWidth - 1 ? DirectionEnum.East : DirectionEnum.None);
                        // introduce random walls here, later 
                        ecb.SetComponent(tile, new Tile
                        {
                            Coords = new int2(x, y),
                            Walls = walls
                                
                        });
                        ecb.SetComponent(tile, new URPMaterialPropertyBaseColor
                        {
                            Value = (x & 1) == (y & 1)? spawner.TileEvenColor : spawner.TileOddColor
                        });
                    }
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
