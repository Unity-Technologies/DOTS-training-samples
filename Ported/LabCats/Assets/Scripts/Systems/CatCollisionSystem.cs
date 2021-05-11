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
                // TODO: Check the distance to each cat
            }).Run();
    }
}
