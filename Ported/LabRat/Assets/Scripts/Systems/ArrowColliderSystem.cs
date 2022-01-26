using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class ArrowColliderSystem : SystemBase
{
    public ComponentDataFromEntity<Tile> tileComponents;
    private EntityQuery arrowsQuery;

    protected override void OnCreate()
    {
        arrowsQuery = GetEntityQuery(typeof(Arrow),typeof(Tile),typeof(Direction));
        
        RequireForUpdate(arrowsQuery);
        RequireSingletonForUpdate<GameRunning>();
    }

    protected override void OnUpdate()
    {
        var arrowTiles = arrowsQuery.ToComponentDataArray<Tile>(Allocator.TempJob);
        var arrowDirs = arrowsQuery.ToComponentDataArray<Direction>(Allocator.TempJob);
        
        Entities
            .WithAll<Creature>()
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
