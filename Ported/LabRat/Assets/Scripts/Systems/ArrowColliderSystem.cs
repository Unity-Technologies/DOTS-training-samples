using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class ArrowColliderSystem : SystemBase
{
    public ComponentDataFromEntity<TileComponent> tileComponents;
    protected override void OnUpdate()
    {
        EntityQuery arrowsQuery = GetEntityQuery(
            ComponentType.ReadOnly<TileComponent>(),
            ComponentType.ReadOnly<DirectionComponent>(),
            typeof(ArrowMiceCountComponent));

        if (arrowsQuery.IsEmpty) return;

        var arrows = arrowsQuery.ToEntityArray(Allocator.TempJob);
        Entities.WithAll<MouseTag>().ForEach((TileComponent tile, ref DirectionComponent dir) =>
        {
            foreach(var arrow in arrows)
            {
                TileComponent arrowTile = GetComponent<TileComponent>(arrow);
                DirectionComponent arrowDir = GetComponent<DirectionComponent>(arrow);
                ArrowMiceCountComponent arrowMiceCount = GetComponent<ArrowMiceCountComponent>(arrow);
                if (tile.coord.Equals(arrowTile.coord))
                {
                    dir.dir = arrowDir.dir;
                    arrowMiceCount.arrowUsages--;
                }
            }
        }).Schedule();
    }
}
