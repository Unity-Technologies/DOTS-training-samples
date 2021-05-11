using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class UpdateTransformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var board = GetSingletonEntity<GridCellSize>();
        var gridCellSize = GetComponent<GridCellSize>(board);
        var gridSize = GetComponent<GridSize>(board);
        var firstCellPosition = GetComponent<FirstCellPosition>(board);

        // Move Mice and Cats
        Entities
            .WithAll<Speed>()
            .ForEach((ref Translation translation, ref Rotation rotation, in GridPosition gridPosition, in CellOffset cellOffset, in Direction direction) =>
            {
                var offsetDirX = 0;
                var offsetDirY = 0;

                switch (direction.Value)
                {
                    case Dir.Up:
                        offsetDirY = 1;
                        break;
                    case Dir.Right:
                        offsetDirX = 1;
                        break;
                    case Dir.Down:
                        offsetDirY = -1;
                        break;
                    case Dir.Left:
                        offsetDirX = -1;
                        break;
                }

                // Fill this in with conversion math as well as adding offset
                translation.Value = firstCellPosition.Value + new float3(gridPosition.X * gridCellSize.X + offsetDirX * cellOffset.Value, 0, -(gridPosition.Y * gridCellSize.Y + offsetDirY * cellOffset.Value));
                // Rotate based on direction
                rotation.Value = quaternion.LookRotation(new float3(offsetDirX, 0f, offsetDirY), new float3(0f, 1f, 0f));
            }).ScheduleParallel();
    }
}
