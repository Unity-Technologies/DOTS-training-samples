using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public class FarmCreator : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random(1234);
        Entities
            .ForEach((Entity entity, in TileSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.GridSize.x; ++i)
                {
                    for (int j = 0; j < spawner.GridSize.y; ++j)
                    {
                        var instance = ecb.Instantiate(spawner.TilePrefab);
                        
                        var translation = new Translation { Value = new float3(i * spawner.TileSize.x, 0, j * spawner.TileSize.y) };
                        var size = new Size { Value = spawner.TileSize };

                        ecb.SetComponent(instance, translation);
                        ecb.AddComponent(instance, size);
                    }
                }

                for (int i = 0; i < spawner.Attempts; i++) 
                {
                    var width = random.NextInt(0,4);
                    var height = random.NextInt(0,4);
                    var rockX = random.NextInt(0, spawner.GridSize.x - width);
                    var rockY = random.NextInt(0, spawner.GridSize.y - height);

                    var instance = ecb.Instantiate(spawner.RockPrefab);
                    var position = new RectPosition { Value = new int2(rockX, rockY) };
                    var size = new RectSize { Value = new int2(width, height) };
                    var translation = new Translation { Value = new float3(rockX * spawner.TileSize.x + width * 0.5f * spawner.TileSize.x , 0, rockY * spawner.TileSize.y + height * 0.5f * spawner.TileSize.y) };
                    var scale = new NonUniformScale { Value = new float3( spawner.TileSize.x * width, 1, spawner.TileSize.y * height ) };

                    ecb.SetComponent(instance, translation);
                    ecb.AddComponent(instance, position);
                    ecb.AddComponent(instance, scale);
                    ecb.AddComponent(instance, size);
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
