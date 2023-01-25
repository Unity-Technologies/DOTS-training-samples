using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

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

        // create grid entity to pass grid info into 
        var gridEntity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(gridEntity, temperatures);
        
        state.Enabled = false;
        return;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        // create flame cells
        var flameCells = CollectionHelper.CreateNativeArray<Entity>(config.gridSize * config.gridSize, Allocator.Temp);
        state.EntityManager.Instantiate(config.flameCellPrefab, flameCells);

        // set default fire temperature to all flamecells
        foreach (var flameCell in flameCells)
        {
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(flameCell, 
                queryMask, 
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.defaultTemperatureColour });
        }

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
