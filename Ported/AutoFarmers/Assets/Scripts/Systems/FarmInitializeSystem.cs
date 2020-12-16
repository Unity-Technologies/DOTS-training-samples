using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class FarmInitializeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var commonSettings = GetSingleton<CommonSettings>();
        
        var data = GetSingletonEntity<CommonData>();
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
            
        var random = new Unity.Mathematics.Random(1234);
        
        Entities
            .ForEach((Entity entity, in InitializationSettings initializationSettings) =>
            {
                ecb.DestroyEntity(entity);
                
                int gridLinearSize = commonSettings.GridSize.x * commonSettings.GridSize.y;

                NativeArray<Entity> tiles = new NativeArray<Entity>(gridLinearSize, Allocator.Temp);
                
                // Tiles
                for (int i = 0; i < commonSettings.GridSize.x; ++i)
                {
                    for (int j = 0; j < commonSettings.GridSize.y; ++j)
                    {
                        var instance = ecb.Instantiate(initializationSettings.TilePrefab);
                        
                        var translation = new Translation { Value = new float3(i * commonSettings.TileSize.x, -1f, j * commonSettings.TileSize.y) };
                        var size = new Size { Value = commonSettings.TileSize };

                        ecb.SetComponent(instance, translation);
                        ecb.AddComponent(instance, size);

                        var linearIndex = i + j * commonSettings.GridSize.x;
                        tiles[linearIndex] = instance;
                    }
                }

                // Stores
                var spawnedStores = 0;
                while (spawnedStores < commonSettings.StoreSpawnCount)
                {
                    int x = random.NextInt(0, commonSettings.GridSize.x);
                    int y = random.NextInt(0, commonSettings.GridSize.y);
                    var storePosition = new Vector2Int(x, y);

                    var linearIndex = storePosition.x + storePosition.y * commonSettings.GridSize.x;
                    
                    if (tileBuffer[linearIndex].Value == ETileState.Store)
                        continue;

                    var instance = ecb.Instantiate(initializationSettings.SiloPrefab);
                    var translation = new Translation { Value = new float3((x + 0.5f) * commonSettings.TileSize.x, 0, (y + 0.5f) * commonSettings.TileSize.y) };
                    ecb.SetComponent(instance, translation);

                    tileBuffer[linearIndex] = new TileState { Value = ETileState.Store };

                    var store = new Store { };
                    ecb.AddComponent(tiles[linearIndex], store);
                    spawnedStores++;
                }

                // Rocks
                for (int i = 0; i < commonSettings.RockSpawnAttempts; i++)
                {
                    var width = random.NextInt(1, 4);
                    var height = random.NextInt(1, 4);
                    var rockX = random.NextInt(0, commonSettings.GridSize.x - width);
                    var rockY = random.NextInt(0, commonSettings.GridSize.y - height);

                    bool blocked = false;

                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            var tileValue = tileBuffer[(rockX + j) + (rockY + k) * commonSettings.GridSize.x].Value;
                            blocked |= tileValue == ETileState.Rock || tileValue == ETileState.Store;
                        }
                    }

                    if (blocked)
                        continue;

                    var instance = ecb.Instantiate(initializationSettings.RockPrefab);
                    var position = new RectPosition { Value = new int2(rockX, rockY) };
                    var size = new RectSize { Value = new int2(width, height) };
                    var translation = new Translation { Value = new float3(rockX * commonSettings.TileSize.x + width * 0.5f * commonSettings.TileSize.x, -0.5f, rockY * commonSettings.TileSize.y + height * 0.5f * commonSettings.TileSize.y) };
                    var scale = new NonUniformScale { Value = new float3(commonSettings.TileSize.x * (width - 0.5f), 1, commonSettings.TileSize.y * (height - 0.5f)) };

                    ecb.SetComponent(instance, translation);
                    ecb.AddComponent(instance, position);
                    ecb.AddComponent(instance, scale);
                    ecb.AddComponent(instance, size);
                    ecb.AddComponent(instance, new Rock {Health = 1, Position = new int2(rockX, rockY), Size = new int2(width, height)});
                    
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            var linearIndex = (rockX + j) + (rockY + k) * commonSettings.GridSize.x;
                            tileBuffer[linearIndex] = new TileState{ Value = ETileState.Rock };
                        }
                    }
                }
                
                // Initial Farmers
                for (int i = 0; i < initializationSettings.InitialFarmersCount; i++)
                {
                    var instance = ecb.Instantiate(commonSettings.FarmerPrefab);
                    ecb.AddComponent(instance, new Farmer());
                    ecb.AddComponent(instance, new Velocity());
                    
                    //if (i == 0)
                        ecb.AddComponent(instance, new CameraTarget());

                    ecb.AddBuffer<PathNode>(instance);
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
