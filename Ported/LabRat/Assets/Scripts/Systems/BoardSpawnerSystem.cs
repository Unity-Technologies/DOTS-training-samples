using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

public class BoardSpawnerSystem : SystemBase
{

    private readonly float4 evenColor = new float4(0.95f, 0.95f, 0.95f, 1f);
    private readonly float4 oddColor = new float4(0.80f, 0.78f, 0.88f, 1f);

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<BoardPrefabs>();
        RequireSingletonForUpdate<BoardSize>();
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

        // Where do walls go?  How do we start enumerating grid cells so they come out in a sane way?
        for (int i = 0; i < boardSize.Value.x; i++)
        {
            for (int j = 0; j < boardSize.Value.y; j++)
            {
                // 0x00, 0x01, 0x02, 0x04, 0x0
                byte currentWall = 0x0;
                if (i == 0) currentWall |= 0x1;
                if (j == 0) currentWall |= 0x8;
                if (i == boardSize.Value.x - 1) currentWall |= 0x2;
                if (j == boardSize.Value.y - 1) currentWall |= 0x4;
                int arrayPos = boardSize.Value.y * j + i;
                walls[arrayPos] = currentWall;
                EntityManager.SetComponentData(cells[arrayPos], new Translation { Value = new float3(i, 0, j) });
                EntityManager.SetComponentData(cells[arrayPos], new Color { Value = (arrayPos + ((boardSize.Value.y + 1) % 2) * (j % 2)) % 2 == 0 ? evenColor : oddColor });
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

        EntityManager.RemoveComponent<BoardSize>(GetSingletonEntity<BoardSize>());
    }
}
