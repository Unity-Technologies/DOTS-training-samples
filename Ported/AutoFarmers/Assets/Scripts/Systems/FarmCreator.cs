using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class FarmCreator : SystemBase
{
    protected override void OnUpdate()
    {
        var tileBufferAccessor = this.GetBufferFromEntity<TileState>();
            
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Unity.Mathematics.Random(1234);
        Entities
            .ForEach((Entity entity, in TileSpawner spawner) =>
            {
                var tileBuffer = tileBufferAccessor[spawner.Settings];
                
                int gridLinearSize = spawner.GridSize.x * spawner.GridSize.y;
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                NativeArray<Entity> tiles = new NativeArray<Entity>(gridLinearSize, Allocator.Temp);
                
                for (int i = 0; i < spawner.GridSize.x; ++i)
                {
                    for (int j = 0; j < spawner.GridSize.y; ++j)
                    {
                        var instance = ecb.Instantiate(spawner.TilePrefab);
                        
                        var translation = new Translation { Value = new float3(i * spawner.TileSize.x, -1f, j * spawner.TileSize.y) };
                        var size = new Size { Value = spawner.TileSize };

                        ecb.SetComponent(instance, translation);
                        ecb.AddComponent(instance, size);

                        var linearIndex = i + j * spawner.GridSize.x;
                        tiles[linearIndex] = instance;
                    }
                }

                int spawnedStores = 0;
                while (spawnedStores < spawner.StoreCount)
                {
                    int x = random.NextInt(0, spawner.GridSize.x);
                    int y = random.NextInt(0, spawner.GridSize.y);
                    var storePosition = new Vector2Int(x, y);

                    var linearIndex = storePosition.x + storePosition.y * spawner.GridSize.x;
                    
                    if ( tileBuffer[linearIndex].Value == TileStates.Store )//stores[storePosition.x + storePosition.y * spawner.GridSize.x])
                        continue;

                    var instance = ecb.Instantiate(spawner.SiloPrefab);
                    var translation = new Translation { Value = new float3((x + 0.5f) * spawner.TileSize.x, 0, (y + 0.5f) * spawner.TileSize.y) };
                    ecb.SetComponent(instance, translation);

                    tileBuffer[linearIndex] = new TileState { Value = TileStates.Store };

                    var store = new Store { };
                    ecb.AddComponent(tiles[linearIndex], store);
                    spawnedStores++;
                }

                for (int i = 0; i < spawner.Attempts; i++)
                {
                    var width = random.NextInt(1, 4);
                    var height = random.NextInt(1, 4);
                    var rockX = random.NextInt(0, spawner.GridSize.x - width);
                    var rockY = random.NextInt(0, spawner.GridSize.y - height);

                    bool blocked = false;

                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            var tileValue = tileBuffer[(rockX + j) + (rockY + k) * spawner.GridSize.x].Value;
                            blocked |= tileValue == TileStates.Rock || tileValue == TileStates.Store;
                        }
                    }

                    if (blocked)
                        continue;

                    var instance = ecb.Instantiate(spawner.RockPrefab);
                    var position = new RectPosition { Value = new int2(rockX, rockY) };
                    var size = new RectSize { Value = new int2(width, height) };
                    var translation = new Translation { Value = new float3(rockX * spawner.TileSize.x + width * 0.5f * spawner.TileSize.x, -0.5f, rockY * spawner.TileSize.y + height * 0.5f * spawner.TileSize.y) };
                    var scale = new NonUniformScale { Value = new float3(spawner.TileSize.x * (width - 0.5f), 1, spawner.TileSize.y * (height - 0.5f)) };

                    ecb.SetComponent(instance, translation);
                    ecb.AddComponent(instance, position);
                    ecb.AddComponent(instance, scale);
                    ecb.AddComponent(instance, size);
                    ecb.AddComponent(instance, new Rock {Health = 5, Position = new int2(rockX, rockY), Size = new int2(width, height)});
                    
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            var linearIndex = (rockX + j) + (rockY + k) * spawner.GridSize.x;
                            
                            tileBuffer[linearIndex] = new TileState
                            {
                                Value = TileStates.Rock
                            };
                        }
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
