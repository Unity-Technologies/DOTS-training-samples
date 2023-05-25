using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawningSystem : ISystem
{
    // TODO:  move these constants to GameSettings
    const float k_DefaultWorkerPosY = 0.25f;
    const float k_DefaultGridSize = 0.3f;
    private const float k_DefaultWaterFeatureDistanceFromGridEdge = k_DefaultGridSize * 2f;
    private const float k_AssumedWaterFeatureWidth = 5f; // TODO: can we read this from the prefab?

    private bool initialized;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireSpawner>();
        state.RequireForUpdate<TeamSpawner>();
        state.RequireForUpdate<WaterSpawner>();
        state.RequireForUpdate<BucketSpawner>();
        state.RequireForUpdate<GameSettings>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (initialized) return;
        
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();
        var random = Random.CreateFromIndex(0);
        
        InitHeatBuffer(ref state, ref gameSettings, ref random);
        SpawnFireCells(ref state, ref gameSettings);
        SpawnTeams(ref state, ref gameSettings, ref random);
        SpawnWater(ref state, ref gameSettings);
        SpawnBucket(ref state, ref gameSettings, ref random);

        initialized = true;
    }

    private void InitHeatBuffer(ref SystemState state, ref GameSettings gameSettings, ref Random random)
    {
        var size = gameSettings.Size;
        
        var settingsEntity = SystemAPI.GetSingletonEntity<GameSettings>();
        var buffer = state.EntityManager.AddBuffer<FireTemperature>(settingsEntity);
        buffer.Resize(size, NativeArrayOptions.ClearMemory);
        
        for (var i = 0; i < gameSettings.StartingFires; i++)
        {
            var fireIndex = random.NextInt(size);
            buffer[fireIndex] = 1f;
        }
    }

    void SpawnFireCells(ref SystemState state, ref GameSettings gameSettings)
    {
        var fireSpawner = SystemAPI.GetSingleton<FireSpawner>();
        var prefab = fireSpawner.Prefab;
        
        state.EntityManager.Instantiate(prefab, gameSettings.Size, Allocator.Temp);
    }

    void SpawnTeams(ref SystemState state, ref GameSettings gameSettings, ref Random random)
    {
        var teamSpawner = SystemAPI.GetSingleton<TeamSpawner>();
        var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        var workerPrefab = teamSpawner.WorkerPrefab;
        
        for (var i = 0; i < teamSpawner.NumberOfTeams; ++i)
        {
            var teamEntity = cmdBuffer.CreateEntity();
            var teamRunner = cmdBuffer.Instantiate(workerPrefab);
            cmdBuffer.AddComponent(teamRunner, new URPMaterialPropertyBaseColor()
            {
                Value = gameSettings.RunnerWorkerColor
            });
            cmdBuffer.AddComponent(teamRunner, new RunnerState()
            {
                State = RunnerStates.Idle
            });

            cmdBuffer.AddComponent(teamEntity, new TeamState()
            {
                Value = TeamStates.Idle,
                RunnerId = teamRunner
            });
            
            cmdBuffer.AddComponent(teamEntity, new TeamData()
            {
                FirePosition = gameSettings.RowsAndColumns / 2f * gameSettings.DefaultGridSize,
                WaterPosition = float2.zero
            });
            
            var teamMembers = cmdBuffer.AddBuffer<TeamMember>(teamEntity);
            var workersPerTeam = teamSpawner.WorkersPerTeam;
            var instances = new NativeArray<Entity>(workersPerTeam, Allocator.Temp);
            cmdBuffer.Instantiate(workerPrefab, instances);

            var workerState = new WorkerState()
            {
                Value = WorkerStates.Idle
            };
            for (var m = 0; m < workersPerTeam; ++m)
            {
                bool isFirstHalf = m < workersPerTeam / 2;
                var workerEntity = instances[m];
                teamMembers.Add(new TeamMember() { Value = workerEntity });
                cmdBuffer.AddComponent(workerEntity, workerState);
                cmdBuffer.AddComponent<NextPosition>(workerEntity);
                cmdBuffer.AddComponent(workerEntity, new URPMaterialPropertyBaseColor()
                {
                    Value = isFirstHalf ? gameSettings.WorkerFullColor : gameSettings.WorkerEmptyColor
                });
            }
        }
        
        SpawnOmniWorkers(ref cmdBuffer, teamSpawner, gameSettings);
        cmdBuffer.Playback(state.EntityManager);

        foreach (var workerTransform in SystemAPI.Query<RefRW<LocalTransform>>()
                     .WithAny<WorkerState, OmniState, RunnerState>())
        {
            var randomGridPos = random.NextFloat2(float2.zero, new float2(gameSettings.RowsAndColumns, gameSettings.RowsAndColumns));
            randomGridPos *= gameSettings.DefaultGridSize;
            workerTransform.ValueRW.Position = new float3(randomGridPos.x, k_DefaultWorkerPosY, randomGridPos.y);
        }
    }

    void SpawnOmniWorkers(ref EntityCommandBuffer cmdBuffer, TeamSpawner teamSpawner, GameSettings gameSettings)
    {
        var omniEntities = new NativeArray<Entity>(teamSpawner.OmniWorkers, Allocator.Temp);
        cmdBuffer.Instantiate(teamSpawner.WorkerPrefab, omniEntities);

        var omniState = new OmniState()
        { Value = OmniStates.Idle };
        cmdBuffer.AddComponent(omniEntities, omniState);

        var baseColor = new URPMaterialPropertyBaseColor()
        { Value = gameSettings.WorkerOmniColor };
        cmdBuffer.AddComponent(omniEntities, baseColor);
    }
    
    void SpawnWater(ref SystemState state, ref GameSettings gameSettings)
    {
        var waterSpawner = SystemAPI.GetSingleton<WaterSpawner>();
        var prefab = waterSpawner.Prefab;
            
        float gridHalfWidth = gameSettings.DefaultGridSize * gameSettings.RowsAndColumns / 2f;
        var gridCenter = new float3(gridHalfWidth, 0.0f, gridHalfWidth);
        float waterGroupDistanceFromCenter = gridHalfWidth + k_DefaultWaterFeatureDistanceFromGridEdge + (k_AssumedWaterFeatureWidth / 2);
        var prefabCount = 4; // One per side.
            
        var instances = state.EntityManager.Instantiate(prefab, prefabCount, Allocator.Temp);

        float angleRadians = 0f;
            
        Debug.Log($"Made {instances.Length} waters!");
        foreach (var entity in instances)
        {
            var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
            math.sincos(angleRadians, out var sin, out var cos);
            var offset = new float3(sin, 0f, cos) * waterGroupDistanceFromCenter;
            transform.ValueRW.Position = gridCenter + offset;
            transform.ValueRW = transform.ValueRW.RotateY(angleRadians);

            angleRadians += math.PI * 0.5f;
        }
    }
    
    void SpawnBucket(ref SystemState state, ref GameSettings gameSettings, ref Random random)
    {
        var bucketSpawner = SystemAPI.GetSingleton<BucketSpawner>();

        var bucketEntities = state.EntityManager.Instantiate(bucketSpawner.BucketPrefab, bucketSpawner.NumberOfBuckets, Allocator.Temp);
        var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
        var bucketData = new BucketData()
        {
            IsFull = false
        };
        for (var i = 0; i < bucketEntities.Length; ++i)
        {
            cmdBuffer.AddComponent(bucketEntities[i], bucketData);
            cmdBuffer.AddComponent(bucketEntities[i], new URPMaterialPropertyBaseColor()
            {
                Value = gameSettings.BucketEmptyColor
            });            
        }

        cmdBuffer.Playback(state.EntityManager);
        
        foreach (var bucketTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<BucketData>())
        {
            var randomGridPos = random.NextFloat2(float2.zero, new float2(gameSettings.RowsAndColumns, gameSettings.RowsAndColumns));
            randomGridPos *= gameSettings.DefaultGridSize;
            bucketTransform.ValueRW.Position = new float3(randomGridPos.x, k_DefaultWorkerPosY, randomGridPos.y);
        }
    }
}