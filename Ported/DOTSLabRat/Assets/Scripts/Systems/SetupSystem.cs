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
        
        var random = Random.CreateFromIndex((uint)System.DateTime.Now.Ticks);

        Entities
            .WithStructuralChanges()
            .WithAll<BoardSpawner>()
            .ForEach((Entity entity, in BoardSpawner boardSpawner) =>
            {
                var size = boardSpawner.boardSize;
                int playerNumber = 0;

                var holesToSpawn = random.NextInt(0, boardSpawner.maxHoles + 1);

                NativeArray<int2> holeCoords = new NativeArray<int2>(holesToSpawn, Allocator.Temp);

                for (int i = 0; i < holesToSpawn; ++i)
                    holeCoords[i] = GenerateNextHoleCoord(holeCoords, size, random);

                // Spawn the GameState
                var gameState = EntityManager.CreateEntity();
                EntityManager.AddComponent<GameState>(gameState);
                EntityManager.SetComponentData(gameState, new GameState{boardSize = size, timer = 30f});
                var cellStructs = new NativeArray<CellStruct>(size * size, Allocator.TempJob);

                for (int z = 0; z < size; ++z)
                {
                    for (int x = 0; x < size; ++x)
                    {
                        // Spawn tiles / holes
                        var cell = new CellStruct();

                        if (!HoleCoordExists(holeCoords, new int2(x, z)))
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
                            SpawnWall(boardSpawner, new int2(x, z), x == 0 ? Direction.West : Direction.East, ref cell);
                        if (z == 0 || z == size - 1)
                            SpawnWall(boardSpawner, new int2(x, z), z == 0 ? Direction.South : Direction.North, ref cell);

                        //spawn goals
                        if (ShouldPlaceGoalTile(new int2(x, z), size))
                            SpawnGoal(boardSpawner, new int2(x, z), playerNumber++, ref cell);

                        cellStructs[z * size + x] = cell;
                    }
                }

                EntityManager.AddBuffer<CellStruct>(gameState).AddRange(cellStructs);
                SetAnimalSpawners(boardSpawner, size);

                EntityManager.DestroyEntity(entity);
            }).Run();
    }

    public Entity SpawnWall(BoardSpawner boardSpawner, int2 coord, Direction direction, ref CellStruct cellStruct)
    {
        Entity wall = default;
        
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
        
        wall = EntityManager.Instantiate(boardSpawner.wallPrefab);
        EntityManager.SetComponentData(wall, new Translation() { Value = position });
        EntityManager.SetComponentData(wall, new Rotation() { Value = rotation });

        cellStruct.wallLayout |= direction;

        return wall;
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

    public void SetAnimalSpawners(BoardSpawner boardSpawner, int boardSize)
    {
        Entity catSpawnPointOne = EntityManager.Instantiate(boardSpawner.catSpawnerPrefab);
        EntityManager.AddComponent<InPlay>(catSpawnPointOne);
        EntityManager.SetComponentData(catSpawnPointOne, new Translation { Value = new float3(0, -0.5f, 0) });

        Entity catSpawnPointTwo = EntityManager.Instantiate(boardSpawner.catSpawnerPrefab);
        EntityManager.AddComponent<InPlay>(catSpawnPointTwo);
        EntityManager.SetComponentData(catSpawnPointTwo, new Translation { Value = new float3(boardSize - 1, -0.5f, boardSize - 1) });

        Entity ratSpawnPointOne = EntityManager.Instantiate(boardSpawner.ratSpawnerPrefab);
        EntityManager.AddComponent<InPlay>(ratSpawnPointOne);
        EntityManager.SetComponentData(ratSpawnPointOne, new Translation { Value = new float3(0, -0.5f, boardSize - 1) });

        Entity ratSpawnPointTwo = EntityManager.Instantiate(boardSpawner.ratSpawnerPrefab);
        EntityManager.AddComponent<InPlay>(ratSpawnPointTwo);
        EntityManager.SetComponentData(ratSpawnPointTwo, new Translation { Value = new float3(boardSize - 1, -0.5f, 0) });
    }

    int2 GenerateNextHoleCoord(NativeArray<int2> holeCoords, int boardSize, Random random)
    {
        int2 nextCoord = int2.zero; 
        do
        {
            nextCoord = new int2(random.NextInt(0, boardSize), random.NextInt(0, boardSize));
        } while (ShouldPlaceGoalTile(nextCoord, boardSize) || 
                 IsCoordEdge(nextCoord, boardSize) || 
                 HoleCoordExists(holeCoords, nextCoord));

        return nextCoord;
    }

    bool HoleCoordExists(NativeArray<int2> holeCoords, int2 coord)
    {
        foreach (int2 holeCoord in holeCoords)
        {
            if (holeCoord.x == coord.x && holeCoord.y == coord.y)
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
}
