using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class ArrowColliderSystem : SystemBase
{
    public ComponentDataFromEntity<Tile> tileComponents;
    protected override void OnUpdate()
    {
        EntityQuery arrowsQuery = GetEntityQuery(typeof(Arrow),typeof(Tile),typeof(Direction));

        if (arrowsQuery.IsEmpty) return;

        var arrowTiles = arrowsQuery.ToComponentDataArray<Tile>(Allocator.TempJob);
        var arrowDirs = arrowsQuery.ToComponentDataArray<Direction>(Allocator.TempJob);
        
        Entities
            .WithAll<Mouse>()
            .WithReadOnly(arrowTiles)
            .WithReadOnly(arrowDirs)
            .WithDisposeOnCompletion(arrowTiles)
            .WithDisposeOnCompletion(arrowDirs)
            .ForEach((ref Direction dir, in Tile tile) =>
            {
                for (int i = 0; i < arrowTiles.Length; i++)
                {
                    if (tile.Coords.Equals(arrowTiles[i].Coords))
                    {
                        dir.Value = arrowDirs[i].Value;
                    }
                }
            }).ScheduleParallel();
    }
}
