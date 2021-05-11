using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;

public class AIPlayerControllerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var boardEntity = GetSingletonEntity<BoardDefinition>();
        
        const float cursorSpeed = 1.0f;
        
        DynamicBuffer<GridCellContent> gridCellContents = EntityManager.GetBuffer<GridCellContent>(boardEntity);
        var gridWorldPosition = EntityManager.GetComponentData<Translation>(boardEntity);
        var timeData = this.Time;
        
        var random = new Random(1234);
        
        const int numberOfRows = 10; //Get actual values
        const int numberOfColumns = 12;

        var ecb = new EntityCommandBuffer();
        Dependency = Entities.WithName("ComputeMovementForCursor").ForEach((Entity e, ref AITargetCell aiTargetCell, ref ArrowReference arrows, ref Translation translation) =>
        {
            var targetCellPosition = new float3(1.0f, 2.0f, 3.0f); // obtain first cell pos + calculate targetPosition
            
            var distanceVector = targetCellPosition - translation.Value;
            var movementDirection = math.normalize(targetCellPosition);
            var squareDistance = distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y + distanceVector.z * distanceVector.z;
            var distance = math.sqrt(squareDistance);

            if (cursorSpeed * timeData.DeltaTime > distance)
            {
                // the cursor has reached its target point, we need to change the cell to have an arrow and setup a new targetCell
                int selectedArrowIndex;
                Entity selectedArrow;
                if (arrows.Entity1 == Entity.Null)
                {
                    selectedArrowIndex = 0;
                    selectedArrow = arrows.Entity1;
                }
                else if (arrows.Entity2 == Entity.Null)
                {
                    selectedArrowIndex = 1;
                    selectedArrow = arrows.Entity2;
                }
                else if (arrows.Entity3 == Entity.Null)
                {
                    selectedArrowIndex = 2;
                    selectedArrow = arrows.Entity3;
                }
                else
                {
                    selectedArrowIndex = 0; //This does not make sense
                    selectedArrow = arrows.Entity1;
                }
                    

                // Move with arrow
                {
                    if (selectedArrow != Entity.Null)
                    {
                        var oldArrowPosition = GetComponent<GridPosition>(selectedArrow);
                        var oldGridContentValue = gridCellContents[oldArrowPosition.Y * numberOfRows + oldArrowPosition.X];
                        oldGridContentValue.Type = GridCellType.None;
                        gridCellContents[aiTargetCell.Y * numberOfRows + aiTargetCell.X] = oldGridContentValue;
                    }

                    var gridContent = gridCellContents[aiTargetCell.Y * numberOfRows + aiTargetCell.X];
                    gridContent.Type = GridCellType.ArrowLeft; //Why left, I don’t know
                    gridCellContents[aiTargetCell.Y * numberOfRows + aiTargetCell.X] = gridContent;
                    
                    ecb.SetComponent<GridPosition>(selectedArrow, new GridPosition(){X = aiTargetCell.X, Y = aiTargetCell.Y});
                    
                }
                //Compute new target
                {

                    var newTargetX = random.NextInt() % numberOfRows;
                    var newTargetY = random.NextInt() % numberOfColumns;

                    aiTargetCell = new AITargetCell(){X = newTargetX, Y = newTargetY};
                }

            }
            var progress = movementDirection * math.min(cursorSpeed*timeData.DeltaTime, distance);
            
            translation.Value = translation.Value + progress;
            
            

        }).ScheduleParallel(Dependency);
    }
}
