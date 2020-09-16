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

        Entity cellEntity = EntityManager.CreateEntity(typeof(CellData));
        EntityManager.SetComponentData(cellEntity, new CellData { cells = cells });

        Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)DateTime.UtcNow.Millisecond + 1);

        // Where do walls go?  How do we start enumerating grid cells so they come out in a sane way?
        for (int i = 0; i < boardSize.Value.x; i++)
        {
            for (int j = 0; j < boardSize.Value.y; j++)
            {
                int arrayPos = boardSize.Value.y * j + i;

                byte currentWall = 0x0;

                EntityManager.SetComponentData(cells[arrayPos], new Translation { Value = new float3(i, 0, j) });

                if (random.NextFloat() < 0.01f)
                {
                    EntityManager.AddComponent<DisableRendering>(cells[arrayPos]);
                }
                else
                {
                    // 0x00, 0x01, 0x02, 0x04, 0x0
                    if (i == 0 || random.NextFloat() < 0.05f)
                    {
                        currentWall |= 0x1;
                        var wall = EntityManager.Instantiate(boardPrefabs.wallPrefab);
                        EntityManager.SetComponentData(wall, new Translation { Value = new float3(i - 0.47f, 0.7f, j) });
                        EntityManager.SetComponentData(wall, new Rotation { Value = quaternion.RotateY(math.PI) });
                    }
                    if (j == 0 || random.NextFloat() < 0.05f)
                    {
                        currentWall |= 0x8;

                        var wall = EntityManager.Instantiate(boardPrefabs.wallPrefab);
                        EntityManager.SetComponentData(wall, new Translation { Value = new float3(i, 0.7f, j - 0.47f) });
                    }
                    if (i == boardSize.Value.x - 1 || random.NextFloat() < 0.05f)
                    {
                        currentWall |= 0x2;

                        var wall = EntityManager.Instantiate(boardPrefabs.wallPrefab);
                        EntityManager.SetComponentData(wall, new Translation { Value = new float3(i + 0.47f, 0.7f, j) });
                        EntityManager.SetComponentData(wall, new Rotation { Value = quaternion.RotateY(math.PI) });
                    }
                    if (j == boardSize.Value.y - 1 || random.NextFloat() < 0.05f) 
                    { 
                        currentWall |= 0x4;

                        var wall = EntityManager.Instantiate(boardPrefabs.wallPrefab);
                        EntityManager.SetComponentData(wall, new Translation { Value = new float3(i, 0.7f, j + 0.47f) });
                    }
                
                    EntityManager.SetComponentData(cells[arrayPos], new Color { Value = (arrayPos + ((boardSize.Value.y + 1) % 2) * (j % 2)) % 2 == 0 ? evenColor : oddColor });
                }

                walls[arrayPos] = currentWall;
            }
        }

        // Where do the bases go?
        Position redPosition = new Position() { Value = new int2(4, 4) };
        Position greenPosition = new Position() { Value = new int2(4, 8) };
        Position bluePosition = new Position() { Value = new int2(8, 4) };
        Position blackPosition = new Position() { Value = new int2(8, 8) };
        Color redColor = new Color() { Value = new float4(1.0f, 0.0f, 0.0f, 1.0f) };
        Color greenColor = new Color() { Value = new float4(0.0f, 1.0f, 0.0f, 1.0f) };
        Color blueColor = new Color() { Value = new float4(0.0f, 0.0f, 1.0f, 1.0f) };
        Color blackColor = new Color() { Value = new float4(0.0f, 0.0f, 0.0f, 1.0f) };
        Entity redEntity = EntityManager.Instantiate(boardPrefabs.basePrefab);
        EntityManager.AddComponent<BaseTag>(redEntity);
        EntityManager.AddComponentData<Position>(redEntity, redPosition);
        EntityManager.AddComponentData<Color>(redEntity, redColor);
        EntityManager.AddComponentData<Score>(redEntity, new Score());
        Entity greenEntity = EntityManager.Instantiate(boardPrefabs.basePrefab);
        EntityManager.AddComponent<BaseTag>(greenEntity);
        EntityManager.AddComponentData<Position>(greenEntity, greenPosition);
        EntityManager.AddComponentData<Color>(greenEntity, greenColor);
        EntityManager.AddComponentData<Score>(greenEntity, new Score());
        Entity blueEntity = EntityManager.Instantiate(boardPrefabs.basePrefab);
        EntityManager.AddComponent<BaseTag>(blueEntity);
        EntityManager.AddComponentData<Position>(blueEntity, bluePosition);
        EntityManager.AddComponentData<Color>(blueEntity, blueColor);
        EntityManager.AddComponentData<Score>(blueEntity, new Score());
        Entity blackEntity = EntityManager.Instantiate(boardPrefabs.basePrefab);
        EntityManager.AddComponent<BaseTag>(blackEntity);
        EntityManager.AddComponentData<Position>(blackEntity, blackPosition);
        EntityManager.AddComponentData<Color>(blackEntity, blackColor);
        EntityManager.AddComponentData<Score>(blackEntity, new Score());

        // Need spawners for mice & cats
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

        EntityManager.RemoveComponent<BoardSize>(GetSingletonEntity<BoardSize>());
    }
}
