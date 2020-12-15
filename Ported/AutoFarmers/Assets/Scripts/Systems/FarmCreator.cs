using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine;

public class FarmCreator : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Unity.Mathematics.Random(1234);
        Entities
            .ForEach((Entity entity, in TileSpawner spawner) =>
            {

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
                        
                        var translation = new Translation { Value = new float3(i * spawner.TileSize.x, 0, j * spawner.TileSize.y) };
                        var size = new Size { Value = spawner.TileSize };

                        ecb.SetComponent(instance, translation);
                        ecb.AddComponent(instance, size);

                        var linearIndex = i + j * spawner.GridSize.x;
                        tiles[linearIndex] = instance;
                    }
                }

                int spawnedStores = 0;
                NativeArray<bool> stores = new NativeArray<bool>(gridLinearSize, Allocator.Temp);
                while (spawnedStores < spawner.StoreCount)
                {
                    int x = random.NextInt(0, spawner.GridSize.x);
                    int y = random.NextInt(0, spawner.GridSize.y);
                    var storePosition = new Vector2Int(x, y);

                    if (stores[storePosition.x + storePosition.y * spawner.GridSize.x])
                        continue;

                    var instance = ecb.Instantiate(spawner.SiloPrefab);
                    var translation = new Translation { Value = new float3((x + 0.5f) * spawner.TileSize.x, 0, (y + 0.5f) * spawner.TileSize.y) };
                    ecb.SetComponent(instance, translation);

                    var linearIndex = storePosition.x + storePosition.y * spawner.GridSize.x;
                    stores[linearIndex] = true;

                    var store = new Store { };
                    ecb.AddComponent(tiles[linearIndex], store);
                    spawnedStores++;
                }


                int spawnedCount = 0;
                NativeArray<RectInt> spawnedRocks = new NativeArray<RectInt>(spawner.Attempts, Allocator.Temp);

                for (int i = 0; i < spawner.Attempts; i++)
                {
                    var width = random.NextInt(1, 4);
                    var height = random.NextInt(1, 4);
                    var rockX = random.NextInt(0, spawner.GridSize.x - width);
                    var rockY = random.NextInt(0, spawner.GridSize.y - height);
                    var rect = new RectInt(rockX, rockY, width, height);

                    bool blocked = false;
                    for (int j = 0; j < spawnedCount; j++)
                    {
                        blocked |= spawnedRocks[j].Overlaps(rect);
                    }

                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            blocked |= stores[(rockX + j) + (rockY + k) * spawner.GridSize.x];
                        }
                    }

                    if (blocked)
                        continue;

                    var instance = ecb.Instantiate(spawner.RockPrefab);
                    var position = new RectPosition { Value = new int2(rockX, rockY) };
                    var size = new RectSize { Value = new int2(width, height) };
                    var translation = new Translation { Value = new float3(rockX * spawner.TileSize.x + width * 0.5f * spawner.TileSize.x, 0, rockY * spawner.TileSize.y + height * 0.5f * spawner.TileSize.y) };
                    var scale = new NonUniformScale { Value = new float3(spawner.TileSize.x * (width - 0.5f), 1, spawner.TileSize.y * (height - 0.5f)) };

                    ecb.SetComponent(instance, translation);
                    ecb.AddComponent(instance, position);
                    ecb.AddComponent(instance, scale);
                    ecb.AddComponent(instance, size);
                    spawnedRocks[spawnedCount] = rect;
                    spawnedCount++;

                    for (int j = rockX; j < rockX + width; j++)
                    {
                        for (int k = rockY; k < rockY + height; k++)
                        {
                            var linearIndex = j + k * spawner.GridSize.x;
                            var rock = new Rock { };
                            ecb.AddComponent(tiles[linearIndex], rock);
                        }
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
