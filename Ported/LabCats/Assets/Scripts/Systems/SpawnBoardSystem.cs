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
    static Dir GetRandomDirection(Unity.Mathematics.Random random)
    {
        int randomDirection = random.NextInt(0, 4);
        if (randomDirection == 0)
            return Dir.Up;
        if (randomDirection == 1)
            return Dir.Right;
        if (randomDirection == 2)
            return Dir.Down;
        return Dir.Left;
    }

    static Dir GetOppositeDirection(Dir direction)
    {
        if (direction == Dir.Up)
            return Dir.Down;
        if (direction == Dir.Down)
            return Dir.Up;
        if (direction == Dir.Left)
            return Dir.Right;
        return Dir.Left;
    }

    static int HasWallBoundariesInDirection(WallBoundaries walls, Dir dir)
    {
        switch (dir)
        {
            case Dir.Up:
                return (int)(walls & WallBoundaries.WallUp);
            case Dir.Down:
                return (int)(walls & WallBoundaries.WallDown);
            case Dir.Left:
                return (int)(walls & WallBoundaries.WallLeft);
            case Dir.Right:
                return (int)(walls & WallBoundaries.WallRight);
        }

        return 0;
    }

    static float halfWallThickness = 0.025f;
    static float kWallDensity = 0.2f;
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
                for (int boardIndex = 0; boardIndex < numberCells; ++boardIndex)
                {
                    WallBoundaries borderWall = WallBoundaries.NoWall;
                    int j = GridCellContent.GetColumnIndexFrom1DIndex(boardIndex, boardDefinition.NumberColumns);
                    int i = GridCellContent.GetRowIndexFrom1DIndex(boardIndex, boardDefinition.NumberColumns);

                    if(i == 0)
                        borderWall |= WallBoundaries.WallUp;
                    if (i == boardDefinition.NumberRows - 1)
                        borderWall |= WallBoundaries.WallDown;
                    if (j == 0)
                        borderWall |= WallBoundaries.WallLeft;
                    if (j == boardDefinition.NumberColumns - 1)
                        borderWall |= WallBoundaries.WallRight;
                    gridContent.Add(new GridCellContent() { Type = GridCellType.None, Walls = borderWall});
                }

                int numWalls = (int)(numberCells * kWallDensity);
                for (int c = 0; c < numWalls; ++c)
                {
                    int wallCellIndex = random.NextInt(0, numberCells);
                    var randomDirection = GetRandomDirection(random);

                    int neighbourCellIndex = GridCellContent.GetNeighbour1DIndexWithDirection(wallCellIndex,randomDirection,boardDefinition.NumberRows, boardDefinition.NumberColumns);
                    int wallBoundaryInCell = HasWallBoundariesInDirection(gridContent[wallCellIndex].Walls, randomDirection);
                    int neighbourBoundary = 0;
                    if (neighbourCellIndex != -1)
                        neighbourBoundary = HasWallBoundariesInDirection(gridContent[neighbourCellIndex].Walls, GetOppositeDirection(randomDirection));
                    if (wallBoundaryInCell + neighbourBoundary > 0)
                        --c;
                    else
                    {
                        var cellContent = gridContent[wallCellIndex];
                        cellContent.Walls |= GridCellContent.GetWallBoundariesFromDirection(randomDirection);
                        gridContent[wallCellIndex] = cellContent;
                        if (neighbourCellIndex != -1)
                        {
                            var neighbourCellContent = gridContent[neighbourCellIndex];
                            neighbourCellContent.Walls |= GridCellContent.GetWallBoundariesFromDirection(GetOppositeDirection(randomDirection));
                            gridContent[neighbourCellIndex] = neighbourCellContent;
                        }
                    }
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

                    var wallBoundaries = gridContent[boardIndex].Walls;
                    if ((wallBoundaries & WallBoundaries.WallUp) != 0)
                    {
                        var wallEntity = ecb.Instantiate(boardPrefab.WallPrefab);
                        ecb.SetComponent(wallEntity, new Translation{Value = new float3(i*boardDefinition.CellSize - 0.5f - halfWallThickness, 0.5f, j*boardDefinition.CellSize)});
                    }

                    if ((wallBoundaries & WallBoundaries.WallDown) != 0)
                    {
                        var wallEntity = ecb.Instantiate(boardPrefab.WallPrefab);
                        ecb.SetComponent(wallEntity, new Translation{Value = new float3(i*boardDefinition.CellSize + 0.5f + halfWallThickness, 0.5f, j*boardDefinition.CellSize)});
                    }

                }


                //Goals are spawned randomly but shouldn’t
                for (int k = 0; k < 3; k++)
                {
                    Entity goalPrefab = boardPrefab.GoalPrefab;
                    var posX = random.NextInt(0, boardDefinition.NumberRows);
                    var posY = random.NextInt(0, boardDefinition.NumberColumns);
                    var spawnedEntity = ecb.Instantiate(goalPrefab);

                    ecb.AddComponent<GoalTag>(spawnedEntity);
                    ecb.SetComponent(spawnedEntity, new Translation
                    {
                        Value = new float3(posX*boardDefinition.CellSize, 0.5f, posY*boardDefinition.CellSize)
                    });
                    ecb.AddComponent(spawnedEntity, new GridPosition(){X=posX,Y=posY});
                    ecb.AddComponent(spawnedEntity, new PlayerIndex(){Value = k});
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(spawnedEntity);
                    ecb.AddComponent<PropagateColor>(spawnedEntity);
                }

                for (int k = 0; k < 4; k++)
                {
                    Entity cursorPrefab = boardPrefab.CursorPrefab;
                    var spawnedEntity = ecb.Instantiate(cursorPrefab);

                    ecb.SetComponent(spawnedEntity, new Translation
                    {
                        Value = new float3(0.0f, 1.0f, 0.0f)
                    });
                    ecb.AddComponent(spawnedEntity, new PlayerIndex(){Value = k});
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(spawnedEntity);
                    ecb.AddBuffer<ArrowReference>(spawnedEntity);
                    if (k != 0)
                    {
                        ecb.AddComponent(spawnedEntity, new AITargetCell()
                        {
                            X = random.NextInt(0, boardDefinition.NumberRows),
                            Y = random.NextInt(0, boardDefinition.NumberColumns),
                        });
                    }

                    ecb.SetBuffer<ArrowReference>(spawnedEntity);

                    for (int l = 0; l < 3; l++)
                    {
                        Entity arrowPrefab = boardPrefab.ArrowPrefab;
                        var posX = random.NextInt(0, boardDefinition.NumberRows);
                        var posY = random.NextInt(0, boardDefinition.NumberColumns);
                        var arrow = ecb.Instantiate(arrowPrefab);
                        //
                        ecb.SetComponent(arrow, new Translation
                        {
                            Value = new float3(posX*boardDefinition.CellSize, 0.501f, posY*boardDefinition.CellSize)
                        });
                        ecb.AddComponent(arrow, new GridPosition(){X=posX,Y=posY});
                        ecb.AddComponent(arrow, new PlayerIndex(){Value = k});
                        ecb.AddComponent(arrow, new Direction(){Value = Dir.Right});
                        ecb.AddComponent<URPMaterialPropertyBaseColor>(arrow);
                        ecb.AddComponent<PropagateColor>(arrow);
                        ecb.AppendToBuffer(spawnedEntity, new ArrowReference(){Entity = arrow});
                    }
                }

                // TODO: Add time?

                // TODO: Set up walls

                // Set up spawners
                var spawner1 = ecb.CreateEntity();
                ecb.AddComponent(spawner1, new SpawnerData()
                {
                    Timer = 0f,
                    Frequency = 0.25f,
                    Direction = Dir.Up,
                    Type = SpawnerType.MouseSpawner,
                    X = 0,
                    Y = 0
                });
                ecb.SetName(spawner1, "Mouse Spawner 1");

                var spawner2 = ecb.CreateEntity();
                ecb.AddComponent(spawner2, new SpawnerData()
                {
                    Timer = 0f,
                    Frequency = 0.25f,
                    Direction = Dir.Down,
                    Type = SpawnerType.MouseSpawner,
                    X = boardDefinition.NumberColumns - 1,
                    Y = boardDefinition.NumberRows - 1
                });
                ecb.SetName(spawner2, "Mouse Spawner 2");

                var spawner3 = ecb.CreateEntity();
                ecb.AddComponent(spawner3, new SpawnerData()
                {
                    Timer = 0f,
                    Frequency = 5f,
                    Direction = Dir.Left,
                    Type = SpawnerType.CatSpawner,
                    X = boardDefinition.NumberColumns - 1,
                    Y = 0
                });
                ecb.SetName(spawner3, "CatSpawner 1");

                var spawner4 = ecb.CreateEntity();
                ecb.AddComponent(spawner4, new SpawnerData()
                {
                    Timer = 0f,
                    Frequency = 5f,
                    Direction = Dir.Right,
                    Type = SpawnerType.CatSpawner,
                    X = 0,
                    Y = boardDefinition.NumberRows - 1
                });

                // TODO: Set up goals
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
