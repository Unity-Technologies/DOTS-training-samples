using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CannonFireSystem : SystemBase
{
    public const float kCannonBallRadius = 0.5f;
    public const float kCannonBallSpeed = 6.0f;

    EntityQuery m_BufferQuery;

    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<GameParams>();
        RequireSingletonForUpdate<GridTag>();

        m_BufferQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new []
            {
                ComponentType.ReadOnly<GridHeight>()
            }
        });
    }

    protected static int2 PositionToTile(float3 position)
    {
        return new int2((int)position.x, (int)position.z);
    }

    protected static float3 TileToPosition(int2 position, float height = 0.0f)
    {
        return new float3(position.x, height, position.y);
    }

    protected static float GetTileHeight(DynamicBuffer<GridHeight> gridHeight, int2 tilePosition, int2 terrainDimension)
    {
        return gridHeight[GridFunctions.GetGridIndex(tilePosition.xy, tilePosition)].Height;
    }

    public static bool HitsCube(float3 center, float width, float height, int2 tilePosition)
    {
        float minTileX = tilePosition.x;
        float maxTileX = tilePosition.x + 1.0f;
        float minTileY = 0.0f;
        float maxTileY = height;
        float minTileZ = tilePosition.y;
        float maxTileZ = tilePosition.y + 1.0f;

        float minBallX = center.x;
        float maxBallX = center.x + width;
        float minBallY = center.y;
        float maxBallY = center.y + width;
        float minBallZ = center.z;
        float maxBallZ = center.z + width;

        if ((maxTileX <= minBallX) || 
            (minTileX >= maxBallX) || 
            (maxTileY <= minBallY) || 
            (minTileY >= maxBallY) || 
            (maxTileZ <= minBallZ) ||
            (minTileZ >= maxBallZ))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the given cube intersects nearby boxes or tanks.
    /// </summary>
    public static  bool TerrainHitsCube(float3 center, float width, int2 terrainDimension, DynamicBuffer<GridHeight> gridHeight)
    {
        // check nearby boxes
        int colMin = (int)math.floor((center.x - width / 2));
        int colMax = (int)((center.x + width / 2));
        int rowMin = (int)math.floor((center.z - width / 2));
        int rowMax = (int)((center.z + width / 2));

        colMin = math.max(0, colMin);
        colMax = math.min(terrainDimension.x - 1, colMax);
        rowMin = math.max(0, rowMin);
        rowMax = math.min(terrainDimension.y - 1, rowMax);

        for (int c = colMin; c <= colMax; c++)
        {
            for (int r = rowMin; r <= rowMax; r++)
            {
                if (HitsCube(center, width, GetTileHeight(gridHeight, new int2(c, r), terrainDimension), new int2(c, r)))
                    return true;
            }
        }

        // TODO: check tanks

        return false;

    }

    /// <summary>
    /// Simulates firing a cannonball with the given trajectory.
    /// Returns true if the cannonball would hit a box on the way there.
    /// </summary>
    public static bool CheckTerrainCollision(float3 start, float3 end, float paraA, float paraB, float paraC, GameParams gameParam, DynamicBuffer<GridHeight> gridHeight)
    {
        float3 diff = end - start;
        float distance = math.length(new float2(diff.x, diff.z));

        int steps = math.max(2, (int)distance + 1);

        steps = (int)(steps * gameParam.collisionStepMultiplier);

        for (int i = 0; i < steps; i++)
        {
            float t = i / (steps - 1f);

            float3 pos = ParabolaMath.GetSimulatedPosition(start, end, paraA, paraB, paraC, t);

            if (TerrainHitsCube(pos, kCannonBallRadius, gameParam.TerrainDimensions, gridHeight))
            {
                return true;
            }
        }

        return false;
    }

    protected static float FindHeight(float3 canonPosition, float3 playerPosition, DynamicBuffer<GridHeight> gridHeight, GameParams gameParam)
    {
        int2 terrainDimension = gameParam.TerrainDimensions;

        // start and end positions
        float3 start = canonPosition;

        int2 playerTile = PositionToTile(playerPosition);
        Vector3 end = TileToPosition(playerTile, GetTileHeight(gridHeight, playerTile, terrainDimension) + kCannonBallRadius);
        float distance = math.length(new float2(end.z - start.z, end.x - start.x));
        float duration = distance / kCannonBallSpeed;
        if (duration < .0001f)
            duration = 1.0f;

        // binary searching to determine height of cannonball arc
        float low = math.max(start.y, end.y);
        float high = low * 2;
        float paraA, paraB, paraC;

        // look for height of arc that won't hit boxes
        while (true)
        {
            ParabolaMath.Create(start.y, high, end.y, out paraA, out paraB, out paraC);
            if (!CheckTerrainCollision(start, end, paraA, paraB, paraC, gameParam, gridHeight))
            {
                // high enough
                break;
            }
            // not high enough.  Double value
            low = high;
            high *= 2;
            // failsafe
            if (high > 9999)
            {
                return 0.0f; // skip launch
            }
        }

        // do binary searches to narrow down
        while (high - low > gameParam.playerParabolaPrecision)
        {
            float mid = (low + high) / 2;
            ParabolaMath.Create(start.y, mid, end.y, out paraA, out paraB, out paraC);
            if (CheckTerrainCollision(start, end, paraA, paraB, paraC, gameParam, gridHeight))
            {
                // not high enough
                low = mid;
            }
            else
            {
                // too high
                high = mid;
            }
        }

        return (low + high) / 2;
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        var playerEntity = GetSingletonEntity<PlayerTag>();
        var playerLocation = EntityManager.GetComponentData<Position>(playerEntity);
        var gameParams = GetSingleton<GameParams>();
        var gridEntity = GetSingletonEntity<GridTag>();
        var gridHeight = GetBufferFromEntity<GridHeight>(true)[gridEntity];

        var deltaTime = Time.DeltaTime;

        Entities
            .WithReadOnly(gridHeight)
            .ForEach((int entityInQueryIndex, Entity e, ref Rotation rotation, ref Cooldown coolDown, in Position position) =>
        {            
            // Fire
            if (coolDown.Value  < 0.0f)
            {
                var cannonPos = (int2)(position.Value.xz + 0.5f);
                var playerPos = (int2)playerLocation.Value.xz;

                var cannonHeight = gridHeight[GridFunctions.GetGridIndex(position.Value.xz, gameParams.TerrainDimensions)].Height;
                var playerHeight = gridHeight[GridFunctions.GetGridIndex(playerLocation.Value.xz, gameParams.TerrainDimensions)].Height;

                var origin = new float3(cannonPos.x, cannonHeight, cannonPos.y);
                var target = new float3(playerPos.x, playerHeight, playerPos.y);

                var height = FindHeight(position.Value, playerLocation.Value, gridHeight, gameParams);
                ParabolaMath.Create(cannonHeight, 3, playerHeight, out var a, out var b, out var c);
                var movementParabole = new MovementParabola
                {
                    Origin = origin,
                    Target = target,
                    Parabola = new float3(a, b, c),
                    Speed = kCannonBallSpeed
                };

                var instance = ecb.Instantiate(entityInQueryIndex, gameParams.CannonBallPrefab);
                ecb.AddComponent(entityInQueryIndex, instance, new Position { Value = origin});
                ecb.AddComponent(entityInQueryIndex, instance, movementParabole);
                ecb.AddComponent(entityInQueryIndex, instance, new NormalisedMoveTime { Value = 0.0f });

                var p2 = ParabolaMath.GetSimulatedPosition(origin, target, a, b, c, 0.1f);
                var dir = p2 - origin;

                rotation.Value = -math.atan(math.length(dir) / dir.y);
                coolDown.Value = gameParams.CannonCooldown;
            }
            else
            {
                coolDown.Value  -= deltaTime;
            }
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}