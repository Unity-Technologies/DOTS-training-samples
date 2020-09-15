using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class WallData : IComponentData
{
    public NativeArray<byte> walls;
}

public struct BaseTag : IComponentData
{

}

public class BoardSpawnerSystem : SystemBase
{

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
    }

    protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.

        BoardPrefabs boardPrefabs = GetSingleton<BoardPrefabs>();
        BoardSize boardSize = GetSingleton<BoardSize>();
        int arraySize = boardSize.Value.x * boardSize.Value.y;
        NativeArray<byte> walls = new NativeArray<byte>(arraySize, Allocator.Persistent);
        Entity id = EntityManager.CreateEntity(typeof(WallData));
        EntityManager.SetComponentData<WallData>(id, new WallData { walls = walls });

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
                walls[boardSize.Value.y * j + i] = currentWall;
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
