using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(EndInitializationEntityCommandBufferSystem))]
[UpdateAfter(typeof(CopyInitialTransformFromGameObjectSystem))] //maybe not necessary but I’m afraid it might create issues
public class SpawnBoardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Unity.Mathematics.Random(1234);

        Entities
            .WithNone<BoardInitializedTag>()
            .WithoutBurst()
            .ForEach((Entity entity, ref GameData gameData, ref DynamicBuffer<GridCellContent> gridContent, in BoardDefinition boardDefinition, in BoardPrefab boardPrefab) =>
            {
                // Store the grid world position
                var firstCellPosition = new FirstCellPosition
                {
                    // TODO: Also get the following from authoring:
                    Value = new float3(0, 0, 0)
                };
                ecb.AddComponent(entity, firstCellPosition);

                // Create the player entities
                var playerReferenceBuffer = ecb.AddBuffer<PlayerReference>(entity);
                playerReferenceBuffer.Capacity = 4;
                for (int i = 0; i < 4; ++i)
                {
                    Entity playerEntity = ecb.CreateEntity();
                    ecb.AddComponent(playerEntity, new PlayerIndex() { Value = i });
                    ecb.AddComponent<Score>(playerEntity);
                    ecb.AddBuffer<ArrowReference>(playerEntity);
                    ecb.AppendToBuffer(entity, new PlayerReference() { Player = playerEntity });
                    if (i != 0)
                        ecb.AddComponent<AITargetCell>(playerEntity);

                    ecb.SetName(playerEntity, "Player " + i);
                }

                int numberCells = boardDefinition.NumberColumns * boardDefinition.NumberRows;
                for (int i = 0; i < numberCells; ++i)
                {
                    gridContent.Add(new GridCellContent() { Type = GridCellType.None });
                }

                int numHoles = random.NextInt(0, 4);
                for (int hole = 0; hole < numHoles; ++hole)
                {
                    int holeIndex = random.NextInt(0, numberCells);
                    var gridContentCell = gridContent[holeIndex];
                    gridContentCell.Type = GridCellType.Hole;
                    gridContent[holeIndex] = gridContentCell;
                }

                //create the board cell entities
                for (int boardIndex = 0; boardIndex < numberCells; ++boardIndex)
                {
                    if (gridContent[boardIndex].Type == GridCellType.Hole)
                        continue;
                    int j = GridCellContent.GetColumnIndexFrom1DIndex(boardIndex, boardDefinition.NumberColumns);
                    int i = GridCellContent.GetRowIndexFrom1DIndex(boardIndex, boardDefinition.NumberColumns);
                    Entity cellPrefab = (j % 2 == i % 2 ? boardPrefab.DarkCellPrefab : boardPrefab.LightCellPrefab);
                    var cell = ecb.Instantiate(cellPrefab);

                    ecb.SetComponent(cell, new Translation
                    {
                        Value = new float3(i*boardDefinition.CellSize, 0, j*boardDefinition.CellSize)
                    });
                    ecb.AddComponent(cell, new GridPosition(){X=j,Y=i});
                }

                //Spawn some arrows randomly, need to remove that eventually when players can put arrows
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        Entity arrowPrefab = boardPrefab.ArrowPrefab;
                        var posX = random.NextInt(0, boardDefinition.NumberRows);
                        var posY = random.NextInt(0, boardDefinition.NumberColumns);
                        var arrow = ecb.Instantiate(arrowPrefab);
                        //
                        ecb.SetComponent(arrow, new Translation
                        {
                            Value = new float3(posX*boardDefinition.CellSize, 1.01f, posY*boardDefinition.CellSize)
                        });
                        ecb.AddComponent(arrow, new GridPosition(){X=posX,Y=posY});
                        ecb.AddComponent(arrow, new PlayerIndex(){Value = k});
                        ecb.AddComponent(arrow, new Direction(){Value = Dir.Right});
                        ecb.AddComponent<URPMaterialPropertyBaseColor>(arrow);
                        ecb.AddComponent<PropagateColor>(arrow);
                    }
                }
                
                //Goals are spawned randomly but shouldn’t
                for (int k = 0; k < 4; k++)
                {
                    Entity goalPrefab = boardPrefab.GoalPrefab;
                    var posX = random.NextInt(0, boardDefinition.NumberRows);
                    var posY = random.NextInt(0, boardDefinition.NumberColumns);
                    var spawnedEntity = ecb.Instantiate(goalPrefab);
                    
                    ecb.AddComponent<GoalTag>(spawnedEntity);
                    ecb.SetComponent(spawnedEntity, new Translation
                    {
                        Value = new float3(posX*boardDefinition.CellSize, 1.0f, posY*boardDefinition.CellSize)
                    });
                    ecb.AddComponent(spawnedEntity, new GridPosition(){X=posX,Y=posY});
                    ecb.AddComponent(spawnedEntity, new PlayerIndex(){Value = k});
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(spawnedEntity);
                    ecb.AddComponent<PropagateColor>(spawnedEntity);
                }

                // TODO: Add time?

                // TODO: Set up walls
                // TODO: Set up spawners
                // TODO: Set up goals
                // TODO: Set up holes
                // ...
                // TODO: Set up Game Data

                // Setup camera
                var gameObjectRefs = this.GetSingleton<GameObjectRefs>();
                var camera = gameObjectRefs.Camera;
                camera.orthographic = true;
                var overheadFactor = 1.5f;

                var maxSize = Mathf.Max(boardDefinition.NumberRows, boardDefinition.NumberColumns);
                var maxCellSize = boardDefinition.CellSize;
                camera.orthographicSize = maxSize * maxCellSize * .65f;

                // scale based on board dimensions - james
                var posXZ = Vector2.Scale(new Vector2(boardDefinition.NumberRows, boardDefinition.NumberColumns) * 0.5f, new Vector2(boardDefinition.CellSize, boardDefinition.CellSize));

                // hold position value adjusted by dimensions of board
                float3 camPosition = new Vector3(0, maxSize * maxCellSize * overheadFactor, 0);
                camera.transform.position = camPosition;

                // set camera to look at board center
                camera.transform.LookAt(new Vector3(posXZ.x, 0f, posXZ.y));


                // Only run on first frame the BoardInitializedTag is not found. Add it so we don't run again
                ecb.AddComponent(entity, new BoardInitializedTag());
                ecb.SetName(entity, "Board");
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        var ecb2 = new EntityCommandBuffer(Allocator.Temp);
            
        Entities.WithAll<GridPosition, PlayerIndex, URPMaterialPropertyBaseColor>()
                .ForEach((Entity e, DynamicBuffer<LinkedEntityGroup> linkedEntities) =>
        {
            foreach (var linkedEntity in linkedEntities)
            {
                ecb2.AddComponent<URPMaterialPropertyBaseColor>(linkedEntity.Value);
            }
        }).Run();
        
        ecb2.Playback(EntityManager);
        ecb2.Dispose();


    }
}
