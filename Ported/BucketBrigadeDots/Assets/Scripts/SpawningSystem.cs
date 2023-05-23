using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
    
    private uint m_UpdateCounter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireSpawner>();
        state.RequireForUpdate<TeamSpawner>();
        state.RequireForUpdate<WaterSpawner>();
        state.RequireForUpdate<GameSettings>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();
        
        InitHeatBuffer(ref state, ref gameSettings);
        SpawnFireCells(ref state, ref gameSettings);
        SpawnTeams(ref state);
        SpawnWater(ref state);
    }

    private void InitHeatBuffer(ref SystemState state, ref GameSettings gameSettings)
    {
        var size = gameSettings.Size;
        
        var settingsEntity = SystemAPI.GetSingletonEntity<GameSettings>();
        var buffer = state.EntityManager.AddBuffer<FireTemperature>(settingsEntity);
        buffer.Resize(size, NativeArrayOptions.ClearMemory);

        var random = Random.CreateFromIndex(0);
        for (var i = 0; i < gameSettings.StartingFires; i++)
        {
            var fireIndex = random.NextInt(size);
            buffer[fireIndex] = 1f;
        }
    }

    void SpawnFireCells(ref SystemState state, ref GameSettings gameSettings)
    {
        var fireCellsQuery = SystemAPI.QueryBuilder().WithAll<FireCell>().Build();
        if (fireCellsQuery.IsEmpty)
        {
            var fireSpawner = SystemAPI.GetSingleton<FireSpawner>();
            var prefab = fireSpawner.Prefab;
            
            var instances = state.EntityManager.Instantiate(prefab, gameSettings.Size, Allocator.Temp);

            var index = 0;
            for (var x = 0; x < gameSettings.RowsAndColumns; x++)
            {
                for (var y = 0; y < gameSettings.RowsAndColumns; y++)
                {
                    var entity = instances[index++];
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                    transform.ValueRW.Position = new float3(x * k_DefaultGridSize, 0f, y * k_DefaultGridSize);

                }
            }
        }
    }

    void SpawnTeams(ref SystemState state)
    {
        var teamsQuery = SystemAPI.QueryBuilder().WithAll<TeamData>().Build();
        if (teamsQuery.IsEmpty)
        {
            var gameSetting = SystemAPI.GetSingleton<GameSettings>();
            var teamSpawner = SystemAPI.GetSingleton<TeamSpawner>();

            var random = Random.CreateFromIndex(0);
            var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            for (var i = 0; i < teamSpawner.NumberOfTeams; ++i)
            {
                var workersPerTeam = teamSpawner.WorkersPerTeam;
                var teamEntity = cmdBuffer.CreateEntity();
                cmdBuffer.AddComponent(teamEntity, new TeamData()
                {
                    FirePosition = gameSetting.RowsAndColumns / 2f * k_DefaultGridSize,
                    WaterPosition = float2.zero
                });
                cmdBuffer.AddComponent(teamEntity, new TeamState()
                {
                    Value = TeamStates.Idle
                });
                var teamMembers = cmdBuffer.AddBuffer<TeamMember>(teamEntity);
                
                var prefab = teamSpawner.WorkerPrefab;
                var instances = new NativeArray<Entity>(workersPerTeam, Allocator.Temp);
                cmdBuffer.Instantiate(prefab, instances);

                var workerState = new WorkerState()
                {
                    Value = WorkerStates.Idle
                };
                for (var m = 0; m < workersPerTeam; ++m)
                {
                    var workerEntity = instances[m];
                    teamMembers.Add(new TeamMember() { Value = workerEntity });
                    cmdBuffer.AddComponent(workerEntity, workerState);
                    cmdBuffer.AddComponent<NextPosition>(workerEntity);
                }
            }
            
            cmdBuffer.Playback(state.EntityManager);

            foreach (var workerTransform in SystemAPI.Query<RefRW<LocalTransform>>())
            {
                var randomGridPos = random.NextFloat2(float2.zero, new float2(gameSetting.RowsAndColumns, gameSetting.RowsAndColumns));
                randomGridPos *= k_DefaultGridSize;
                workerTransform.ValueRW.Position = new float3(randomGridPos.x, k_DefaultWorkerPosY, randomGridPos.y);
            }
        }
    }
    
    void SpawnWater(ref SystemState state)
    {
        var waterCellsQuery = SystemAPI.QueryBuilder().WithAll<WaterCell>().Build();
        if (waterCellsQuery.IsEmpty)
        {
            var gameSetting = SystemAPI.GetSingleton<GameSettings>();
            var waterSpawner = SystemAPI.GetSingleton<WaterSpawner>();
            var prefab = waterSpawner.Prefab;
            
            float gridHalfWidth = k_DefaultGridSize * gameSetting.RowsAndColumns / 2f;
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
    }
}