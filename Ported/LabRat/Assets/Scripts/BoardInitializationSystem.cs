using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BoardInitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((Entity entity, in Board board) =>
            {
                var gameInfo = EntityManager.CreateEntity(typeof(GameInfo));
                SetComponent(gameInfo,
                    new GameInfo() {boardSize = new int2(board.size, board.size), ArrowPrefab = board.arrowPrefab});

                //UnityEngine.Debug.Log("Initializing Board");
                var wallEntity = EntityManager.CreateEntity();
                var wallBuffer = EntityManager.AddBuffer<TileWall>(wallEntity);
                wallBuffer.ResizeUninitialized(board.size * board.size);
                for (int i = 0; i < board.size * board.size; i++)
                {
                    wallBuffer[i] = new TileWall() {Value = 0};
                }

                var rand = new Unity.Mathematics.Random((uint) DateTime.Now.Millisecond);
                var boardQuarter = board.size / 4;

                var holeProbability = board.holeCount / (float) (board.size * board.size);
                var holeCount = board.holeCount;

                var wallProbability = board.wallCount / (float) (board.size * board.size);
                var wallCount = board.wallCount;

                var additionalSpawnerProbability = board.additionalSpawnerCount / (float) (board.size * board.size);
                var additionalSpawnerCount = board.additionalSpawnerCount;

                for (int x = 0; x < board.size; ++x)
                for (int z = 0; z < board.size; ++z)
                {
                    uint localWall = 0;

                    var posX = x;
                    var posY = rand.NextFloat(0.0f, board.yNoise);
                    var posZ = z;

                    var tileInstance = EntityManager.Instantiate(board.tilePrefab);
                    SetComponent(tileInstance, new Translation {Value = new float3(posX, posY, posZ)});
                    URPMaterialPropertyBaseColor floorColor;
                    floorColor = (x + z) % 2 == 0
                        ? new URPMaterialPropertyBaseColor {Value = new float4(0.8f, 0.8f, 0.8f, 1.0f)}
                        : new URPMaterialPropertyBaseColor {Value = new float4(0.6f, 0.6f, 0.6f, 1.0f)};
                    SetComponent(tileInstance, floorColor);
                    SetComponent(tileInstance, new Cell() {Index = x + z * board.size});

                    // Place border walls
                    if (x == 0) // West
                    {
                        PlaceWall(posX, posZ, DirectionDefines.West, board.wallPrefab, posY, tileInstance);
                        AddWallToWallTileBuffer(wallEntity, DirectionDefines.West, posX, posZ, board.size);
                        localWall |= DirectionDefines.West;
                    }
                    else if (x == (board.size - 1)) // East
                    {
                        PlaceWall(posX, posZ, DirectionDefines.East, board.wallPrefab, posY, tileInstance);
                        AddWallToWallTileBuffer(wallEntity, DirectionDefines.East, posX, posZ, board.size);
                        localWall |= DirectionDefines.East;
                    }

                    if (z == 0) // South
                    {
                        PlaceWall(posX, posZ, DirectionDefines.South, board.wallPrefab, posY, tileInstance);
                        AddWallToWallTileBuffer(wallEntity, DirectionDefines.South, posX, posZ, board.size);
                        localWall |= DirectionDefines.South;
                    }
                    else if (z == (board.size - 1)) // North
                    {
                        PlaceWall(posX, posZ, DirectionDefines.North, board.wallPrefab, posY, tileInstance);
                        AddWallToWallTileBuffer(wallEntity, DirectionDefines.North, posX, posZ, board.size);
                        localWall |= DirectionDefines.North;
                    }

                    // Place Spawn Points
                    if ((x == 0 && z == 0) || (x == (board.size - 1) && z == (board.size - 1)))
                    {
                        var mouseSpawnPoint = new SpawnPoint
                        {
                            spawnType = 0,
                            direction = (x == 0 && z == 0) ? DirectionDefines.North : DirectionDefines.South,
                            spawnCount = board.miceCount,
                        };
                        EntityManager.AddComponent<SpawnPoint>(tileInstance);
                        SetComponent(tileInstance, mouseSpawnPoint);
                        var buffer = EntityManager.AddBuffer<SpawnType>(tileInstance);
                        buffer.Length = 2;
                        buffer[0] = new SpawnType()
                        {
                            spawnPrefab = board.ratPrefab, spawnMax = board.miceCount, spawnFrequency = 0.2f, speedRange = new float2(1f, 5f), spawnDelay = board.miceWait
                        };
                        buffer[1] = new SpawnType()
                        {
                            spawnPrefab = board.catPrefab, spawnMax = board.catCount, spawnFrequency = 0f, speedRange = new float2(.5f, 1.5f), spawnDelay = board.catWait
                        };
                    }
                    else if ((x == 0 && z == (board.size - 1)) || (x == (board.size - 1) && z == 0))
                    {
                        var catSpawnPoint = new SpawnPoint
                        {
                            spawnType = 0,
                            direction = (x == 0 && z == (board.size - 1))
                                ? DirectionDefines.East
                                : DirectionDefines.West,
                            spawnCount = board.catCount,
                        };
                        EntityManager.AddComponent<SpawnPoint>(tileInstance);
                        SetComponent(tileInstance, catSpawnPoint);
                        var buffer = EntityManager.AddBuffer<SpawnType>(tileInstance);
                        buffer.Length = 1;
                        buffer[0] = new SpawnType()
                        {
                            spawnPrefab = board.catPrefab, spawnMax = board.catCount, spawnFrequency = 0f, speedRange = new float2(.5f, 1.5f), spawnDelay = board.catWait
                        };
                    }

                    // Place home base
                    if (x == boardQuarter && z == boardQuarter)
                        PlaceHomeBase(posX, posZ, 0, board.homeBasePrefab, posY, tileInstance);
                    else if (x == boardQuarter && z == board.size - boardQuarter - 1)
                        PlaceHomeBase(posX, posZ, 1, board.homeBasePrefab, posY, tileInstance);
                    else if (x == board.size - boardQuarter - 1 && z == board.size - boardQuarter - 1)
                        PlaceHomeBase(posX, posZ, 2, board.homeBasePrefab, posY, tileInstance);
                    else if (x == board.size - boardQuarter - 1 && z == boardQuarter)
                        PlaceHomeBase(posX, posZ, 3, board.homeBasePrefab, posY, tileInstance);
                    // Not a base: can be a hole
                    else
                    {
                        if (holeCount > 0 && rand.NextFloat(1f) < holeProbability)
                        {
                            EntityManager.DestroyEntity(tileInstance);

                            tileInstance = EntityManager.Instantiate(board.invisibleTilePrefab);
                            SetComponent(tileInstance, new Translation {Value = new float3(posX, posY, posZ)});
                            SetComponent(tileInstance, new Cell() {Index = x + z * board.size});
                            EntityManager.AddComponent<Hole>(tileInstance);
                            holeCount--;
                        }
                        else if( additionalSpawnerCount > 0 && rand.NextFloat(1f) < additionalSpawnerProbability)
                        {
                            var mouseSpawnPoint = new SpawnPoint
                            {
                                spawnType = 0,
                                direction = DirectionDefines.North,
                                spawnCount = 20,
                                spawnDelay = 6f,
                            };
                            EntityManager.AddComponent<SpawnPoint>(tileInstance);
                            SetComponent(tileInstance, mouseSpawnPoint);
                            var buffer = EntityManager.AddBuffer<SpawnType>(tileInstance);
                            buffer.Length = 2;
                            buffer[0] = new SpawnType()
                            {
                                spawnPrefab = board.ratPrefab,
                                spawnMax = 20,
                                spawnFrequency = 0.2f,
                                speedRange = new float2(1f, 5f)
                            };
                            buffer[1] = new SpawnType()
                            {
                                spawnPrefab = board.catPrefab,
                                spawnMax = 1,
                                spawnFrequency = 0f,
                                speedRange = new float2(.5f, 1.5f)
                            };
                        }
                    }

                    // Place other wall
                    if (wallCount > 0 && localWall < 15 && rand.NextFloat(1f) < wallProbability)
                    {
                        var coordinate = 2 ^ rand.NextUInt(0, 3);
                        if ((localWall & coordinate) == 0)
                        {
                            byte direction = 0;
                            switch (coordinate)
                            {
                                case 0: // North
                                    direction = DirectionDefines.North;
                                    if (posZ < board.size - 1)
                                    {
                                        AddWallToWallTileBuffer(wallEntity, DirectionDefines.South, posX, posZ + 1,
                                            board.size);
                                    }

                                    break;
                                case 1: // South
                                    direction = DirectionDefines.South;
                                    if (posZ > 0)
                                    {
                                        AddWallToWallTileBuffer(wallEntity, DirectionDefines.North, posX, posZ - 1,
                                            board.size);
                                    }

                                    break;
                                case 2: // West 
                                    direction = DirectionDefines.West;
                                    if (posX > 0)
                                    {
                                        AddWallToWallTileBuffer(wallEntity, DirectionDefines.East, posX - 1, posZ,
                                            board.size);
                                    }

                                    break;
                                case 3: // East
                                    direction = DirectionDefines.East;
                                    if (posX < board.size - 1)
                                    {
                                        AddWallToWallTileBuffer(wallEntity, DirectionDefines.West, posX + 1, posZ,
                                            board.size);
                                    }

                                    break;
                                default:
                                    break;
                            }

                            PlaceWall(posX, posZ, direction, board.wallPrefab, posY, tileInstance);
                            AddWallToWallTileBuffer(wallEntity, direction, posX, posZ, board.size);
                            wallCount--;
                        }
                    }
                }

                var playerArchetypes =
                    EntityManager.CreateArchetype(typeof(AICursor), typeof(PlayerTransform), typeof(Position));
                var mainPlayerArchetypes =
                    EntityManager.CreateArchetype(typeof(PlayerCursor), typeof(PlayerTransform), typeof(Position));
                for (int i = 0; i < 4; ++i)
                {
                    var player = EntityManager.CreateEntity(i == 0 ? mainPlayerArchetypes : playerArchetypes);
                    EntityManager.AddBuffer<PlayerArrow>(player);
                    SetComponent(player, new PlayerTransform
                    {
                        Index = i
                    });
                    var position = new Position();
                    switch (i)
                    {
                        case 0:
                            position.Value = new int2(boardQuarter, boardQuarter);
                            break;
                        case 1:
                            position.Value = new int2(boardQuarter, board.size - boardQuarter - 1);
                            break;
                        case 2:
                            position.Value = new int2(board.size - boardQuarter - 1, board.size - boardQuarter - 1);
                            break;
                        case 3:
                            position.Value = new int2(board.size - boardQuarter - 1, boardQuarter);
                            break;
                    }

                    SetComponent(player, position);
                    if (i > 0)
                    {
                        SetComponent(player, new AICursor
                        {
                            Destination = position.Value
                        });
                    }
                }

                var orthoSize = 2 * board.size / 3f;
                Camera.main.orthographicSize = orthoSize;
                Camera.main.transform.position = new Vector3(orthoSize / 2f, orthoSize, orthoSize / 2f);
                
                EntityManager.DestroyEntity(entity);
            }).Run();
    }

    void AddWallToWallTileBuffer(
        Entity wallBufferEntity,
        byte wallDirection,
        int positionX,
        int positionY,
        int boardWidth)
    {
        int index = boardWidth * positionY + positionX;
        var wallBuffer = EntityManager.GetBuffer<TileWall>(wallBufferEntity);
        var value = wallBuffer[index];
        wallBuffer.ElementAt(index).Value |= (byte) (value.Value | wallDirection);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="coordinate">North (0), South (1), East (2) or West (3)</param>
    /// <param name="board"></param>
    /// <param name="yOffset"></param>
    void PlaceWall(
        float posX,
        float posZ,
        byte direction,
        Entity wallPrefab,
        float yOffset,
        Entity tileInstance)
    {
        switch (direction)
        {
            case DirectionDefines.North: // North
                posZ += 0.5f;
                break;
            case DirectionDefines.South: // South
                posZ -= 0.5f;
                break;
            case DirectionDefines.West: // West 
                posX -= 0.5f;
                break;
            case DirectionDefines.East: // East
                posX += 0.5f;
                break;
            default:
                break;
        }

        var instance = EntityManager.Instantiate(wallPrefab);
        SetComponent(instance, new Translation {Value = new float3(posX, 0.725f + yOffset, posZ)});

        if ((direction & (DirectionDefines.North | DirectionDefines.South)) != 0
        ) // Turn the wall if it's placed on North or South
            SetComponent(instance, new Rotation {Value = quaternion.RotateY(math.radians(90f))});
    }

    void AddWallToTile(Entity tileEntity, byte wallDirection)
    {
        if (HasComponent<Wall>(tileEntity))
        {
            var wallComponent = GetComponent<Wall>(tileEntity);
            wallComponent.Value |= wallDirection;
            SetComponent<Wall>(tileEntity, wallComponent);
        }
        else
        {
            EntityManager.AddComponent<Wall>(tileEntity);
            SetComponent<Wall>(tileEntity, new Wall {Value = wallDirection});
        }
    }

    void PlaceHomeBase(
        int posX,
        int posZ,
        int playerIndex,
        Entity homeBasePrefab,
        float yOffset,
        Entity tileInstance)
    {
        var homebaseInstance = EntityManager.Instantiate(homeBasePrefab);
        SetComponent(homebaseInstance, new Translation {Value = new float3(posX, yOffset + 0.5f, posZ)});

        SetComponent(homebaseInstance, PlayerUtility.ColorFromPlayerIndex(playerIndex));

        EntityManager.AddComponent<HomeBase>(homebaseInstance);
        SetComponent<HomeBase>(homebaseInstance, new HomeBase
        {
            playerIndex = (byte) playerIndex,
            playerScore = 0
        });
    }
}
