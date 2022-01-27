
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

//[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class MapSpawningSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;
    private EntityQuery playersQuery;

    protected override void OnCreate()
    {
        // this system should only perform work if MapSpawner is present without MapWasSpawned
        RequireForUpdate(
            GetEntityQuery(
                ComponentType.ReadOnly<MapSpawner>(),
                ComponentType.Exclude<MapWasSpawned>()));
        ecbSystem = World.GetExistingSystem<EntityCommandBufferSystem>();
        playersQuery = GetEntityQuery(ComponentType.ReadOnly<Score>(),ComponentType.ReadOnly<Color>());
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer();

        var config = GetSingleton<Config>();
        SetSingleton(config);
        var random = new Random(config.MapSeed);

        var players = playersQuery.ToEntityArray(Allocator.TempJob);
        var playerColors = playersQuery.ToComponentDataArray<Color>(Allocator.TempJob);

        Entities
            .WithReadOnly(playerColors)
            .WithDisposeOnCompletion(playerColors)
            .WithDisposeOnCompletion(players)
            .ForEach((Entity entity, in MapSpawner spawner) =>
            {
                ecb.AddComponent<MapWasSpawned>(entity);
                
                // warm up the random generator
                for (int i = 0; i < 1000; i++)
                    random.NextFloat();
                
                SpawnMap(config, ref random, ref ecb, spawner);

                SpawnExits(config, ref random, ref players, ref playerColors, ref ecb);

                SpawnCats(config, ref random, ref ecb);

                SpawnMiceSpawners(config, ref random, ref ecb);
                
            }).Run();
    }

    private static void SpawnMiceSpawners(in Config config, ref Random random, ref EntityCommandBuffer ecb)
    {
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
    }

    private static void SpawnCats(in Config config, ref Random random, ref EntityCommandBuffer ecb)
    {
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
            ecb.SetComponent(cat, new Scale
            {
                Value = 1.0f
            });
        }
    }

    private static void SpawnExits(in Config config, ref Random random, ref NativeArray<Entity> players, ref NativeArray<Color> playerColors, ref EntityCommandBuffer ecb)
    {
        // spawn one exit per player
        for (int i = 0; i < players.Length; i++)
        {
            var playerEntity = players[i];
            var color = playerColors[i];
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
    }

    private static void SpawnMap(in Config config, ref Random random, ref EntityCommandBuffer ecb,
        in MapSpawner spawner)
    {
        // Create MapData
        var mapData = ecb.CreateEntity();
        ecb.AddComponent<MapData>(mapData);
        var mapDataBuffer = ecb.AddBuffer<TileData>(mapData);
        mapDataBuffer.ResizeUninitialized(config.MapWidth * config.MapHeight);

        // spawn tiles and random walls
        for (int y = 0; y < config.MapHeight; ++y)
        {
            for (int x = 0; x < config.MapWidth; ++x)
            {
                // Add no Tile Geometry when there is a hole
                if (config.MapHoleFrequency > random.NextFloat())
                {
                    mapDataBuffer[x + y * config.MapWidth] = new TileData
                    {
                        Walls = new Direction { Value = DirectionEnum.Hole }
                    };
                    continue;
                }

                var tile = ecb.Instantiate(spawner.TilePrefab);
                ecb.SetComponent(tile, new Translation
                {
                    Value = new float3(x, -0.5f, y)
                });
                var walls = (y == 0 ? DirectionEnum.North :
                                config.MapWallFrequency > random.NextFloat() ? DirectionEnum.North : DirectionEnum.None) |
                            (y == config.MapHeight - 1 ? DirectionEnum.South :
                                config.MapWallFrequency > random.NextFloat() ? DirectionEnum.South : DirectionEnum.None) |
                            (x == 0 ? DirectionEnum.East :
                                config.MapWallFrequency > random.NextFloat() ? DirectionEnum.East : DirectionEnum.None) |
                            (x == config.MapWidth - 1 ? DirectionEnum.West :
                                config.MapWallFrequency > random.NextFloat() ? DirectionEnum.East : DirectionEnum.None);

                ecb.SetComponent(tile, new Tile
                {
                    Coords = new int2(x, y)
                });
                ecb.SetComponent(tile, new Direction
                {
                    Value = walls
                });
                ecb.SetComponent(tile, new URPMaterialPropertyBaseColor
                {
                    Value = (x & 1) == (y & 1) ? spawner.TileEvenColor : spawner.TileOddColor
                });

                mapDataBuffer[x + y * config.MapWidth] = new TileData
                {
                    Walls = new Direction { Value = walls }
                };

                if ((walls & DirectionEnum.North) == DirectionEnum.North)
                {
                    var wall = ecb.Instantiate(spawner.WallPrefab);
                    ecb.SetComponent(wall, new Translation
                    {
                        Value = new float3(x, -0.25f, y - 0.5f)
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
                        Value = new float3(x, -0.25f, y + 0.5f)
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
                        Value = new float3(x + 0.5f, -0.25f, y)
                    });
                }

                if ((walls & DirectionEnum.East) == DirectionEnum.East)
                {
                    var wall = ecb.Instantiate(spawner.WallPrefab);
                    ecb.SetComponent(wall, new Translation
                    {
                        Value = new float3(x - 0.5f, -0.25f, y)
                    });
                }
            }
        }
    }
}
