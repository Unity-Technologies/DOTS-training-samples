using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TileCheckSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        EntityQueryDesc desc = new EntityQueryDesc
        {
            // Query only matches chunks with both Red and Green components.
            All = new ComponentType[] {typeof(Wall), typeof(Translation)}
        };
        EntityManager.CreateEntityQuery(desc);
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var tileWallsEntity = GetSingletonEntity<TileWall>();
        var tileWalls = EntityManager.GetBuffer<TileWall>(tileWallsEntity);
        // can probably make position readonly.
        Entities
            .WithAll<TileCheckTag>()
            .WithReadOnly(tileWalls)
            .ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref Direction direction,
                ref Rotation rotation,
                in TileCoord tileCoord) =>
            {
                var tileX = tileCoord.Value.x;
                var tileY = tileCoord.Value.y;

                byte newDirection = FindNewDirectionIfNeeded(tileX, tileY, direction.Value, tileWalls);

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
    }

    static byte FindNewDirectionIfNeeded(
        int tileX,
        int tileY,
        byte direction,
        DynamicBuffer<TileWall> tileWalls)
    {
        int bufferIndex = tileY * 10 + tileX;
        TileWall wall = tileWalls[bufferIndex];

        bool directionFound = false;
        int c = 4;

        byte directionOut = direction;
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