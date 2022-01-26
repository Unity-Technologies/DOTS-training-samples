
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

        var configEntity = GetSingletonEntity<Config>();
        var config = GetComponent<Config>(configEntity);
        var random = new Unity.Mathematics.Random(config.MapSeed);

        var playersQuery = GetEntityQuery(typeof(Score));
        var players = playersQuery.ToEntityArray(Allocator.TempJob);


        Entities
            .ForEach((Entity entity, in MapSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                
                // Create MapData
                var mapData = ecb.CreateEntity();
                ecb.AddComponent<MapData>(mapData);
                var mapDataBuffer = ecb.AddBuffer<TileData>(mapData);

                // warm up the random generator
                for (int i = 0; i < 1000; i++)
                    random.NextFloat();
                
                // spawn tiles and random walls
                for (int y = 0; y < config.MapHeight; ++y)
                {
                    for (int x = 0; x < config.MapWidth; ++x)
                    {
                        // Add no Tile Geometry when there is a hole
                        if (config.MapHoleFrequency > random.NextFloat())
                        {
                            mapDataBuffer.Add(new TileData
                            {
                                Walls = new Direction { Value = DirectionEnum.Hole }
                            });
                            continue;
                        }

                        var tile = ecb.Instantiate(spawner.TilePrefab);
                        ecb.SetComponent(tile, new Translation
                        {
                            Value = new float3(x, -0.5f, y)
                        });
                        var walls = (y == 0 ? DirectionEnum.North : config.MapWallFrequency > random.NextFloat()? DirectionEnum.North : DirectionEnum.None) |
                                    (y == config.MapHeight - 1 ? DirectionEnum.South : config.MapWallFrequency > random.NextFloat()? DirectionEnum.South : DirectionEnum.None) |
                                    (x == 0 ? DirectionEnum.East : config.MapWallFrequency > random.NextFloat()? DirectionEnum.East : DirectionEnum.None) |
                                    (x == config.MapWidth - 1 ? DirectionEnum.West : config.MapWallFrequency > random.NextFloat()? DirectionEnum.East : DirectionEnum.None);
                        
                        ecb.SetComponent(tile, new Tile
                        {
                            Coords = new int2(x, y)
                        });
                        ecb.SetComponent(tile, new Direction {
                            Value = walls
                        });
                        ecb.SetComponent(tile, new URPMaterialPropertyBaseColor
                        {
                            Value = (x & 1) == (y & 1)? spawner.TileEvenColor : spawner.TileOddColor
                        });

                        mapDataBuffer.Add(new TileData
                        {
                            Walls = new Direction { Value = walls }
                        });

                        if ((walls & DirectionEnum.North) == DirectionEnum.North)
                        {
                            var wall = ecb.Instantiate(spawner.WallPrefab);
                            ecb.SetComponent(wall, new Translation
                            {
                                Value = new float3(x, 0, y-0.5f)
                            });
                            ecb.SetComponent(wall, new Rotation
                            {
                                Value = quaternion.Euler(0, math.PI * 0.5f, 0)
                            });
                        }
                        
                        if ((walls & DirectionEnum.South) == DirectionEnum.South)
                        {
                            var wall = ecb.Instantiate(spawner.WallPrefab);
                            ecb.SetComponent(wall, new Translation
                            {
                                Value = new float3(x, 0, y+0.5f)
                            });
                            ecb.SetComponent(wall, new Rotation
                            {
                                Value = quaternion.Euler(0, math.PI * 0.5f, 0)
                            });
                        }

                        if ((walls & DirectionEnum.West) == DirectionEnum.West)
                        {
                            var wall = ecb.Instantiate(spawner.WallPrefab);
                            ecb.SetComponent(wall, new Translation
                            {
                                Value = new float3(x + 0.5f, 0, y)
                            });
                        }

                        if ((walls & DirectionEnum.East) == DirectionEnum.East)
                        {
                            var wall = ecb.Instantiate(spawner.WallPrefab);
                            ecb.SetComponent(wall, new Translation
                            {
                                Value = new float3(x - 0.5f, 0, y)
                            });
                        }
                    }
                }

                // spawn one exit per player
                foreach (var playerEntity in players)
                {
                    var color = GetComponent<Color>(playerEntity);
                    var exit = ecb.Instantiate(config.ExitPrefab);
                    var coords = random.NextInt2(int2.zero, new int2(config.MapWidth, config.MapHeight));
                    ecb.SetComponent(exit, new Translation
                    {
                        Value = new float3(coords.x, 0, coords.y)
                    });
                    ecb.SetComponent(exit, new Tile
                    {
                        Coords = coords
                    });
                    ecb.SetComponent(exit, new URPMaterialPropertyBaseColor
                    {
                        Value = color.Value
                    });
                    ecb.SetComponent(exit, new PlayerOwned
                    {
                        Owner = playerEntity
                    });
                }

                // spawn the cats
                for (int i = 0; i < config.CatsInMap; ++i)
                {
                    var cat = ecb.Instantiate(config.CatPrefab);
                    var coords = random.NextInt2(int2.zero, new int2(config.MapWidth, config.MapHeight));
                    ecb.SetComponent(cat, new Tile
                    {
                        Coords = coords
                    });
                    ecb.SetComponent(cat, new Direction
                    {
                        Value = random.NextInt(4) switch
                        {
                            0 => DirectionEnum.North,
                            1 => DirectionEnum.East,
                            2 => DirectionEnum.South,
                            3 => DirectionEnum.West,
                            _ => DirectionEnum.North
                        }
                    });
                }

                // spawn the mice spawners
                var timeToStartSpawning = 0f;
                for (int i = 0; i < config.MiceSpawnerInMap; ++i)
                {
                    // spread out the seeds of random generation
                    for (int j = 0; j < 1000; ++j)
                    {
                        random.NextUInt();
                    }
                    var miceSpawner = ecb.CreateEntity();
                    ecb.AddComponent(miceSpawner, new MiceSpawner
                    {
                        RandomizerState = random.state,
                        SpawnCooldown = timeToStartSpawning
                    });
                    ecb.AddComponent(miceSpawner, new Tile
                    {
                        Coords = random.NextInt2(new int2(config.MapWidth, config.MapHeight)) 
                    });
                    ecb.AddComponent(miceSpawner, new Direction
                    {
                        Value = random.NextInt(4) switch
                        {
                            0 => DirectionEnum.North,
                            1 => DirectionEnum.East,
                            2 => DirectionEnum.South,
                            3 => DirectionEnum.West,
                            _ => DirectionEnum.North
                        }
                    });
                    // make it so that each following spawner takes a longer time to start spawning
                    timeToStartSpawning +=
                        random.NextFloat(config.MouseSpawnCooldown.x, config.MouseSpawnCooldown.y);
                }
            }).Run();

        players.Dispose();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
