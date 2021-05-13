using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

public class AIPlayerControllerSystem : SystemBase
{
    private EntityCommandBufferSystem m_EcbSystem;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BoardInitializedTag>();
        m_EcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var boardEntity = GetSingletonEntity<BoardInitializedTag>();
        var boardDefinition = GetSingleton<BoardDefinition>();

        const float cursorSpeed = 5.0f;

        var firstCellPosition = EntityManager.GetComponentData<FirstCellPosition>(boardEntity);
        var timeData = this.Time;

        int numberOfRows = boardDefinition.NumberRows;
        int numberOfColumns = boardDefinition.NumberColumns;

        var ecb = m_EcbSystem.CreateCommandBuffer().AsParallelWriter();
        var gridCellContents = GetBufferFromEntity<GridCellContent>(true)[boardEntity];
        Dependency = Entities
            .WithName("ComputeMovementForCursor")
            .WithReadOnly(gridCellContents)
            .ForEach((Entity e, int entityInQueryIndex, ref AITargetCell aiTargetCell, ref DynamicBuffer<ArrowReference> arrows, ref Translation translation, ref NextArrowIndex nextArrowIndex, ref RandomContainer random) =>
        {
            // if target is already occupied, pick a new target
            var currentTargetCellType = gridCellContents[GridCellContent.Get1DIndexFromGridPosition(aiTargetCell.X, aiTargetCell.Y, boardDefinition.NumberColumns)].Type;
            if (currentTargetCellType != GridCellType.None)
            {
                var newTargetX = random.Value.NextInt(0, numberOfColumns);
                var newTargetY = random.Value.NextInt(0, numberOfRows);

                aiTargetCell = new AITargetCell(){X = newTargetX, Y = newTargetY};
            }

            var cellOffSet = new float3(boardDefinition.CellSize * aiTargetCell.Y, 1.0f, boardDefinition.CellSize * aiTargetCell.X);
            float3 targetCellPosition = firstCellPosition.Value + cellOffSet;

            var distanceVector = targetCellPosition - translation.Value;
            if (math.length(distanceVector) < 0.001)
            {
                Debug.Log("Problem");
            }
            var movementDirection = math.normalize(distanceVector);
            var squareDistance = math.distancesq(translation.Value, targetCellPosition);
            var distanceToTarget = math.sqrt(squareDistance);

            if (cursorSpeed * timeData.DeltaTime > distanceToTarget)
            {
                // the cursor has reached its target point, we need to change the cell to have an arrow and setup a new targetCell
                Entity selectedArrow = arrows[nextArrowIndex.Value].Value;
                //don't forget to cycle to the next arrow target to have 3 at the same time
                ++nextArrowIndex.Value;
                if (nextArrowIndex.Value == 3)
                    nextArrowIndex.Value = 0;
                // Move with arrow
                {
                    var newBuffer = ecb.SetBuffer<GridCellContent>(entityInQueryIndex, boardEntity);
                    newBuffer.CopyFrom(gridCellContents);
                    var index = GridCellContent.Get1DIndexFromGridPosition(aiTargetCell.X, aiTargetCell.Y, numberOfColumns);
                    if (selectedArrow != Entity.Null)
                    {
                        var oldArrowPosition = GetComponent<GridPosition>(selectedArrow);
                        var oldGridContentIndex = GridCellContent.Get1DIndexFromGridPosition(oldArrowPosition.X, oldArrowPosition.Y, numberOfColumns);
                        var oldGridContentValue = gridCellContents[oldGridContentIndex];
                        oldGridContentValue.Type = GridCellType.None;
                        newBuffer[oldGridContentIndex] = oldGridContentValue;
                    }

                    var gridContent = gridCellContents[index];

                    var newArrowDirectionAsInt = random.Value.NextInt(0, 4);
                    var newArrowDirection = Dir.Left;
                    var newType = GridCellType.ArrowLeft;
                    if (newArrowDirectionAsInt == 0)
                    {
                        newArrowDirection = Dir.Left;
                        newType = GridCellType.ArrowLeft;
                    }

                    else if (newArrowDirectionAsInt == 1)
                    {
                        newArrowDirection = Dir.Right;
                        newType = GridCellType.ArrowRight;
                    }
                    else if (newArrowDirectionAsInt == 2)
                    {
                        newArrowDirection = Dir.Up;
                        newType = GridCellType.ArrowUp;
                    }
                    else
                    {
                        newArrowDirection = Dir.Down;
                        newType = GridCellType.ArrowDown;
                    }

                    gridContent.Type = newType;
                    newBuffer[index] = gridContent;
                    ecb.SetComponent(entityInQueryIndex, selectedArrow, new GridPosition(){X = aiTargetCell.X, Y = aiTargetCell.Y});
                    ecb.SetComponent(entityInQueryIndex, selectedArrow, new Direction(){ Value = newArrowDirection});

                }
                //Compute new target

                var newTargetX = random.Value.NextInt(0, numberOfColumns);
                var newTargetY = random.Value.NextInt(0, numberOfRows);

                aiTargetCell = new AITargetCell(){X = newTargetX, Y = newTargetY};
            }
            var progress = movementDirection * math.min(cursorSpeed * timeData.DeltaTime, distanceToTarget);

            translation.Value = translation.Value + progress;
        }).ScheduleParallel(Dependency);

        m_EcbSystem.AddJobHandleForProducer(Dependency);
    }
}
