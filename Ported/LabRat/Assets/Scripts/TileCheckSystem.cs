using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class TileCheckSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_tilesWithArrowQuery;
    private EntityQuery m_tileWithHomebaseQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        EntityQueryDesc arrowQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Arrow), typeof(Direction)}
        };
        m_tilesWithArrowQuery = EntityManager.CreateEntityQuery(arrowQueryDesc);

        EntityQueryDesc desc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(HomeBase), typeof(Translation) }
        };
        m_tileWithHomebaseQuery = EntityManager.CreateEntityQuery(desc);
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var arrows = m_tilesWithArrowQuery.ToComponentDataArray<Arrow>(Allocator.TempJob);
        var arrowDirections = m_tilesWithArrowQuery.ToComponentDataArray<Direction>(Allocator.TempJob);
        var boardSize = GetSingleton<GameInfo>().boardSize.x;

        var homeBaseEntities = m_tileWithHomebaseQuery.ToEntityArray(Allocator.TempJob);
        var homeBases = m_tileWithHomebaseQuery.ToComponentDataArray<HomeBase>(Allocator.TempJob);
        var homeBaseTileTranslations = m_tileWithHomebaseQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        var tileWallsEntity = GetSingletonEntity<TileWall>();
        var tileWalls = EntityManager.GetBuffer<TileWall>(tileWallsEntity);

        // can probably make position readonly.
        Entities
            .WithAll<TileCheckTag>()
            .WithReadOnly(arrows)
            .WithReadOnly(arrowDirections)
            .WithReadOnly(tileWalls)
            .ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref Direction direction,
                ref Rotation rotation,
                ref TileCoord tileCoord) =>
            {
                var tileX = tileCoord.Value.x;
                var tileY = tileCoord.Value.y;
                int bufferIndex = tileY * boardSize + tileX;

                bool hitHomeBase = false;
                for (int i = 0; i < homeBaseTileTranslations.Length; i++)
                {
                    if (tileX == (int)homeBaseTileTranslations[i].Value.x &&
                        tileY == (int)homeBaseTileTranslations[i].Value.z)
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
                    ecb.RemoveComponent<TileCheckTag>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
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
}