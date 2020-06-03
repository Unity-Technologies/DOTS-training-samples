using Unity.Collections;
using Unity.Entities;

class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var gridSystem = World.GetOrCreateSystem<GridCreationSystem>();
        if (!gridSystem.Cells.IsCreated)
            return;

        var cells = gridSystem.Cells;
        var rows = ConstantData.Instance.BoardDimensions.x;
        var cols = ConstantData.Instance.BoardDimensions.y;

        // update movement
        Entities
            .ForEach((ref Position2D pos, ref Rotation2D rot, ref Direction2D dir, in WalkSpeed speed) =>
            {

            })
            .WithName("UpdateMovables")
            .ScheduleParallel();
    }
}