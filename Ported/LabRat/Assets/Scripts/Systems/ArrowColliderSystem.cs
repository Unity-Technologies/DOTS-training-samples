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
            ComponentType.ReadOnly<PlayTime>());

        if (arrowsQuery.IsEmpty) return;

        var arrows = arrowsQuery.ToEntityArray(Allocator.TempJob);
        foreach (var arrow in arrows)
        {
            Tile arrowTile = GetComponent<Tile>(arrow);
            Direction arrowDir = GetComponent<Direction>(arrow);

            Entities.WithAll<Mouse>().ForEach((Direction dir, ref Tile tile) =>
            {
                if (tile.Coords.Equals(arrowTile.Coords))
                {
                    dir.Value = arrowDir.Value;
                }
            }).Schedule();
        }
    }
}
