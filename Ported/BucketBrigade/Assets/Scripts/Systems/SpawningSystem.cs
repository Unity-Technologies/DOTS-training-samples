using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct SpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TileGridConfig>();
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var random = new Random((uint)UnityEngine.Random.Range(1, 100000));
        var tileGridConfig = SystemAPI.GetSingleton<TileGridConfig>();
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;

        for (int idx = 0; idx < config.NbOfTeams; idx++)
        {
            var workersFull = CollectionHelper.CreateNativeArray<Entity>(config.WorkerFullPerTeamCount, allocator);
            ecb.Instantiate(config.WorkerFullPrefab, workersFull);
        
            var workersEmpty = CollectionHelper.CreateNativeArray<Entity>(config.WorkerEmptyPerTeamCount, allocator);
            ecb.Instantiate(config.WorkerEmptyPrefab, workersEmpty);

            var fetchers = CollectionHelper.CreateNativeArray<Entity>(config.FetcherPerTeamCount, allocator);
            ecb.Instantiate(config.FetcherPrefab, fetchers);

            foreach (var workerFull in workersFull)
            {
                var randomRow = random.NextInt(0, tileGridConfig.Size);
                var randomColumn = random.NextInt(0, tileGridConfig.Size);
            
                ecb.SetComponent(workerFull, new Translation { Value = new float3(randomRow * tileGridConfig.CellSize, 0, randomColumn * tileGridConfig.CellSize) });
                ecb.AddComponent(workerFull, new Team { TeamNb = idx });
            }
        
            foreach (var workerEmpty in workersEmpty)
            {
                var randomRow = random.NextInt(0, tileGridConfig.Size);
                var randomColumn = random.NextInt(0, tileGridConfig.Size);
            
                ecb.SetComponent(workerEmpty, new Translation { Value = new float3(randomRow * tileGridConfig.CellSize, 0, randomColumn * tileGridConfig.CellSize) });
                ecb.AddComponent(workerEmpty, new Team { TeamNb = idx });
            } 
            
            foreach (var fetcher in fetchers)
            {
                var randomRow = random.NextInt(0, tileGridConfig.Size);
                var randomColumn = random.NextInt(0, tileGridConfig.Size);
            
                ecb.SetComponent(fetcher, new Translation { Value = new float3(randomRow * tileGridConfig.CellSize, 0, randomColumn * tileGridConfig.CellSize) });
                ecb.AddComponent(fetcher, new Team { TeamNb = idx });
            }
        }
        
        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}