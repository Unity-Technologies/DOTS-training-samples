using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class MouseInteractionSystem : SystemBase
{
    private EntityCommandBufferSystem m_EcbSystem;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameStartedTag>();
        RequireSingletonForUpdate<GameObjectRefs>();
        m_EcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var gameObjectRefs = this.GetSingleton<GameObjectRefs>();
        var playerCursor = gameObjectRefs.Player1Cursor;
        var camera = gameObjectRefs.Camera;

        var transform = playerCursor.GetComponent<RectTransform>();
        var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        transform.position = mousePosition;

        var ray = camera.ScreenPointToRay(mousePosition);

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f);

        var hitCoords = new float2(-1f, -1f);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitCoords = new float2(hit.textureCoord);

            var boardDefinition = GetSingleton<BoardDefinition>();

            var boardEntity = GetSingletonEntity<BoardInitializedTag>();
            var gridCellContents = GetBufferFromEntity<GridCellContent>(true)[boardEntity];

            var playerClicked = Input.GetMouseButtonDown(0);

            var ecb = m_EcbSystem.CreateCommandBuffer().AsParallelWriter();

            Entities
                .WithoutBurst()
                .WithNone<AITargetCell>()
                .ForEach((Entity e, int entityInQueryIndex, ref DynamicBuffer<ArrowReference> arrows, ref Translation translation, ref NextArrowIndex nextArrowIndex, in PlayerIndex playerIndex) =>
                {
                    hitCoords = hitCoords.yx;
                    hitCoords.x = boardDefinition.NumberColumns * hitCoords.x;
                    hitCoords.y = boardDefinition.NumberRows * hitCoords.y;

                    var gridPosition = (int2)hitCoords;

                    var arrowDirection = Dir.Down;
                    var newType = GridCellType.ArrowDown;

                    var gridOffset = hitCoords - gridPosition - new float2(0.5f, 0.5f);
                    var absOffset = math.abs(gridOffset);
                    var signOffset = math.sign(gridOffset);

                    if (absOffset.x > absOffset.y)
                    {
                        if (signOffset.x > 0)
                        {
                            arrowDirection = Dir.Right;
                            newType = GridCellType.ArrowRight;
                        }
                        else
                        {
                            arrowDirection = Dir.Left;
                            newType = GridCellType.ArrowLeft;
                        }
                    }
                    else
                    {
                        if (signOffset.y > 0)
                        {
                            arrowDirection = Dir.Down;
                            newType = GridCellType.ArrowDown;
                        }
                        else
                        {
                            arrowDirection = Dir.Up;
                            newType = GridCellType.ArrowUp;
                        }
                    }    

                    var currentCellIndex = GridCellContent.Get1DIndexFromGridPosition(gridPosition.x, gridPosition.y, boardDefinition.NumberColumns);
                    var currentGridCellContent = gridCellContents[currentCellIndex];

                    //Debug.Log("Hit: " + hitCoords + "Grid position: " + gridPosition + " Grid Offset:" + gridOffset + " Abs Off: " + absOffset + " Sign Off: " + signOffset + " Type: " + currentGridCellContent.Type);

                    if (playerClicked && currentGridCellContent.Type == GridCellType.None)
                    {
                        var arrow = arrows[nextArrowIndex.Value].Value;
                        nextArrowIndex.Value++;
                        if (nextArrowIndex.Value == 3)
                        {
                            nextArrowIndex.Value = 0;
                        }

                        var newBuffer = ecb.SetBuffer<GridCellContent>(entityInQueryIndex, boardEntity);
                        newBuffer.CopyFrom(gridCellContents);
                        if (arrow != Entity.Null)
                        {
                            var oldArrowPosition = GetComponent<GridPosition>(arrow);
                            var oldGridContentIndex = GridCellContent.Get1DIndexFromGridPosition(oldArrowPosition.X, oldArrowPosition.Y, boardDefinition.NumberColumns);
                            var oldGridContentValue = gridCellContents[oldGridContentIndex];
                            oldGridContentValue.Type = GridCellType.None;
                            newBuffer[oldGridContentIndex] = oldGridContentValue;
                        }
                        
                        currentGridCellContent.Type = newType;
                        newBuffer[currentCellIndex] = currentGridCellContent;
                        ecb.SetComponent(entityInQueryIndex, arrow, new GridPosition() { X = gridPosition.x, Y = gridPosition.y });
                        ecb.SetComponent(entityInQueryIndex, arrow, new Direction() { Value = arrowDirection });
                    }
                }).Run();

            m_EcbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
