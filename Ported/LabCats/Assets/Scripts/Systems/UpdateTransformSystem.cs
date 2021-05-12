using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class UpdateTransformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var board = GetSingletonEntity<BoardDefinition>();
        var boardDefinition = GetComponent<BoardDefinition>(board);
        var cellSize = boardDefinition.CellSize;
        var columns = boardDefinition.NumberColumns;
        var rows = boardDefinition.NumberRows;

        if (!HasComponent<FirstCellPosition>(board))
            return;

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
                var xOffset = gridPosition.X * cellSize + offsetDirX * (cellOffset.Value - .5f) * cellSize;
                var yOffset = gridPosition.Y * cellSize + offsetDirY * (cellOffset.Value - .5f) * cellSize;
                translation.Value = firstCellPosition.Value + new float3(xOffset, 0.5f, yOffset);
                // Rotate based on direction
                rotation.Value = quaternion.LookRotation(new float3(offsetDirX, 0f, offsetDirY), new float3(0f, 1f, 0f));
            }).ScheduleParallel();
    }
}
