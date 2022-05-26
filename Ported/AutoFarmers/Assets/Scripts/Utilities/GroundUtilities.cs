using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

static class GroundUtilities
{
    public static void GenerateGroundAndRocks(EntityManager em, in GameConfig config, in Allocator allocator)
    {
        int2 mapSize = config.MapSize;

        NativeArray<Entity> groundTileEntities = CollectionHelper.CreateNativeArray<Entity>(
            mapSize.x * mapSize.y, allocator);
        em.Instantiate(config.GroundTileNormalPrefab, groundTileEntities);

        // Create Ground
        Entity groundEntity = em.CreateEntity();
        em.AddComponent<Ground>(groundEntity);
        DynamicBuffer<GroundTile> groundData = em.AddBuffer<GroundTile>(groundEntity);
        groundData.Length = mapSize.x * mapSize.y;

        Random randomGenerator = new Random((uint)config.WorldGenerationSeed);
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                int index = MapUtil.MapCordToIndex(mapSize, x, y);

                if (randomGenerator.NextFloat(0f, 1f) < 0.9f)
                {
                    groundData[index] = new GroundTile
                    {
                        tileState = GroundTileState.Open,
                        rockEntityByTile = Entity.Null,
                        siloEntityByTile = Entity.Null,
                        plantEntityByTile = Entity.Null
                    };
                }
                else if(randomGenerator.NextFloat(0f, 1f) < 0.05f)
                {
                    var silo = em.Instantiate(config.SiloPrefab);
                    em.SetComponentData<Translation>(silo, new Translation
                    {
                        Value = new float3(x, .2f, y),
                    });
                    groundData[index] = new GroundTile
                    {
                        tileState = GroundTileState.Silo,
                        rockEntityByTile = Entity.Null,
                        plantEntityByTile = Entity.Null,
                        siloEntityByTile = silo,
                    };
                }

                em.SetComponentData(groundTileEntities[index], new GroundTileView
                {
                    Index = index
                });
                em.SetComponentData(groundTileEntities[index], new Translation
                {
                    Value = new float3(x, 0, y)
                });
            }
        }

        for (int i = 0; i < config.InitialRockAttempts; ++i)
        {
            TryGenerateRock(em, config, groundEntity, ref randomGenerator);
        }
    }

    public static int GetTileIndex(in int2 xy, in int groundWidth)
    {
        return xy.y * groundWidth + xy.x;
    }

    public static int2 GetTileCoords(in int index, in int groundWidth)
    {
        int y = index / groundWidth;
        int x = index % groundWidth;
        return new int2(x, y);
    }

    public static bool TryGetTileCoords(in float3 position, in int groundWidth, in int groundHeight, out int2 result)
    {
        Translation translation = new Translation() {
            Value = new float3(position.x, 0, position.y)
        };
        return TryGetTileCoords(translation, groundWidth, groundHeight, out result);
    }

    public static bool TryGetTileCoords(in Translation translation, in int groundWidth, in int groundHeight, out int2 result)
    {
        float x = translation.Value.x;
        float y = translation.Value.z;

        // Assuming tiles are size 1x1 units

        int gridX = (int)math.round(x);
        int gridY = (int)math.round(y);

        if (gridX > groundWidth || gridX < 0 || gridY > groundHeight || gridY < 0)
        {
            result = new int2();
            return false;
        }
        result = new int2(gridX, gridY);
        return true;
    }


    public static float2 GetTileTranslation(in int tileIndex, in int groundWidth)
    {
        int2 coordinates = GetTileCoords(tileIndex, groundWidth);

        // Assuming tiles are sized 1x1
        return new float2(coordinates.x, coordinates.y);
    }

    #region Rock Lifecycle Helpers
    public static bool TryGenerateRock(
        EntityManager em,
        in GameConfig config,
        in Entity groundEntity,
        ref Random randomGenerator)
    {
        var groundData = em.GetBuffer<GroundTile>(groundEntity);

        float2 size = new float2(
            randomGenerator.NextFloat(config.MinRockSize, config.MaxRockSize),
            randomGenerator.NextFloat(config.MinRockSize, config.MaxRockSize));

        float2 topLeft = new float2(
            randomGenerator.NextFloat(0, config.MapSize.x - size.x),
            randomGenerator.NextFloat(0, config.MapSize.y - size.y));

        int2 minTile = (int2)math.floor(topLeft);
        int2 maxTile = (int2)math.floor(topLeft + size);

        if (!AreAllTilesInRangeOpen(groundData, minTile, maxTile, config.MapSize.x))
        {
            return false;
        }

        float depth = randomGenerator.NextFloat(config.MinRockDepth, config.MaxRockDepth);

        float3 rockSize = new float3(size.x, depth, size.y);
        float3 rockCenter = new float3(topLeft.x + size.x / 2 - 0.5f, depth / 2, topLeft.y + size.y / 2 - 0.5f);

        float health = (size.x) * (size.y) * config.RockHealthPerUnitArea;

        Entity rockEntity = em.Instantiate(config.RockPrefab);
        em.SetComponentData(rockEntity, new Translation
        {
            Value = rockCenter
        });
        em.AddComponentData(rockEntity, new NonUniformScale
        {
            Value = rockSize
        });
        em.SetComponentData(rockEntity, new Rock
        {
            size = rockSize,
            initialHealth = health
        });
        em.SetComponentData(rockEntity, new RockHealth
        {
            Value = health,
        });

        groundData = em.GetBuffer<GroundTile>(groundEntity);
        SetAllTilesInRangeTo(GroundTileState.Unpassable, rockEntity, ref groundData, minTile, maxTile, config.MapSize.x);

        return true;
    }

    public static void DestroyRock(
        in Entity rockEntity,
        in EntityManager entityManager,
        EntityCommandBuffer ecb,
        in GameConfig config,
        ref DynamicBuffer<GroundTile> groundData)
    {
        Translation rockTranslation = entityManager.GetComponentData<Translation>(rockEntity);
        NonUniformScale rockScale = entityManager.GetComponentData<NonUniformScale>(rockEntity);

        float2 offset = new float2(0.5f, 0.5f);
        int2 minTile = math.clamp((int2)math.floor(rockTranslation.Value.xz - rockScale.Value.xz / 2 + offset), int2.zero, config.MapSize);
        int2 maxTile = math.clamp((int2)math.floor(rockTranslation.Value.xz + rockScale.Value.xz / 2 + offset), int2.zero, config.MapSize);

        SetAllTilesInRangeTo(GroundTileState.Open, Entity.Null, ref groundData, minTile, maxTile, config.MapSize.x);

        ecb.DestroyEntity(rockEntity);
    }
    #endregion

    #region Tile Access Helpers
    public static bool IsTileTilled(GroundTileState state)
    {
        return state == GroundTileState.Tilled || state == GroundTileState.Planted;
    }
    public static bool IsTilePassable(GroundTileState state)
    {
        return state != GroundTileState.Unpassable;
    }


    public static bool AreAllTilesInRangeOpen(
        in DynamicBuffer<GroundTile> groundData,
        in int2 minTile, in int2 maxTile, in int mapWidth)
    {
        for (int y = minTile.y; y <= maxTile.y; ++y)
        {
            for (int x = minTile.x; x <= maxTile.x; ++x)
            {
                int index = y * mapWidth + x;
                if (groundData[index].tileState != GroundTileState.Open)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static void SetAllTilesInRangeTo(GroundTileState state,
        Entity rockEntity, // NOTE: Sent Entity.null if you are not placing a rock
        ref DynamicBuffer<GroundTile> groundData,
        in int2 minTile, in int2 maxTile, in int mapWidth)
    {
        for (int y = minTile.y; y <= maxTile.y; ++y)
        {
            for (int x = minTile.x; x <= maxTile.x; ++x)
            {
                int index = y * mapWidth + x;
                groundData[index] = new GroundTile
                {
                    tileState = state,
                    rockEntityByTile = rockEntity,
                    plantEntityByTile = Entity.Null
                };
            }
        }
    }

    public static UnityEngine.RectInt GetFullMapBounds(in GameConfig gameConfig)
    {
        return new UnityEngine.RectInt(0, 0, gameConfig.MapSize.x, gameConfig.MapSize.y);
    }
    #endregion
}
