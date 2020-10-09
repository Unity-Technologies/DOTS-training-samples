using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_HolePositionQuery;
    private EntityQuery m_tilesWithArrowQuery;
    private EntityQuery m_tileWithHomebaseQuery;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        EntityQueryDesc holeQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Hole), typeof(Translation)}
        };
        m_HolePositionQuery = EntityManager.CreateEntityQuery(holeQueryDesc);

        EntityQueryDesc arrowQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Arrow), typeof(Direction)}
        };
        m_tilesWithArrowQuery = EntityManager.CreateEntityQuery(arrowQueryDesc);

        EntityQueryDesc homebaseQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(HomeBase), typeof(Translation)}
        };
        m_tileWithHomebaseQuery = EntityManager.CreateEntityQuery(homebaseQueryDesc);
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var holeTranslations = m_HolePositionQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        var arrows = m_tilesWithArrowQuery.ToComponentDataArray<Arrow>(Allocator.TempJob);
        var arrowDirections = m_tilesWithArrowQuery.ToComponentDataArray<Direction>(Allocator.TempJob);
        var boardSize = GetSingleton<GameInfo>().boardSize.x;

        var homeBaseEntities = m_tileWithHomebaseQuery.ToEntityArray(Allocator.TempJob);
        var homeBases = m_tileWithHomebaseQuery.ToComponentDataArray<HomeBase>(Allocator.TempJob);
        var homeBaseTileTranslations = m_tileWithHomebaseQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        var tileWallsEntity = GetSingletonEntity<TileWall>();
        var tileWalls = EntityManager.GetBuffer<TileWall>(tileWallsEntity);

        Entities
            .WithNone<Falling>()
            .WithReadOnly(arrows)
            .WithReadOnly(arrowDirections)
            .WithReadOnly(tileWalls)
            .ForEach(
                (Entity entity, int entityInQueryIndex, ref Position position, ref Translation translation,
                    ref TileCoord tileCoord, ref Direction direction, ref Rotation rotation, in Speed speed) =>
                {
                    var forward = float2.zero;
                    //Convert direction to forward
                    if ((direction.Value & DirectionDefines.North) == 1)
                    {
                        forward = new float2(0, 1);
                    }
                    else if ((direction.Value & DirectionDefines.South) == 2)
                    {
                        forward = new float2(0, -1);
                    }
                    else if ((direction.Value & DirectionDefines.East) == 4)
                    {
                        forward = new float2(1, 0);
                    }
                    else if ((direction.Value & DirectionDefines.West) == 8)
                    {
                        forward = new float2(-1, 0);
                    }

                    var prevTileX = tileCoord.Value.x;
                    var prevTileY = tileCoord.Value.y;

                    //Add direction * speed * deltaTime to position
                    var deltaX = math.mul(math.mul(forward.x, speed.Value), deltaTime);
                    var deltaY = math.mul(math.mul(forward.y, speed.Value), deltaTime);
                    position.Value += new float2(deltaX, deltaY);

                    var tileCenterOffset = position.Value - tileCoord.Value;

                    bool fellIntoHole = false;
                    for (int i = 0; i < holeTranslations.Length; i++)
                    {
                        if (math.distancesq(holeTranslations[i].Value.x, position.Value.x) < 0.02 &&
                            math.distancesq(holeTranslations[i].Value.z, position.Value.y) < 0.02)
                        {
                            //Add Falling Tag
                            ecb.AddComponent<Falling>(entityInQueryIndex, entity);
                            fellIntoHole = true;
                        }
                    }

                    if (!fellIntoHole && (math.abs(tileCenterOffset.x) > 1) || math.abs(tileCenterOffset.y) > 1)
                    {
                        //We crossed to a new tile, so check for arrows, walls, homebases, etc.
                        var tileX = (int) (prevTileX + math.trunc(tileCenterOffset.x));
                        var tileY = (int) (prevTileY + math.trunc(tileCenterOffset.y));

                        tileX = math.clamp(tileX, 0, boardSize - 1);
                        tileY = math.clamp(tileY, 0, boardSize - 1);

                        position.Value.x = math.clamp(position.Value.x, 0, boardSize - 1);
                        position.Value.y = math.clamp(position.Value.y, 0, boardSize - 1);

                        tileCoord.Value = new int2(tileX, tileY);

                        int bufferIndex = tileY * boardSize + tileX;

                        bool hitHomeBase = false;
                        for (int i = 0; i < homeBaseTileTranslations.Length; i++)
                        {
                            if (tileX == (int) homeBaseTileTranslations[i].Value.x &&
                                tileY == (int) homeBaseTileTranslations[i].Value.z)
                            {
                                var homebase = homeBases[i];
                                int scoreaddition = 1;
                                if (HasComponent<Cat>(entity))
                                {
                                    scoreaddition = (int) -(homebase.playerScore * 0.33333);
                                }

                                ecb.SetComponent<HomeBase>(entityInQueryIndex, homeBaseEntities[i],
                                    new HomeBase()
                                    {
                                        playerIndex = homebase.playerIndex,
                                        playerScore = homebase.playerScore + scoreaddition
                                    });
                                hitHomeBase = true;

                                ecb.DestroyEntity(entityInQueryIndex, entity);
                            }
                        }

                        if (!hitHomeBase)
                        {
                            byte newDirection = direction.Value;
                            for (int i = 0; i < arrows.Length; i++)
                            {
                                if (arrows[i].Position == bufferIndex)
                                    newDirection = arrowDirections[i].Value;
                            }

                            //Bug in here somewhere.  the mice can get to a certain point and then continuously spin on the tile.
                            newDirection = FindNewDirectionIfNeeded(bufferIndex, newDirection, tileWalls);
                            if (newDirection != direction.Value)
                            {
                                direction.Value = newDirection;
                                var temp = quaternion.RotateY(math.radians(90f));
                                rotation.Value = math.normalize(math.mul(rotation.Value, temp));
                            }
                        }
                    }

                    translation.Value = new float3(position.Value.x, 0, position.Value.y);
                }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
        holeTranslations.Dispose(Dependency);
        arrows.Dispose(Dependency);
        arrowDirections.Dispose(Dependency);
        homeBaseEntities.Dispose(Dependency);
        homeBases.Dispose(Dependency);
        homeBaseTileTranslations.Dispose(Dependency);
    }

    static byte FindNewDirectionIfNeeded(
        int bufferIndex,
        byte direction,
        DynamicBuffer<TileWall> tileWalls)
    {
        TileWall wall = tileWalls[bufferIndex];

        byte directionOut = direction;
        bool directionFound = false;
        int c = 4;
        while (!directionFound && c > 0)
        {
            switch (directionOut)
            {
                case DirectionDefines.North:
                    if ((wall.Value & DirectionDefines.North) != 0)
                    {
                        directionOut = DirectionDefines.East;
                    }
                    else
                    {
                        directionFound = true;
                    }

                    break;
                case DirectionDefines.South:
                    if ((wall.Value & DirectionDefines.South) != 0)
                    {
                        directionOut = DirectionDefines.West;
                    }
                    else
                    {
                        directionFound = true;
                    }

                    break;
                case DirectionDefines.West:
                    if ((wall.Value & DirectionDefines.West) != 0)
                    {
                        directionOut = DirectionDefines.North;
                    }
                    else
                    {
                        directionFound = true;
                    }

                    break;
                case DirectionDefines.East:
                    if ((wall.Value & DirectionDefines.East) != 0)
                    {
                        directionOut = DirectionDefines.South;
                    }
                    else
                    {
                        directionFound = true;
                    }

                    break;
            }

            --c;
        }

        return directionOut;
    }

    protected override void OnDestroy()
    {
        m_HolePositionQuery.Dispose();
        m_tilesWithArrowQuery.Dispose();
        m_tileWithHomebaseQuery.Dispose();
    }
}