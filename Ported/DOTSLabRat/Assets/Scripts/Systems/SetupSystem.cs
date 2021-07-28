using System.Linq;
using DOTSRATS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SetupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        const float k_yRangeSize = 0.03f;

        var randomSeed = GetSingleton<BoardSpawner>().randomSeed;
        var random = Random.CreateFromIndex(randomSeed == 0 ? (uint)System.DateTime.Now.Ticks : randomSeed);

        Entities
            .WithStructuralChanges()
            .WithAll<BoardSpawner>()
            .ForEach((Entity entity, in BoardSpawner boardSpawner) =>
            {
                var size = boardSpawner.boardSize;
                int playerNumber = 0;
                
                // Spawn the GameState
                var gameState = EntityManager.CreateEntity();
                EntityManager.AddComponent<GameState>(gameState);
                EntityManager.SetComponentData(gameState, new GameState{boardSize = size, timer = 30f});
                var cellStructs = new NativeArray<CellStruct>(size * size, Allocator.TempJob);
                
                // Init hole coordinates
                var holesToSpawn = random.NextInt(0, boardSpawner.maxHoles + 1);
                var holeCoords = new NativeArray<int2>(holesToSpawn, Allocator.Temp);
                for (int i = 0; i < holesToSpawn; ++i)
                    holeCoords[i] = GenerateNextHoleCoord(holeCoords, size, ref random);
                
                // Init inner wall spawn parameters
                var innerWallsToSpawn = random.NextInt(boardSpawner.maxWallsRange.x, boardSpawner.maxWallsRange.y + 1);

                var wallGenParams = new NativeHashMap<int2, int>(innerWallsToSpawn, Allocator.Temp);
                var paramsGenerated = 0;
                
                while (paramsGenerated < innerWallsToSpawn)
                {
                    var spawnParam = GenerateNextWallSpawnParam(wallGenParams, size, ref random, innerWallsToSpawn - paramsGenerated);
                    paramsGenerated += spawnParam.z;
                    wallGenParams.Add(new int2(spawnParam.x, spawnParam.y), spawnParam.z);
                }

                for (int z = 0; z < size; ++z)
                {
                    for (int x = 0; x < size; ++x)
                    {
                        var cellIndex = z * size + x;
                        // Spawn tiles / holes
                        var cell = cellStructs[cellIndex];
                        var cellCoord = new int2(x, z);

                        if (!CoordExistsInArray(new int2(x, z), holeCoords))
                        {
                            var tile = EntityManager.Instantiate(boardSpawner.tilePrefab);
                            var yValue = random.NextFloat(-k_yRangeSize, k_yRangeSize);
                            var translation = new Translation() { Value = new float3(x, yValue - 0.5f, z) };
                            EntityManager.SetComponentData(tile, translation);
                        }
                        else
                            cell.hole = true;

                        // Spawn outer walls
                        if (x == 0 || x == size - 1)
                            SpawnWall(boardSpawner, cellCoord, x == 0 ? Direction.West : Direction.East, ref cell, cellStructs);
                        if (z == 0 || z == size - 1)
                            SpawnWall(boardSpawner,cellCoord, z == 0 ? Direction.South : Direction.North, ref cell, cellStructs);

                        // Spawn inner walls
                        if (wallGenParams.TryGetValue(cellCoord, out var spawnCount))
                            SpawnInnerWall(boardSpawner, cellCoord, spawnCount, ref cell, cellStructs, ref random);

                        //spawn goals
                        if (ShouldPlaceGoalTile(cellCoord, size))
                            SpawnGoal(boardSpawner, cellCoord, playerNumber++, ref cell);

                        cellStructs[cellIndex] = cell;
                    }
                }

                /*for (int z = 0; z < size; ++z)
                    for (int x = 0; x < size; ++x)
                        Debug.Log($"Wall layout at: ({x},{z}): {cellStructs[z * size + x].wallLayout}");*/
                    
                EntityManager.AddBuffer<CellStruct>(gameState).AddRange(cellStructs);
                SetAnimalSpawners(boardSpawner, size, ref random);

                EntityManager.DestroyEntity(entity);
                holeCoords.Dispose();
                wallGenParams.Dispose();
                cellStructs.Dispose();
            }).Run();
    }

    void SpawnWall(BoardSpawner boardSpawner, int2 coord, Direction direction, ref CellStruct cellStruct, NativeArray<CellStruct> cellStructs)
    {
        Direction wallsSet = direction;
        cellStruct.wallLayout |= direction;
        if (GetOppositeCoord(coord, direction, boardSpawner.boardSize, out var oppositeCoord))
        {
            var oppositeDir = GetOppositeDirection(direction);
            var oppositeCellIndex = oppositeCoord.y * boardSpawner.boardSize + oppositeCoord.x;
            var oppositeCell = cellStructs[oppositeCellIndex];
            oppositeCell.wallLayout |= oppositeDir;
            wallsSet |= oppositeDir;
            cellStructs[oppositeCellIndex] = oppositeCell;
        }

        // Optimization: For presentation, only North and East walls, unless we're at South or West edge of the board
        if (direction == Direction.West && ((wallsSet & Direction.East) != 0))
        {
            direction = Direction.East;
            coord = oppositeCoord;
        }

        if (direction == Direction.South && ((wallsSet & Direction.North) != 0))
        {
            direction = Direction.North;
            coord = oppositeCoord;
        }
        
        if ((direction == Direction.West && coord.x > 0) ||
            (direction == Direction.South && coord.y > 0))
        { 
            return;
        }

        float3 position = new float3(coord.x, 0.25f, coord.y);
        quaternion rotation = quaternion.identity;

        switch (direction)
        {
            case Direction.North:
                position += new float3(0f, 0f, 0.5f);
                rotation = quaternion.AxisAngle(math.up(), math.radians(90f));
                break;

            case Direction.South:
                position += new float3(0f, 0f, -0.5f);
                rotation = quaternion.AxisAngle(math.up(), math.radians(90f));
                break;

            case Direction.East:
                position += new float3(0.5f, 0f, 0f);
                break;

            case Direction.West:
                position += new float3(-0.5f, 0f, 0f);
                break;
        }

        var wall = EntityManager.Instantiate(boardSpawner.wallPrefab);
        EntityManager.SetComponentData(wall, new Translation() {Value = position});
        EntityManager.SetComponentData(wall, new Rotation() {Value = rotation});
    }

    void SpawnInnerWall(BoardSpawner boardSpawner, int2 coord, int amount, ref CellStruct cellStruct, NativeArray<CellStruct> cellStructs, ref Random random)
    {
        if (amount == 1)
            SpawnWall(boardSpawner, coord, (Direction)(1 << random.NextInt(0, 4)), ref cellStruct, cellStructs);
        if (amount == 2)
        {
            if (random.NextInt(0, 2) == 0)
            {
                if (random.NextInt(0, 2) == 0)
                {
                    SpawnWall(boardSpawner, coord, Direction.East, ref cellStruct, cellStructs);
                    SpawnWall(boardSpawner, coord, Direction.West, ref cellStruct, cellStructs);
                }
                else
                {
                    SpawnWall(boardSpawner, coord, Direction.North, ref cellStruct, cellStructs);
                    SpawnWall(boardSpawner, coord, Direction.South, ref cellStruct, cellStructs);
                }
            }
            else
            {
                var baseDir = (Direction)(1 << random.NextInt(0, 4));
                var nextCardinalDir = GetNextCardinalCW(baseDir);
                
                SpawnWall(boardSpawner, coord, baseDir, ref cellStruct, cellStructs);
                SpawnWall(boardSpawner, coord, nextCardinalDir, ref cellStruct, cellStructs);

            }
        }
        else
        {
            var dir = (Direction)(1 << random.NextInt(0, 4));
            for (int i = 0; i < amount; ++i)
            {
                SpawnWall(boardSpawner, coord,dir, ref cellStruct, cellStructs);
                dir = GetNextCardinalCW(dir);
            }
        }
    }
    
    public Entity SpawnGoal(BoardSpawner boardSpawner, int2 coord, int playerNumber,  ref CellStruct cellStruct)
    {
        Entity goal = default;
        float3 position = new float3(coord.x, -0.5f, coord.y);

        goal = EntityManager.Instantiate(boardSpawner.goalPrefab);
        EntityManager.SetComponentData(goal, new Translation() { Value = position });
        EntityManager.SetComponentData(goal, new Goal() { playerNumber = playerNumber });

        cellStruct.goal = goal;

        return goal;
    }

    // TODO: also return player id as an out variable
    public bool ShouldPlaceGoalTile(int2 coord, int boardSize)
    {
        var halfSize = boardSize / 2;
        var midCoord = halfSize + (boardSize % 2 == 0 ? 1 : 2);
        var offsetFromCenter = halfSize / 6;
        
        if ((coord.x == (halfSize - offsetFromCenter - 1) || coord.x == (midCoord + offsetFromCenter - 1)) &&
            (coord.y == (halfSize - offsetFromCenter - 1) || coord.y == (midCoord + offsetFromCenter - 1)))
        {
            return true;
        }
        
        return false;
    }

    public void SetAnimalSpawners(BoardSpawner boardSpawner, int boardSize, ref Random random)
    {
        Entity catSpawnPointOne = EntityManager.Instantiate(boardSpawner.catSpawnerPrefab);
        EntityManager.AddComponent<InPlay>(catSpawnPointOne);
        var animalSpawner = EntityManager.GetComponentData<AnimalSpawner>(catSpawnPointOne);
        animalSpawner.randomSeed = random.NextUInt();
        EntityManager.SetComponentData(catSpawnPointOne, animalSpawner);
        EntityManager.SetComponentData(catSpawnPointOne, new Translation { Value = new float3(0, -0.5f, 0) });
        EntityManager.AddComponentData(catSpawnPointOne, new DirectionData { Value = Direction.North });

        Entity catSpawnPointTwo = EntityManager.Instantiate(boardSpawner.catSpawnerPrefab);
        EntityManager.AddComponent<InPlay>(catSpawnPointTwo);
        animalSpawner = EntityManager.GetComponentData<AnimalSpawner>(catSpawnPointOne);
        animalSpawner.randomSeed = random.NextUInt();
        EntityManager.SetComponentData(catSpawnPointOne, animalSpawner);
        EntityManager.SetComponentData(catSpawnPointTwo, new Translation { Value = new float3(boardSize - 1, -0.5f, boardSize - 1) });
        EntityManager.AddComponentData(catSpawnPointTwo, new DirectionData { Value = Direction.South });

        Entity ratSpawnPointOne = EntityManager.Instantiate(boardSpawner.ratSpawnerPrefab);
        EntityManager.AddComponent<InPlay>(ratSpawnPointOne);
        animalSpawner = EntityManager.GetComponentData<AnimalSpawner>(catSpawnPointOne);
        animalSpawner.randomSeed = random.NextUInt();
        EntityManager.SetComponentData(catSpawnPointOne, animalSpawner);
        EntityManager.SetComponentData(ratSpawnPointOne, new Translation { Value = new float3(0, -0.5f, boardSize - 1) });
        EntityManager.AddComponentData(ratSpawnPointOne, new DirectionData { Value = Direction.East });

        Entity ratSpawnPointTwo = EntityManager.Instantiate(boardSpawner.ratSpawnerPrefab);
        EntityManager.AddComponent<InPlay>(ratSpawnPointTwo);
        animalSpawner = EntityManager.GetComponentData<AnimalSpawner>(catSpawnPointOne);
        animalSpawner.randomSeed = random.NextUInt();
        EntityManager.SetComponentData(catSpawnPointOne, animalSpawner);
        EntityManager.SetComponentData(ratSpawnPointTwo, new Translation { Value = new float3(boardSize - 1, -0.5f, 0) });
        EntityManager.AddComponentData(ratSpawnPointTwo, new DirectionData { Value = Direction.West });
    }

    int2 GenerateNextHoleCoord(NativeArray<int2> holeCoords, int boardSize, ref Random random)
    {
        int2 nextCoord = int2.zero; 
        do
        {
            nextCoord = new int2(random.NextInt(0, boardSize), random.NextInt(0, boardSize));
        } while (ShouldPlaceGoalTile(nextCoord, boardSize) || 
                 IsCoordEdge(nextCoord, boardSize) || 
                 CoordExistsInArray(nextCoord, holeCoords));

        return nextCoord;
    }
    
    int3 GenerateNextWallSpawnParam(NativeHashMap<int2, int> spawnParams, int boardSize, ref Random random, int maxSpawnCount)
    {
        int2 spawnCoord = int2.zero;
        
        // We want to spawn max up to 3 walls per cell, but not more than the number of pending walls to spawn
        maxSpawnCount = math.min(3, maxSpawnCount);
        var spawnCount = random.NextInt(1, maxSpawnCount + 1);

        do
        {
            spawnCoord = new int2(random.NextInt(0, boardSize), random.NextInt(0, boardSize));
        } while (ShouldPlaceGoalTile(spawnCoord, boardSize) ||
                 IsCoordCorner(spawnCoord, boardSize) ||
                 spawnParams.ContainsKey(spawnCoord));

        return new int3(spawnCoord.x, spawnCoord.y, spawnCount);
    }

    bool CoordExistsInArray(int2 coord, NativeArray<int2> coordArray)
    {
        foreach (int2 arrayElement in coordArray)
        {
            if (arrayElement.x == coord.x && arrayElement.y == coord.y)
                return true;
        }

        return false;
    }
    
    bool CoordExistsInArray(int3 coord, NativeArray<int3> coordArray)
    {
        foreach (int3 arrayElement in coordArray)
        {
            if (arrayElement.x == coord.x && 
                arrayElement.y == coord.y &&
                arrayElement.z == coord.z)
                return true;
        }

        return false;
    }

    bool IsCoordCorner(int2 coord, int boardSize)
    {
        if ((coord.x == 0 && coord.y == 0) ||
            (coord.x == 0 && coord.y == boardSize - 1) ||
            (coord.y == 0 && coord.x == boardSize - 1) ||
            (coord.x == boardSize - 1 && coord.y == boardSize - 1))
        {
            return true;
        }

        return false;
    }
    
    bool IsCoordEdge(int2 coord, int boardSize)
    {
        if ((coord.x == 0 || coord.y == 0) ||
            (coord.x == boardSize - 1 || coord.y == boardSize - 1))
        {
            return true;
        }

        return false;
    }

    bool GetOppositeCoord(int2 coord, Direction direction, int boardSize, out int2 oppositeCoord)
    {
        oppositeCoord = default;
        switch (direction)
        {
            case Direction.North:
                coord.y += 1;
                if (coord.y >= boardSize)
                    return false;
                break;
            case Direction.South:
                coord.y -= 1;
                if (coord.y < 0)
                    return false;
                break;
            case Direction.West:
                coord.x -= 1;
                if (coord.x < 0)
                    return false;
                break;
            case Direction.East:
                coord.x += 1;
                if (coord.x >= boardSize)
                    return false;
                break;
            default:
                return false;
        }

        oppositeCoord = coord;
        return true;
    }

    Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return Direction.South;
            case Direction.South:
                return Direction.North;
            case Direction.West:
                return Direction.East;
            case Direction.East:
                return Direction.West;
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
        }

        return direction;
    }
    
    Direction GetNextCardinalCW(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return Direction.East;
            case Direction.East:
                return Direction.South;
            case Direction.South:
                return Direction.West;
            case Direction.West:
                return Direction.North;
        }

        return direction;
    }
}
