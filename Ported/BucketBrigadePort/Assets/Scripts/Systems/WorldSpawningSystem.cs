using Unity.Collections;
using Unity.Entities;
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
        var tileSpawner = GetSingleton<TileSpawner>(); 
        Entities
            .WithStructuralChanges()
            .ForEach((Entity spawnerEntity, in InitializeWorldAuthority initializeWorld) => // , in TileSpawner tileSpawner
            {
                var blankTiles = EntityManager.Instantiate(tileSpawner.Prefab, tileSpawner.XSize * tileSpawner.YSize, Allocator.Temp);
                // TODO: Scale prefab itself by tileSpawner.Scale. Currently prefab hardcoded to 0.3 and tileSpawner.Scale set in UI to match

                var random = new Random(1);
                int tileCount = 0;
                for (int x = 0; x < tileSpawner.XSize; x++)
                {
                    for (int y = 0; y < tileSpawner.YSize; y++)
                    {
                        var yPosition = random.NextFloat(0, 0.05f) -0.5f; // Height vs. pivot of prefab. TODO: Read prefab data
                        var position = new Translation() { Value = new float3(x * tileSpawner.Scale, yPosition, y * tileSpawner.Scale) };
                        EntityManager.SetComponentData(blankTiles[tileCount], position);

                        EntityManager.AddComponentData<Color>(blankTiles[tileCount], new Color());
                        // TODO: Add scaling
                        // var scale = new Scale() { Value = tileSpawner.Scale };
                        // EntityManager.SetComponentData(blankTiles[tileCount], scale);

                        var tileAuthor = new Tile();
                        tileAuthor.Id = y * tileSpawner.XSize + x;
                        EntityManager.SetComponentData(blankTiles[tileAuthor.Id], tileAuthor);

                        tileCount++;
                    }
                }
                blankTiles.Dispose();
                EntityManager.DestroyEntity(spawnerEntity);
            }).Run();

        // TODO: Store 2d array of entities
    }
}
