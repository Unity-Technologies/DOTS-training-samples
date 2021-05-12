using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class CatCollisionSystem : SystemBase
{
    EntityQuery m_CatQuery;

    protected override void OnCreate()
    {
        m_CatQuery = GetEntityQuery(typeof(CatTag), typeof(GridPosition), typeof(CellOffset), typeof(Direction));
    }

    protected override void OnUpdate()
    {
        // Grab all the Cats and put their positions in a dyanmic buffer
        // TODO: I have no idea if this will work
        var catEntities = m_CatQuery.ToEntityArray(Allocator.Temp);
        var catPositions = m_CatQuery.ToComponentDataArray<GridPosition>(Allocator.Temp);
        var catOffsets = m_CatQuery.ToComponentDataArray<CellOffset>(Allocator.Temp);
        var catDirections = m_CatQuery.ToComponentDataArray<Direction>(Allocator.Temp);

        // Foreach on all the mice and check if they collide
        Entities
            .WithNone<CatTag>()
            .ForEach((Entity entity, in GridPosition gridPosition, in CellOffset cellOffset, in Direction direction) =>
            {
                var offsetX = 0;
                var offsetY = 0;

                UpdateTransformSystem.GetOffsetDirs(ref offsetX, ref offsetY, in direction);

                //var mouseGridPosition = new float2()

                // TODO: Check the distance to each cat
                for (int i = 0; i < catPositions.Length; i++)
                {
                    //if (math.distance(new float2(catPositions[i].X, catPositions[i].Y), new float2(gridPosition))
                }
            }).Run();
    }
}
