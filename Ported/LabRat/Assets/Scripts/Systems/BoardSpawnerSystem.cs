using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class WallData : IComponentData
{
    public NativeArray<byte> walls;
}
public class CellData : IComponentData
{
    public NativeArray<Entity> cells;
    public NativeArray<byte> directions; // 0 means no arrow
}

public struct BaseTag : IComponentData
{

}

public struct MouseAndCatSpawnerData : IComponentData
{
    public Entity prefabEntity;
    public float ticks;
    public float frequency;
}

public class BoardSpawnerSystem : SystemBase
{

    private readonly float4 evenColor = new float4(0.95f, 0.95f, 0.95f, 1f);
    private readonly float4 oddColor = new float4(0.80f, 0.78f, 0.88f, 1f);

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<BoardPrefabs>();
        RequireSingletonForUpdate<BoardSize>();
        RequireSingletonForUpdate<MousePrefabs>();
        RequireSingletonForUpdate<CatPrefabs>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // This is our pattern for accessing the wall data from other systems
        Entity wallEntity = GetSingletonEntity<WallData>();
        WallData wallData = EntityManager.GetComponentObject<WallData>(wallEntity);
        wallData.walls.Dispose();

        Entity cellEntity = GetSingletonEntity<CellData>();
        CellData cellData = EntityManager.GetComponentObject<CellData>(cellEntity);
        cellData.cells.Dispose();
        cellData.directions.Dispose();
    }

    protected override void OnUpdate()
    {
        BoardPrefabs boardPrefabs = GetSingleton<BoardPrefabs>();
        BoardSize boardSize = GetSingleton<BoardSize>();
        int arraySize = boardSize.Value.x * boardSize.Value.y;

        NativeArray<byte> walls = new NativeArray<byte>(arraySize, Allocator.Persistent);

        Entity wallEntity = EntityManager.CreateEntity(typeof(WallData));
        EntityManager.SetComponentData(wallEntity, new WallData { walls = walls });

        NativeArray<Entity> cells = new NativeArray<Entity>(arraySize, Allocator.Persistent);
        EntityManager.Instantiate(boardPrefabs.cellPrefab, cells);

        NativeArray<byte> directions = new NativeArray<byte>(arraySize, Allocator.Persistent);
        Entity cellEntity = EntityManager.CreateEntity(typeof(CellData));
        EntityManager.SetComponentData(cellEntity, new CellData { cells = cells, directions = directions });

        uint randomSeed = (uint)DateTime.UtcNow.Millisecond + 1;

        Unity.Mathematics.Random random = new Unity.Mathematics.Random(randomSeed);

        // Where do walls go?  How do we start enumerating grid cells so they come out in a sane way?
        for (int i = 0; i < boardSize.Value.x; i++)
        {
            for (int j = 0; j < boardSize.Value.y; j++)
            {
                int arrayPos = boardSize.Value.x * j + i;

                byte currentWall = walls[arrayPos];

                EntityManager.SetComponentData(cells[arrayPos], new Translation { Value = new float3(i, 0, j) });
                EntityManager.SetComponentData(cells[arrayPos], new Position { Value = new int2(i, j) });

                if (random.NextFloat() < 0.01f)
                {
                    EntityManager.AddComponent<DisableRendering>(cells[arrayPos]);
                    EntityManager.AddComponent<Hole>(cells[arrayPos]);
                }
                else
                {
                    // 0x00, 0x01, 0x02, 0x04, 0x0
                    if (i == 0 || random.NextFloat() < 0.05f) // West
                    {
                        currentWall |= 1 << (byte)DirectionEnum.West;

                        // If this cell has an interior wall that is "shared" with an adjacent cell, we add the "converse" wall
                        // to the neighboring cell.
                        if (i != 0)
                        {
                            int neighborPos = boardSize.Value.x * j + i - 1;
                            walls[neighborPos] |= 1 << (byte)DirectionEnum.East;
                        }
                        var wall = EntityManager.Instantiate(boardPrefabs.wallPrefab);
                        EntityManager.SetComponentData(wall, new Translation { Value = new float3(i - 0.47f, 0.7f, j) });
                        EntityManager.SetComponentData(wall, new Rotation { Value = quaternion.RotateY(math.PI) });
                    }
                    if (j == 0 || random.NextFloat() < 0.05f) // South
                    {
                        currentWall |= 1 << (byte)DirectionEnum.South;

                        // If this cell has an interior wall that is "shared" with an adjacent cell, we add the "converse" wall
                        // to the neighboring cell.
                        if (j != 0)
                        {
                            int neighborPos = boardSize.Value.x * (j - 1) + i;
                            walls[neighborPos] |= 1 << (byte)DirectionEnum.North;
                        }
                        var wall = EntityManager.Instantiate(boardPrefabs.wallPrefab);
                        EntityManager.SetComponentData(wall, new Translation { Value = new float3(i, 0.7f, j - 0.47f) });
                    }
                    if (i == boardSize.Value.x - 1 || random.NextFloat() < 0.05f) // East
                    {
                        currentWall |= 1 << (byte)DirectionEnum.East;

                        // If this cell has an interior wall that is "shared" with an adjacent cell, we add the "converse" wall
                        // to the neighboring cell.
                        if (i != boardSize.Value.x - 1)
                        {
                            int neighborPos = boardSize.Value.x * j + i + 1;
                            walls[neighborPos] |= 1 << (byte)DirectionEnum.West;
                        }
                        var wall = EntityManager.Instantiate(boardPrefabs.wallPrefab);
                        EntityManager.SetComponentData(wall, new Translation { Value = new float3(i + 0.47f, 0.7f, j) });
                        EntityManager.SetComponentData(wall, new Rotation { Value = quaternion.RotateY(math.PI) });
                    }
                    if (j == boardSize.Value.y - 1 || random.NextFloat() < 0.05f) // North
                    { 
                        currentWall |= 1 << (byte)DirectionEnum.North;

                        // If this cell has an interior wall that is "shared" with an adjacent cell, we add the "converse" wall
                        // to the neighboring cell.
                        if (j != boardSize.Value.y - 1)
                        {
                            int neighborPos = boardSize.Value.x * (j + 1) + i;
                            walls[neighborPos] |= 1 << (byte)DirectionEnum.South;
                        }

                        var wall = EntityManager.Instantiate(boardPrefabs.wallPrefab);
                        EntityManager.SetComponentData(wall, new Translation { Value = new float3(i, 0.7f, j + 0.47f) });
                    }
                
                    EntityManager.SetComponentData(cells[arrayPos], new Color { Value = (arrayPos + ((boardSize.Value.y + 1) % 2) * (j % 2)) % 2 == 0 ? evenColor : oddColor });
                }

                walls[arrayPos] = currentWall;
            }
        }

        // Where do the bases go? 1/3 & 2/3 of the board size.
        float offset = 1f / 3f;
        float sizeX = boardSize.Value.x;
        float sizeY = boardSize.Value.y;
        Position redPosition = new Position() { Value = new int2((int)(sizeX * offset), (int)(sizeY * offset)) };
        Position greenPosition = new Position() { Value = new int2((int)(sizeX * offset), (int)(sizeY * 2f * offset)) };
        Position bluePosition = new Position() { Value = new int2((int)(sizeX * 2f * offset), (int)(sizeY * offset)) };
        Position blackPosition = new Position() { Value = new int2((int)(sizeX * 2f * offset), (int)(sizeY * 2f * offset)) };
        Color redColor = new Color() { Value = new float4(1.0f, 0.0f, 0.0f, 1.0f) };
        Color greenColor = new Color() { Value = new float4(0.0f, 1.0f, 0.0f, 1.0f) };
        Color blueColor = new Color() { Value = new float4(0.0f, 0.0f, 1.0f, 1.0f) };
        Color blackColor = new Color() { Value = new float4(0.0f, 0.0f, 0.0f, 1.0f) };

        Entity redEntity = EntityManager.Instantiate(boardPrefabs.basePrefab);
        EntityManager.AddComponent<BaseTag>(redEntity);
        EntityManager.AddComponentData(redEntity, redPosition);
        EntityManager.AddComponentData(redEntity, new Score());
        EntityManager.AddComponentData(redEntity, new PlayerId { Value = Player.Red });
        EntityManager.AddComponentData(redEntity, new NpcInput { random = new Unity.Mathematics.Random(random.NextUInt()) });

        var redLink = EntityManager.GetComponentData<BaseComponentLink>(redEntity);
        EntityManager.SetComponentData(redLink.baseTop, redColor);
        EntityManager.SetComponentData(redLink.baseBottom, redColor);

        Entity greenEntity = EntityManager.Instantiate(boardPrefabs.basePrefab);
        EntityManager.AddComponent<BaseTag>(greenEntity);
        EntityManager.AddComponentData(greenEntity, greenPosition);
        EntityManager.AddComponentData(greenEntity, new Score());
        EntityManager.AddComponentData(greenEntity, new PlayerId { Value = Player.Green });
        EntityManager.AddComponentData(greenEntity, new NpcInput { random = new Unity.Mathematics.Random(random.NextUInt()) });

        var greenLink = EntityManager.GetComponentData<BaseComponentLink>(greenEntity);
        EntityManager.SetComponentData(greenLink.baseTop, greenColor);
        EntityManager.SetComponentData(greenLink.baseBottom, greenColor);

        Entity blueEntity = EntityManager.Instantiate(boardPrefabs.basePrefab);
        EntityManager.AddComponent<BaseTag>(blueEntity);
        EntityManager.AddComponentData(blueEntity, bluePosition);
        EntityManager.AddComponentData(blueEntity, new Score());
        EntityManager.AddComponentData(blueEntity, new PlayerId { Value = Player.Blue });
        EntityManager.AddComponentData(blueEntity, new NpcInput { random = new Unity.Mathematics.Random(random.NextUInt()) });

        var blueLink = EntityManager.GetComponentData<BaseComponentLink>(blueEntity);
        EntityManager.SetComponentData(blueLink.baseTop, blueColor);
        EntityManager.SetComponentData(blueLink.baseBottom, blueColor);

        Entity blackEntity = EntityManager.Instantiate(boardPrefabs.basePrefab);
        EntityManager.AddComponent<BaseTag>(blackEntity);
        EntityManager.AddComponentData(blackEntity, blackPosition);
        EntityManager.AddComponentData(blackEntity, new Score());
        EntityManager.AddComponentData(blackEntity, new PlayerId { Value = Player.Black });

        var blackLink = EntityManager.GetComponentData<BaseComponentLink>(blackEntity);
        EntityManager.SetComponentData(blackLink.baseTop, blackColor);
        EntityManager.SetComponentData(blackLink.baseBottom, blackColor);


        // Need spawners for mice & cats - Add them to all corners first
        MousePrefabs mousePrefabs = GetSingleton<MousePrefabs>();
        CatPrefabs catPrefabs = GetSingleton<CatPrefabs>();
        MouseAndCatSpawnerData spawnerDataMouse = new MouseAndCatSpawnerData() { prefabEntity = mousePrefabs.mousePrefab, ticks = 0.0f, frequency = 0.2f };
        MouseAndCatSpawnerData spawnerDataCat = new MouseAndCatSpawnerData() { prefabEntity = catPrefabs.catPrefab, ticks = 0.0f, frequency = 7.0f };
        Position spawnerPositionA = new Position() { Value = new int2(0, 0) };
        Position spawnerPositionB = new Position() { Value = new int2(boardSize.Value.x - 1, boardSize.Value.y - 1) };
        Position spawnerPositionC = new Position() { Value = new int2(0, boardSize.Value.y - 1) };
        Position spawnerPositionD = new Position() { Value = new int2(boardSize.Value.x - 1, 0) };
        Direction spawnerDirectionA = new Direction() { Value = DirectionEnum.East };
        Direction spawnerDirectionB = new Direction() { Value = DirectionEnum.West };
        Direction spawnerDirectionC = new Direction() { Value = DirectionEnum.South };
        Direction spawnerDirectionD = new Direction() { Value = DirectionEnum.North };
        Speed spawnerSpeedMouse = new Speed() { Value = 4.0f };
        Speed spawnerSpeedCat = new Speed() { Value = 1.0f };

        Entity spawnerEntityA = EntityManager.CreateEntity();
        EntityManager.AddComponentData<MouseAndCatSpawnerData>(spawnerEntityA, spawnerDataMouse);
        EntityManager.AddComponentData<Position>(spawnerEntityA, spawnerPositionA);
        EntityManager.AddComponentData<Direction>(spawnerEntityA, spawnerDirectionA);
        EntityManager.AddComponentData<Speed>(spawnerEntityA, spawnerSpeedMouse);
        Entity spawnerEntityB = EntityManager.CreateEntity();
        EntityManager.AddComponentData<MouseAndCatSpawnerData>(spawnerEntityB, spawnerDataMouse);
        EntityManager.AddComponentData<Position>(spawnerEntityB, spawnerPositionB);
        EntityManager.AddComponentData<Direction>(spawnerEntityB, spawnerDirectionB);
        EntityManager.AddComponentData<Speed>(spawnerEntityB, spawnerSpeedMouse);
        Entity spawnerEntityC = EntityManager.CreateEntity();
        EntityManager.AddComponentData<MouseAndCatSpawnerData>(spawnerEntityC, spawnerDataCat);
        EntityManager.AddComponentData<Position>(spawnerEntityC, spawnerPositionC);
        EntityManager.AddComponentData<Direction>(spawnerEntityC, spawnerDirectionC);
        EntityManager.AddComponentData<Speed>(spawnerEntityC, spawnerSpeedCat);
        Entity spawnerEntityD = EntityManager.CreateEntity();
        EntityManager.AddComponentData<MouseAndCatSpawnerData>(spawnerEntityD, spawnerDataCat);
        EntityManager.AddComponentData<Position>(spawnerEntityD, spawnerPositionD);
        EntityManager.AddComponentData<Direction>(spawnerEntityD, spawnerDirectionD);
        EntityManager.AddComponentData<Speed>(spawnerEntityD, spawnerSpeedCat);

        // How many spawners?
        int numMouseSpawners = boardSize.Value.x * boardSize.Value.y / 80; // one spawner per 80 grid cells or so
        int numCatSpawners = numMouseSpawners / 2;
        if (numMouseSpawners > 4)
        {
            numMouseSpawners -= 4;
            DirectionEnum dirEnum = DirectionEnum.North;
            for (int s = 0; s < numMouseSpawners; s++)
            {
                int2 curValue = new int2((int)(boardSize.Value.x * random.NextFloat()), (int)(boardSize.Value.y * random.NextFloat()));
                if (curValue.x == 0) dirEnum = DirectionEnum.North;
                if (curValue.x == boardSize.Value.x - 1) dirEnum = DirectionEnum.South;
                if (curValue.y == 0) dirEnum = DirectionEnum.East;
                if (curValue.y == boardSize.Value.y - 1) dirEnum = DirectionEnum.West;

                Position curSpawnerPosition = new Position() { Value = curValue };
                Direction curSpawnerDirection = new Direction() { Value = dirEnum };
                Entity curSpawnerEntity = EntityManager.CreateEntity();
                EntityManager.AddComponentData<MouseAndCatSpawnerData>(curSpawnerEntity, spawnerDataMouse);
                EntityManager.AddComponentData<Position>(curSpawnerEntity, curSpawnerPosition);
                EntityManager.AddComponentData<Direction>(curSpawnerEntity, curSpawnerDirection);
                EntityManager.AddComponentData<Speed>(curSpawnerEntity, spawnerSpeedMouse);
                // Change direction for next spawner
                dirEnum = (DirectionEnum)(s % 4);
            }
        }
 
        EntityManager.RemoveComponent<BoardPrefabs>(GetSingletonEntity<BoardPrefabs>());
    }
}
