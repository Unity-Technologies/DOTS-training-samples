using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(UpdatePositionSystem))]
public class UpdateTransformSystem : SystemBase
{
    EntityCommandBufferSystem CommandBufferSystem;

    public static void GetOffsetDirs(ref int offsetDirX, ref int offsetDirY, in Direction direction)
    {
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
    }

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BoardInitializedTag>();
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var board = GetSingletonEntity<BoardDefinition>();
        var boardDefinition = GetComponent<BoardDefinition>(board);
        var cellSize = boardDefinition.CellSize;
        var firstCellPosition = GetComponent<FirstCellPosition>(board).Value;

        // Move Mice and Cats
        Entities
            .WithName("UpdateMovingGridObjectPosition")
            .WithAll<Speed>()
            .WithNone<FallingTime>()
            .ForEach((ref Translation translation, ref Rotation rotation, in GridPosition gridPosition, in CellOffset cellOffset, in Direction direction) =>
            {
                var offsetDirX = 0;
                var offsetDirY = 0;

                GetOffsetDirs(ref offsetDirX, ref offsetDirY, in direction);

                // Fill this in with conversion math as well as adding offset
                var cellOffsetMiddle = cellOffset.Value - 0.5f;
                var xOffset = gridPosition.X + offsetDirX * cellOffsetMiddle ;
                var yOffset = gridPosition.Y - offsetDirY * cellOffsetMiddle;
                translation.Value = firstCellPosition + new float3(yOffset * cellSize, 0.5f, xOffset * cellSize);
            }).ScheduleParallel();

        Entities
            .WithName("UpdateMovableGridObjectPosition")
            .WithNone<Speed>()
            .WithAll<MovableTag>()
            .ForEach((ref Translation translation, in GridPosition gridPosition) =>
            {
                var xOffset = gridPosition.X * cellSize;
                var yOffset = gridPosition.Y * cellSize;
                translation.Value = firstCellPosition + new float3(yOffset, translation.Value.y, xOffset);
            }).ScheduleParallel();


        // Rotate all transforms with Directions
        Entities
            .WithName("UpdateMovingObjectRotation")
            .WithAll<Speed>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, in Direction direction) =>
            {
                var offsetDirX = 0;
                var offsetDirY = 0;

                GetOffsetDirs(ref offsetDirX, ref offsetDirY, in direction);

                var currentRotationValue = rotation.Value;
                // Rotate based on direction
                rotation.Value = quaternion.LookRotation(new float3(-offsetDirY, 0f, offsetDirX), new float3(0f, 1f, 0f));

                if (!currentRotationValue.Equals(rotation.Value))
                {
                    ecb.AddComponent(entityInQueryIndex,entity,new RotateAnimationProperties(){AnimationDuration = 0.05f, OriginalRotation = currentRotationValue, TargetRotation = rotation.Value});
                }

            }).ScheduleParallel();

        // Rotate all transforms with Directions
        Entities
            .WithName("UpdateMovableGridObjectRotation")
            .WithNone<Speed>()
            .WithAll<MovableTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, in Direction direction) =>
            {
                var offsetDirX = 0;
                var offsetDirY = 0;

                GetOffsetDirs(ref offsetDirX, ref offsetDirY, in direction);

                // Rotate based on direction
                rotation.Value = quaternion.LookRotation(new float3(-offsetDirY, 0f, offsetDirX), new float3(0f, 1f, 0f));
            }).ScheduleParallel();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
