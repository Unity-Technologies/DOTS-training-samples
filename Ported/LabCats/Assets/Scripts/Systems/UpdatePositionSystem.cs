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
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameStartedTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var deltaTime = Time.DeltaTime;
        var boardEntity = GetSingletonEntity<BoardDefinition>();

        var boardDefinition = EntityManager.GetComponentData<BoardDefinition>(boardEntity);

        var numberColumns = boardDefinition.NumberColumns;
        var gridCellContents = GetBufferFromEntity<GridCellContent>(true)[boardEntity];
        var goalReferences = GetBufferFromEntity<GoalReference>(true)[boardEntity];

        Dependency = Entities.WithReadOnly(gridCellContents)
                            .WithReadOnly(goalReferences)
                            .WithNone<FallingTime>()
                            .ForEach((Entity e, int entityInQueryIndex, ref GridPosition position, ref CellOffset offset, ref Direction dir, in Speed speed) =>
        {
            var deltaDisplacement = speed.Value * deltaTime;
            var deltaRatio = deltaDisplacement / boardDefinition.CellSize;
            var newOffset = offset.Value + deltaRatio;

            if (newOffset >= 0.5f)
            {
                int cell1DIndex = GridCellContent.Get1DIndexFromGridPosition(position, numberColumns);
                var cellType = gridCellContents[cell1DIndex].Type;
                var wallBoundaries = gridCellContents[cell1DIndex].Walls;
                if ((wallBoundaries & WallBoundaries.WallAll) == WallBoundaries.WallAll)
                {
                    offset.Value = 0.5f;
                    return;
                }
                switch(cellType)
                {
                    case GridCellType.Goal:
                    {
                        int playerIndex = 0;
                        if (cell1DIndex == GridCellContent.Get1DIndexFromGridPosition(boardDefinition.GoalPlayer2, numberColumns))
                        {
                            playerIndex = 1;
                        }
                        if (cell1DIndex == GridCellContent.Get1DIndexFromGridPosition(boardDefinition.GoalPlayer3, numberColumns))
                        {
                            playerIndex = 2;
                        }
                        if (cell1DIndex == GridCellContent.Get1DIndexFromGridPosition(boardDefinition.GoalPlayer4, numberColumns))
                        {
                            playerIndex = 3;
                        }

                        ecb.AddComponent(entityInQueryIndex,goalReferences[playerIndex].Goal,
                            new BounceScaleAnimationProperties(){ AccumulatedTime = 0.0f, AnimationDuration = 0.5f, OriginalScale = 1.0f, TargetScale = 1.4f});
                        ecb.AddComponent(entityInQueryIndex, e, new HittingGoal{PlayerIndex = playerIndex});
                    }
                        break;
                    case GridCellType.Hole:
                    {
                        ecb.AddComponent<FallingTime>(entityInQueryIndex, e);
                    }
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

                switch (dir.Value)
                {
                    case Dir.Down:
                    {
                        if ((wallBoundaries & WallBoundaries.WallDown) == 0)
                            break;
                        if ((wallBoundaries & WallBoundaries.WallLeft) == 0)
                            dir.Value = Dir.Left;
                        else if ((wallBoundaries & WallBoundaries.WallRight) == 0)
                            dir.Value = Dir.Right;
                        else
                            dir.Value = Dir.Up;
                    }
                        break;
                    case Dir.Left:
                    {
                        if ((wallBoundaries & WallBoundaries.WallLeft) == 0)
                            break;
                        if ((wallBoundaries & WallBoundaries.WallUp) == 0)
                            dir.Value = Dir.Up;
                        else if ((wallBoundaries & WallBoundaries.WallDown) == 0)
                            dir.Value = Dir.Down;
                        else
                            dir.Value = Dir.Right;
                    }
                        break;
                    case Dir.Right:
                    {
                        if ((wallBoundaries & WallBoundaries.WallRight) == 0)
                            break;
                        if ((wallBoundaries & WallBoundaries.WallDown) == 0)
                            dir.Value = Dir.Down;
                        else if ((wallBoundaries & WallBoundaries.WallUp) == 0)
                            dir.Value = Dir.Up;
                        else
                            dir.Value = Dir.Left;
                    }
                        break;
                    case Dir.Up:
                    {
                        if ((wallBoundaries & WallBoundaries.WallUp) == 0)
                            break;
                        if ((wallBoundaries & WallBoundaries.WallRight) == 0)
                            dir.Value = Dir.Right;
                        else if ((wallBoundaries & WallBoundaries.WallLeft) == 0)
                            dir.Value = Dir.Left;
                        else
                            dir.Value = Dir.Down;
                    }
                        break;
                }
            }
            if (newOffset >= 1.0f)
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

                newOffset -= 1.0f;
            }
            offset.Value = newOffset;

        }).ScheduleParallel(Dependency);
        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
