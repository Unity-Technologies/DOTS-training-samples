using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class UpdatePositionSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        // Looking up another system in the world is an expensive operation.
        // In order to not do it every frame, we store the reference in a field.
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        var deltaTime = Time.DeltaTime;
        var boardEntity = GetSingletonEntity<BoardDefinition>();
        var gridCellContents = EntityManager.GetBuffer<GridCellContent>(boardEntity);
        var boardDefinition = EntityManager.GetComponentData<BoardDefinition>(boardEntity);
        var numberColumns = boardDefinition.NumberColumns;

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities.WithNone<FallingTime>().ForEach((Entity e, ref GridPosition position, ref CellOffset offset, ref Direction dir, in Speed speed) =>
        {
            var deltaDisplacement = speed.Value * deltaTime;
            var deltaRatio = deltaDisplacement / boardDefinition.CellSize;
            var newOffset = offset.Value + deltaRatio;
            if (offset.Value < 0.5f && newOffset > 0.5f)
            {
                var cellType = gridCellContents[GridCellContent.Get1DIndexFromGridPosition(position, numberColumns)].Type;
                switch(cellType)
                {
                    case GridCellType.Goal:
                    {
                        //@TODO who is the goal play index ? Should add a component data with that information so we can update the score.
                        ecb.AddComponent<HittingGoal>(e);
                    }
                        break;
                    case GridCellType.Hole:
                        break;
                    case GridCellType.ArrowDown:
                    {
                        dir.Value = Dir.Down;
                    }
                        break;
                    case GridCellType.ArrowLeft:
                    {
                        dir.Value = Dir.Left;
                    }
                        break;
                    case GridCellType.ArrowRight:
                    {
                        dir.Value = Dir.Right;
                    }
                        break;
                    case GridCellType.ArrowUp:
                    {
                        dir.Value = Dir.Up;
                    }
                        break;
                }
            }
            else if (newOffset >= 1.0f)
            {
                switch (dir.Value)
                {
                    case Dir.Down:
                    {
                        ++position.Y;
                    }
                        break;
                    case Dir.Left:
                    {
                        --position.X;
                    }
                        break;
                    case Dir.Right:
                    {
                        ++position.X;
                    }
                        break;
                    case Dir.Up:
                    {
                        --position.Y;
                    }
                        break;
                }

                offset.Value -= 1.0f;
            }
        }).ScheduleParallel();
    }
}
