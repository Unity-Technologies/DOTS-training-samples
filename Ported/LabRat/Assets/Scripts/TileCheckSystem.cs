using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class TileCheckSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_tilesWithArrowQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        EntityQueryDesc arrowQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Arrow), typeof(Direction)}
        };
        m_tilesWithArrowQuery = EntityManager.CreateEntityQuery(arrowQueryDesc);
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var arrows = m_tilesWithArrowQuery.ToComponentDataArray<Arrow>(Allocator.TempJob);
        var arrowDirections = m_tilesWithArrowQuery.ToComponentDataArray<Direction>(Allocator.TempJob);
        var boardSize = GetSingleton<GameInfo>().boardSize.x;

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

                //if we dont hit a wall, we still want to remove the tag regardless.
                ecb.RemoveComponent<TileCheckTag>(entityInQueryIndex, entity);
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
        arrows.Dispose(Dependency);
        arrowDirections.Dispose(Dependency);
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