using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct SpawningSystem : ISystem
{
    EntityQuery m_BaseColorQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();

        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();

        var config = SystemAPI.GetSingleton<Config>();

        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var random = Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        // create grid based off gridSize
        var temperatures = new GridTemperatures();
        temperatures.Init(config.gridSize);

        // random fire seeding
        var seedInitialValue = 0.05f;
        var percentAlreadyOnFire = 0.01f;
        var totalSeedsCount = math.max(percentAlreadyOnFire * config.gridSize * config.gridSize, 1);
        for (int i = 0; i < totalSeedsCount; i++)
        {
            var seedX = random.NextInt(0, config.gridSize - 1);
            var seedY = random.NextInt(0, config.gridSize - 1);
            temperatures.Set(seedX, seedY, seedInitialValue);
        }


        // create grid entity to pass grid info into
        var gridEntity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(gridEntity, temperatures);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // create flame cells
        var flameCells = CollectionHelper.CreateNativeArray<Entity>(config.gridSize * config.gridSize, Allocator.Temp);
        state.EntityManager.Instantiate(config.flameCellPrefab, flameCells);

        int row = 0;
        int column = 0;
        foreach (var (cellInfo, transform) in SystemAPI.Query<RefRW<CellInfo>, TransformAspect>())
        {
            cellInfo.ValueRW.indexX = row;
            cellInfo.ValueRW.indexY = column;
            transform.LocalPosition = new float3(row, 0, column);
            row++;
            if (row >= config.gridSize)
            {
                column++;
                row = 0;

                if (column >= config.gridSize)
                {
                    break;
                }
            }
        }

        // set default fire temperature to all flamecells
        foreach (var flameCell in flameCells)
        {
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(flameCell,
                queryMask,
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.defaultTemperatureColour });
        }

        state.Enabled = false;
        return;

        // create water cells
        var waterCells = CollectionHelper.CreateNativeArray<Entity>(config.waterCellCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.waterCellPrefab, waterCells);

        // create omniworkers
        var omniworkers = CollectionHelper.CreateNativeArray<Entity>(config.omniWorkersCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.omniWorkerPrefab, omniworkers);

        // create teams
        var teams = CollectionHelper.CreateNativeArray<Entity>(config.numberOfTeams, Allocator.Temp);

        var bucketPassers = CollectionHelper.CreateNativeArray<Entity>(config.numberOfTeams * config.bucketPassersPerTeam, Allocator.Temp);
        state.EntityManager.Instantiate(config.bucketPasserPrefab, bucketPassers);

        var bucketFetchers = CollectionHelper.CreateNativeArray<Entity>(config.numberOfTeams * config.bucketFetchersPerTeam, Allocator.Temp);
        state.EntityManager.Instantiate(config.bucketFetcherPrefab, bucketFetchers);

        // create buckets
        var buckets = CollectionHelper.CreateNativeArray<Entity>(config.bucketCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.bucketPrefab, buckets);

        // set empty bucket colour
        foreach (var bucket in buckets)
        {
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(bucket,
                queryMask,
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.emptyBucketColour });
        }

        state.Enabled = false;
    }
}
