using Unity.Collections;
using Unity.Entities;

class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // placeholder grid
        var numRows = 30;
        var numColumns = 30;
        var grid = new NativeArray<CellInfo>(numRows * numColumns, Allocator.TempJob);

        // update movement
        Entities
            .ForEach((ref Position2D pos, ref Rotation2D rot, ref Direction2D dir, in WalkSpeed speed) =>
            {

            })
            .WithName("UpdateMovables")
            .ScheduleParallel();

        // clean up temporary memory
        grid.Dispose(Dependency);
    }
}