using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
partial struct FireSpreadingSystem : ISystem
{
    TileGridConfig m_TileGridConfig;
    TileGrid m_TileGrid;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TileGridConfig>();
        state.RequireForUpdate<TileGrid>();
        state.RequireForUpdate<HeatBufferElement>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Better way to do this?
        m_TileGridConfig = SystemAPI.GetSingleton<TileGridConfig>();
        m_TileGrid = SystemAPI.GetSingleton<TileGrid>();
        var heatBuffer = state.EntityManager.GetBuffer<HeatBufferElement>(m_TileGrid.entity);

        var fireSpreadingJob = new FireSpreadingJob
        {
            HeatBuffer = heatBuffer,
            HeatIncreaseSpeed = m_TileGridConfig.HeatIncreaseSpeed,
            DeltaTime = state.Time.DeltaTime,
            GridSize = m_TileGridConfig.Size
        };

        state.Dependency = fireSpreadingJob.Schedule(state.Dependency);
    }
}

[BurstCompile]
partial struct FireSpreadingJob : IJob
{
    public DynamicBuffer<HeatBufferElement> HeatBuffer;
    public float HeatIncreaseSpeed;
    public float DeltaTime;
    public int GridSize;
    
    public void Execute()
    {
        int count = 0;

        var allocator = Allocator.Temp;
        NativeList<int> fireTilesIndices = new NativeList<int>(HeatBuffer.Length, allocator);
        foreach (HeatBufferElement heatElement in HeatBuffer)
        {
            var heat = heatElement.Heat;
            if (heat >= 0.1f)
            {
                fireTilesIndices.Add(count);
            }
            count++;
        }

        foreach (var fireTile in fireTilesIndices)
        {
            IncreaseHeatForElement(fireTile);
            
            // Spread to upper tile
            var upperTile = fireTile + GridSize;
            if (upperTile < HeatBuffer.Length)
            {
                IncreaseHeatForElement(upperTile);
            }

            // Spread to lower tile
            var lowerTile = fireTile - GridSize;
            if (lowerTile >= 0)
            {
                IncreaseHeatForElement(lowerTile);
            }

            // Spread to left tile
            var leftTile = fireTile - 1;
            if (leftTile >= 0)
            {
                IncreaseHeatForElement(leftTile);
            }

            // Spread to right tile
            var rightTile = fireTile + 1;
            if (rightTile < HeatBuffer.Length)
            {
                IncreaseHeatForElement(rightTile);
            }
        }
    }
    
    void IncreaseHeatForElement(int tileIndex)
    {
        var heat = HeatBuffer[tileIndex];

        if (heat.Heat < 1.0f)
        {
            heat.Heat += DeltaTime * HeatIncreaseSpeed;
            HeatBuffer[tileIndex] = heat;
        }
    }
}