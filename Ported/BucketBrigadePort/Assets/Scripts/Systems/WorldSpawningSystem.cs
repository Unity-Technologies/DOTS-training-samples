using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class WorldSpawningSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Create queries and static data only here.
    }
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((Entity spawnerEntity, in TileSpawner tileSpawner, in Translation translation) =>
            {
                var blankTiles = EntityManager.Instantiate(tileSpawner.Prefab, tileSpawner.XSize * tileSpawner.YSize, Allocator.Temp);
                int tileCount = 0;
                for (int x = 0; x < tileSpawner.XSize; x++)
                {
                    for (int y = 0; y < tileSpawner.YSize; y++)
                    {
                        var position = new Translation() { Value = new float3(x, 0, y) };
                        EntityManager.SetComponentData(blankTiles[tileCount], position);

                        var tileAuthor = new Tile();
                        tileAuthor.Id = new int2() { x = x, y = y};
                        EntityManager.SetComponentData(blankTiles[tileCount], tileAuthor);

                        tileCount++;
                    }
                }
                blankTiles.Dispose();
                EntityManager.DestroyEntity(spawnerEntity);
            }).Run();
    }
}
