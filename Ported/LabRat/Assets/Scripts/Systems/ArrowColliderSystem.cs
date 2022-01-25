using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class ArrowColliderSystem : SystemBase
{
    public ComponentDataFromEntity<Tile> tileComponents;
    protected override void OnUpdate()
    {
        EntityQuery arrowsQuery = GetEntityQuery(
            ComponentType.ReadOnly<Tile>(),
            ComponentType.ReadOnly<Direction>(),
            typeof(ArrowMiceCount));

        if (arrowsQuery.IsEmpty) return;

        var arrows = arrowsQuery.ToEntityArray(Allocator.TempJob);
        foreach (var arrow in arrows)
        {
            Tile arrowTile = GetComponent<Tile>(arrow);
            Direction arrowDir = GetComponent<Direction>(arrow);
            ArrowMiceCount arrowMiceCount = GetComponent<ArrowMiceCount>(arrow);

            Entities.WithAll<Mouse>().ForEach((Direction dir, ref Tile tile) =>
            {
                if (tile.Coords.Equals(arrowTile.Coords) && arrowMiceCount.arrowUsages > 0)
                {
                    arrowMiceCount.arrowUsages--;
                    dir.Value = arrowDir.Value;
                }
            }).Schedule();
        }
    }
}
