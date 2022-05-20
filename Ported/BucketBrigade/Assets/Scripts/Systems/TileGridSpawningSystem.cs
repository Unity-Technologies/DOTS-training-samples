using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct TileGridSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TileGridConfig>();
        state.RequireForUpdate<TileGrid>();
    }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SpawnInnerTiles(ref state);
        SpawnOuterTiles(ref state);

        state.Enabled = false;
    }

    private void SpawnInnerTiles(ref SystemState state)
    {
        var tileGridConfig = SystemAPI.GetSingleton<TileGridConfig>();
        var tileGrid = SystemAPI.GetSingleton<TileGrid>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var tiles = CollectionHelper.CreateNativeArray<Entity>(tileGridConfig.Size * tileGridConfig.Size, allocator);
        ecb.Instantiate(tileGridConfig.TilePrefab, tiles);
        
        var heatBuffer = state.EntityManager.GetBuffer<HeatBufferElement>(tileGrid.entity);

        var random = new Random((uint)UnityEngine.Random.Range(1, 100000));
        var randomRow = random.NextInt(0, tileGridConfig.Size);
        var randomColumn = random.NextInt(0, tileGridConfig.Size);

        var rowCount = 0;
        var columnCount = 0;
        foreach (var tile in tiles)
        {
            var tilePosition = new int2(rowCount, columnCount);
            columnCount++;
            if (columnCount == tileGridConfig.Size)
            {
                rowCount++;
                columnCount = 0;
            }

            float heat = 0.0f;
            if (tilePosition.x == randomRow && tilePosition.y == randomColumn)
            {
                // Fire tile
                ecb.SetComponent(tile, new URPMaterialPropertyBaseColor { Value = tileGridConfig.LightFireColor });
                ecb.SetComponent(tile, new Tile { Position = tilePosition });
                ecb.SetComponent(tile, new NonUniformScale {Value = new float3(tileGridConfig.CellSize, 0.6f, tileGridConfig.CellSize)});
                heat = 0.5f;
            }
            else
            {
                // Grass tile
                ecb.SetComponent(tile, new URPMaterialPropertyBaseColor { Value = tileGridConfig.GrassColor });
                ecb.SetComponent(tile, new Tile { Position = tilePosition });
            }
            
            ecb.AddComponent<Combustable>(tile, new Combustable());
            ecb.SetComponent(tile, new Translation { Value = new float3(tilePosition.x * tileGridConfig.CellSize, 0, tilePosition.y * tileGridConfig.CellSize) });
            
            heatBuffer.Add(new HeatBufferElement { Heat = heat });
        }
    }
    
    private void SpawnOuterTiles(ref SystemState state)
    {
        // TODO: Optimize by spawning outer/inner tiles in one pass
        
        var tileGridConfig = SystemAPI.GetSingleton<TileGridConfig>();
        var tileGrid = SystemAPI.GetSingleton<TileGrid>();
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var tiles = CollectionHelper.CreateNativeArray<Entity>(tileGridConfig.NbOfWaterTiles, allocator);
        ecb.Instantiate(tileGridConfig.TilePrefab, tiles);

        var random = new Random((uint)UnityEngine.Random.Range(1, 100000));

        int innerSizeMin = -tileGridConfig.Spacing;
        int outerSizeMin = innerSizeMin - tileGridConfig.OuterSize;
        
        int innerSizeMax = tileGridConfig.Size + tileGridConfig.Spacing;
        int outerSizeMax = innerSizeMax + tileGridConfig.OuterSize;
        
        foreach (var tile in tiles)
        {
            // TODO: Optimize this to prevent long randoms
            int randomRow = 0;
            int randomColumn = 0;
            do
            {
                randomRow = random.NextInt(outerSizeMin, outerSizeMax);
                randomColumn = random.NextInt(outerSizeMin, outerSizeMax);
            } while (randomRow >= innerSizeMin && randomRow <= innerSizeMax &&
                     randomColumn >= innerSizeMin && randomColumn <= innerSizeMax);
            
            // TODO: Should handle overlaps
            var tilePosition = new int2(randomRow, randomColumn);
            
            // Water tile
            ecb.SetComponent(tile, new URPMaterialPropertyBaseColor { Value = tileGridConfig.IntenseWaterColor });
            ecb.SetComponent(tile, new Tile { Position = tilePosition, Water = 100.0f });
            ecb.SetComponent(tile, new NonUniformScale {Value = new float3(tileGridConfig.CellSize, 0.3f, tileGridConfig.CellSize)});
            ecb.SetComponent(tile, new Translation { Value = new float3(tilePosition.x, 0, tilePosition.y) });
            ecb.AddComponent<FetcherDropZone>(tile, new FetcherDropZone());
        }
    }
}