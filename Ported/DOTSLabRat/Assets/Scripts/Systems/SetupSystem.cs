using System.Linq;
using DOTSRATS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SetupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        const float k_yRangeSize = 0.05f;

        var randomSeed = GetSingleton<BoardSpawner>().randomSeed;
        var random = Random.CreateFromIndex(randomSeed == 0 ? (uint)System.DateTime.Now.Ticks : randomSeed);

        if (!EntityManager.HasComponent<Initialized>(GetSingletonEntity<BoardSpawner>()))
        {
            // Cleanup anything that already exists!
            // TBD: optimize using chunking
            Entities
                .WithStructuralChanges()
                .WithAny<InPause, InPlay>()
                .WithNone<Player>()
                .ForEach((Entity entity) => EntityManager.DestroyEntity(entity))
                .Run();
            
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity, ref Player player, in DynamicBuffer<PlacedArrow> placedArrows) =>
                {
                    player.arrowToPlace = new int2(-1, -1);
                    player.arrowDirection = Direction.None;
                    player.currentArrow = 0;
                    player.arrowToRemove = new int2(-1, -1);
                    
                    for (int i = 0; i < placedArrows.Length; i++)
                    {
                        EntityManager.SetComponentData(placedArrows[i].entity, 
                            new Translation() { Value = new float3(-100, 0, -100) });
                    }

                    EntityManager.RemoveComponent<InPause>(entity);
                    EntityManager.AddComponent<InPlay>(entity);
                }).Run();
        }

        Entities
            .WithStructuralChanges()
            .WithNone<Initialized>()
            .ForEach((Entity entity, in BoardSpawner boardSpawner) =>
            {
                var size = boardSpawner.boardSize;
                int playerNumber = 0;
                
                // Spawn the GameState
                var gameState = EntityManager.CreateEntity();
                EntityManager.AddComponent<GameState>(gameState);
                EntityManager.AddComponent<InPlay>(gameState);
                EntityManager.SetComponentData(gameState, new GameState{boardSize = size, timer = boardSpawner.matchDuration});
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
                var attempts = 0;
                
                while (paramsGenerated < innerWallsToSpawn)
                {
                    if (!GenerateNextWallSpawnParam(wallGenParams, size, ref random, out var spawnParam))
                        break;
                    paramsGenerated += spawnParam.z;
                    wallGenParams.Add(new int2(spawnParam.x, spawnParam.y), spawnParam.z);
                    if (++attempts > innerWallsToSpawn * 2)
                        break;
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
                            EntityManager.AddComponent<URPMaterialPropertyBaseColor>(tile);
                            EntityManager.SetComponentData(tile,
                                new URPMaterialPropertyBaseColor()
                                {
                                    Value = (x + z % 2) % 2 == 0 ? new float4(1f, 1f, 1f, 1f) : new float4(0.9f, 0.9f, 0.9f, 1f)
                                });
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

                EntityManager.AddComponent<Initialized>(entity);
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
            var oppositeDir = Utils.GetOppositeDirection(direction);
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
                var nextCardinalDir = Utils.GetNextCardinalCW(baseDir);
                
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
                dir = Utils.GetNextCardinalCW(dir);
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
        EntityManager.AddComponent(goal, typeof(Scale));
        EntityManager.SetComponentData(goal, new Scale { Value = 1f });

        cellStruct.goal = goal;

        return goal;
    }
    
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
        void CreateAnimalSpawn(Entity prefab, float2 position, Direction direction, int max, ref Random random)
        {
            Entity spawnPoint = EntityManager.Instantiate(prefab);
            EntityManager.AddComponent<InPlay>(spawnPoint);
            var animalSpawner = EntityManager.GetComponentData<AnimalSpawner>(spawnPoint);
            animalSpawner.random = Random.CreateFromIndex(random.NextUInt());
            animalSpawner.maxAnimals = max;
            EntityManager.SetComponentData(spawnPoint, animalSpawner);
            EntityManager.SetComponentData(spawnPoint, new Translation {Value = new float3(position.x, -0.5f, position.y)});
            EntityManager.AddComponentData(spawnPoint, new DirectionData {Value = direction});
        }

        CreateAnimalSpawn(boardSpawner.catSpawnerPrefab, new float2(0f, 0f), Direction.North, boardSpawner.maxCats, ref random);
        CreateAnimalSpawn(boardSpawner.catSpawnerPrefab, new float2(boardSize - 1, boardSize - 1), Direction.South, boardSpawner.maxCats, ref random);
        CreateAnimalSpawn(boardSpawner.ratSpawnerPrefab, new float2(0f, boardSize - 1), Direction.East, boardSpawner.maxRats, ref random);
        CreateAnimalSpawn(boardSpawner.ratSpawnerPrefab, new float2(boardSize - 1, 0f), Direction.West, boardSpawner.maxRats, ref random);
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
    
    bool GenerateNextWallSpawnParam(NativeHashMap<int2, int> spawnParams, int boardSize, ref Random random, out int3 spawnParam)
    {
        int2 spawnCoord = int2.zero;
        spawnParam = default;
        
        // We want to spawn max up to 2 walls per cell, but not more than the number of pending walls to spawn
        int2 northCoord, southCoord, eastCoord, westCoord;
        int attemps = 0;

        do
        {
            spawnCoord = new int2(random.NextInt(0, boardSize), random.NextInt(0, boardSize));
            GetOppositeCoord(spawnCoord, Direction.North, boardSize, out northCoord);
            GetOppositeCoord(spawnCoord, Direction.South, boardSize, out southCoord);
            GetOppositeCoord(spawnCoord, Direction.East, boardSize, out eastCoord);
            GetOppositeCoord(spawnCoord, Direction.West, boardSize, out westCoord);
            if (++attemps > 10)
                return false;
        } while (DoesCoordConnectToGoal(spawnCoord, boardSize) ||
                 DoesCoordConnectToCorner(spawnCoord, boardSize) ||
                 spawnParams.ContainsKey(spawnCoord) ||
                 spawnParams.ContainsKey(northCoord) || 
                 spawnParams.ContainsKey(southCoord) || 
                 spawnParams.ContainsKey(eastCoord) || 
                 spawnParams.ContainsKey(westCoord));

        spawnParam = new int3(spawnCoord.x, spawnCoord.y, 1);
        return true;
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

    bool DoesCoordConnectToCorner(int2 coord, int boardSize)
    {
        if (IsCoordCorner(coord, boardSize))
            return true;

        for (int i = 0; i < 2; i++)
        {
            int sign = i == 0 ? 1 : -1;

            if (IsCoordCorner(coord + new int2(sign, sign), boardSize))
                return true;
            if (IsCoordCorner(coord + new int2(sign, 0), boardSize))
                return true;
            if (IsCoordCorner(coord + new int2(0, sign), boardSize))
                return true;
        }

        return false;
    }
    
    bool DoesCoordConnectToGoal(int2 coord, int boardSize)
    {
        if (ShouldPlaceGoalTile(coord, boardSize))
            return true;

        for (int i = 0; i < 2; i++)
        {
            int sign = i == 0 ? 1 : -1;

            if (ShouldPlaceGoalTile(coord + new int2(sign, sign), boardSize))
                return true;
            if (ShouldPlaceGoalTile(coord + new int2(sign, 0), boardSize))
                return true;
            if (ShouldPlaceGoalTile(coord + new int2(0, sign), boardSize))
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
}
