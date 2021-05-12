using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

public class AIPlayerControllerSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BoardInitializedTag>();
    }
    
    protected override void OnUpdate()
    {
        var boardEntity = GetSingletonEntity<BoardInitializedTag>();
        var boardDefinition = GetSingleton<BoardDefinition>();
        
        const float cursorSpeed = 3.0f;
        
        var firstCellPosition = EntityManager.GetComponentData<FirstCellPosition>(boardEntity);
        var timeData = this.Time;

        int numberOfRows = boardDefinition.NumberRows;
        int numberOfColumns = boardDefinition.NumberColumns;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.WithName("ComputeMovementForCursor").ForEach((Entity e, ref AITargetCell aiTargetCell, ref DynamicBuffer<ArrowReference> arrows, ref Translation translation, ref NextArrowIndex nextArrowIndex, ref RandomContainer random) =>
        {
            DynamicBuffer<GridCellContent> gridCellContents = GetBufferFromEntity<GridCellContent>()[boardEntity];
            var cellOffSet = new float3(boardDefinition.CellSize * aiTargetCell.X, 1.0f, boardDefinition.CellSize * aiTargetCell.Y);
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
                
                // Move with arrow
                {
                    var index = GridCellContent.Get1DIndexFromGridPosition(aiTargetCell.X, aiTargetCell.Y, numberOfColumns);
                    if (selectedArrow != Entity.Null)
                    {
                        var oldArrowPosition = GetComponent<GridPosition>(selectedArrow);
                        var oldGridContentValue = gridCellContents[GridCellContent.Get1DIndexFromGridPosition(oldArrowPosition.X, oldArrowPosition.Y, numberOfColumns)];
                        oldGridContentValue.Type = GridCellType.None;
                        gridCellContents[index] = oldGridContentValue;
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
                    gridCellContents[index] = gridContent;
                
                    ecb.SetComponent(selectedArrow, new GridPosition(){X = aiTargetCell.X, Y = aiTargetCell.Y});
                    ecb.SetComponent(selectedArrow, new Direction(){ Value = newArrowDirection});
                
                }
                //Compute new target 

                var newTargetX = random.Value.NextInt(0, numberOfRows);
                var newTargetY = random.Value.NextInt(0, numberOfColumns);
    
                aiTargetCell = new AITargetCell(){X = newTargetX, Y = newTargetY};
            }
            var progress = movementDirection * math.min(cursorSpeed * timeData.DeltaTime, distanceToTarget);
        
            translation.Value = translation.Value + progress;
        
        
        }).Run();
        //
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
