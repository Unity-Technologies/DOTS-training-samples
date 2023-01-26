using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct SpawningSystem : ISystem
{
    EntityQuery m_BaseColorQuery;

    EntityQuery m_BucketFetcherQuery;

    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<Config>();

        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        
        m_BucketFetcherQuery = state.GetEntityQuery(ComponentType.ReadWrite<TeamInfo>(), ComponentType.Exclude<LineInfo>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var colourQueryMask = m_BaseColorQuery.GetEntityQueryMask();
        
        var config = SystemAPI.GetSingleton<Config>();

        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var random = Random.CreateFromIndex(1234);

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

        // create Omniworkers
        var omniworkers = CollectionHelper.CreateNativeArray<Entity>(config.omniWorkersCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.omniWorkerPrefab, omniworkers);
        
        // set Omniworker colour
        foreach (var omniworker in omniworkers)
        {
            state.EntityManager.SetComponentData(omniworker, 
                new MoveInfo()
                {
                    destinationPosition = new float2(0,0),
                    speed = config.workerSpeed
                });
            
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(omniworker,
                colourQueryMask,
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.omniworkerColour });
        }

        /*// create teams
        var teams = CollectionHelper.CreateNativeArray<Entity>(config.numberOfTeams, Allocator.Temp);
        state.EntityManager.Instantiate(state.EntityManager.CreateEntity(), teams);

        // create BucketFetchers
        var bucketFetchers = CollectionHelper.CreateNativeArray<Entity>(config.numberOfTeams * config.bucketFetchersPerTeam, Allocator.Temp);
        state.EntityManager.Instantiate(config.bucketFetcherPrefab, bucketFetchers);
        
        // set BucketPasser colour
        foreach (var bucketFetcher in bucketFetchers)
        {
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(bucketFetcher,
                colourQueryMask,
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.bucketFetcherColour });
        }
        
        // create BucketPassers
        var bucketPassers = CollectionHelper.CreateNativeArray<Entity>(config.numberOfTeams * config.bucketPassersPerTeam, Allocator.Temp);
        state.EntityManager.Instantiate(config.bucketPasserPrefab, bucketPassers);
        
        // set BucketPasser colour
        foreach (var bucketPasser in bucketPassers)
        {
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(bucketPasser,
                colourQueryMask,
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.bucketPasserColour });
        }
        
        // set team Entity all BucketPassers
        int indexInLine = 0;
        int teamIndex = 0;
        foreach (var (teamInfo, lineInfo) in SystemAPI.Query<RefRW<TeamInfo>, RefRW<LineInfo>>())
        {
            //UnityEngine.Debug.Log($"Setting BucketPasser at index {indexInLine} to team {teams[teamIndex]}");
            teamInfo.ValueRW.team = teams[teamIndex];
            lineInfo.ValueRW.index = indexInLine;
            indexInLine++;
            if (indexInLine >= config.bucketPassersPerTeam)
            {
                indexInLine = 0;
                teamIndex++;
                if (teamIndex >= config.numberOfTeams)
                {
                    break;
                }
            }
        }
        
        // set team Entity to all BucketFetchers
        teamIndex = 0;
        int bucketFetcherIndex = 0;
        foreach (var bucketFetcher in m_BucketFetcherQuery.ToEntityArray(Allocator.Temp))
        {
            //UnityEngine.Debug.Log($"Setting BucketFetcher to team {teams[teamIndex]}");
            state.EntityManager.SetComponentData(bucketFetcher, new TeamInfo() { team = teams[teamIndex]});

            bucketFetcherIndex++;
            if (bucketFetcherIndex >= config.bucketFetchersPerTeam)
            {
                bucketFetcherIndex = 0;
                teamIndex++;
                if (teamIndex >= config.numberOfTeams)
                {
                    break;
                }
            }
        }
*/

        // set random position to workers
        foreach (var (transform, position) in SystemAPI.Query<TransformAspect, RefRW<Position>>())
        {
            var entityPosition = new float3(random.NextFloat(0f, (float)config.gridSize), 0f, random.NextFloat(0f, (float)config.gridSize));
            transform.LocalPosition = entityPosition;
            position.ValueRW.position = new float2(entityPosition.x, entityPosition.z);
        }
        
        // create water cells
        var waterCells = CollectionHelper.CreateNativeArray<Entity>(config.waterCellCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.waterCellPrefab, waterCells);
        
        // set empty bucket colour
        foreach (var waterCell in waterCells)
        {
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(waterCell,
                colourQueryMask,
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.waterCellColour });
        }
        
        // set position and water amount on water cells
        foreach (var (transform, position, waterAmount) in SystemAPI.Query<TransformAspect, RefRW<Position>, RefRW<WaterAmount>>())
        {
            var entityPosition = new float3(random.NextFloat(-1f, (float)config.gridSize) + 1, 0f, random.NextFloat(-1f, (float)config.gridSize + 1f));
            var fixX = random.NextBool();
            entityPosition.x = fixX ? (random.NextBool() ? -1f : config.gridSize + 1f) : random.NextFloat(-1f, (float)config.gridSize) + 1f;
            entityPosition.z = fixX ? random.NextFloat(-1f, (float)config.gridSize) + 1f : (random.NextBool() ? -1f : config.gridSize + 1f);
            transform.LocalPosition = entityPosition;
            position.ValueRW.position = new float2(entityPosition.x, entityPosition.z);
            waterAmount.ValueRW.currentContain = config.maxWaterCellWaterAmount;
            waterAmount.ValueRW.maxContain = config.maxWaterCellWaterAmount;
        }
        
        // create buckets
        var buckets = CollectionHelper.CreateNativeArray<Entity>(config.bucketCount, Allocator.Temp);
        state.EntityManager.Instantiate(config.bucketPrefab, buckets);

        // set empty bucket colour
        foreach (var bucket in buckets)
        {
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(bucket,
                colourQueryMask,
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.fullBucketColour });
        }
        
        // initialize buckets with max contain
        foreach (var (waterAmount, pickedUpTag) in SystemAPI.Query<RefRW<WaterAmount>, BucketTag>())
        {
            waterAmount.ValueRW.currentContain = 0;
            waterAmount.ValueRW.maxContain = config.maxBucketAmount;
        }
        
        // set random position to buckets
        foreach (var (transform, position, pickedUpTag) in SystemAPI.Query<TransformAspect, RefRW<Position>, BucketTag>())
        {
            var entityPosition = new float3(random.NextFloat(0f, (float)config.gridSize), 0f, random.NextFloat(0f, (float)config.gridSize));
            transform.LocalPosition = entityPosition;
            position.ValueRW.position = new float2(entityPosition.x, entityPosition.z);
        }

        // create flame cells
        var flameCells = CollectionHelper.CreateNativeArray<Entity>(config.gridSize * config.gridSize, Allocator.Temp);
        state.EntityManager.Instantiate(config.flameCellPrefab, flameCells);
        
        // set default fire temperature to all flamecells
        foreach (var flameCell in flameCells)
        {
            // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
            ecb.SetComponentForLinkedEntityGroup(flameCell,
                colourQueryMask,
                new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.defaultTemperatureColour });
        }

        // set the indices to all cells and modify their position & TransformAspect
        int row = 0;
        int column = 0;
        foreach (var (cellInfo, transform, position) in SystemAPI.Query<RefRW<CellInfo>, TransformAspect, RefRW<Position>>())
        {
            cellInfo.ValueRW.indexX = row;
            cellInfo.ValueRW.indexY = column;
            transform.LocalPosition = new float3(row, -1f, column);
            position.ValueRW.position = new float2(row, column);
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

        state.Enabled = false;
    }
}
