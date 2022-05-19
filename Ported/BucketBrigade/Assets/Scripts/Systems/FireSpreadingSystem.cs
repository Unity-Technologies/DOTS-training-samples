using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
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
        
        int count = 0;

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        NativeList<int> fireTiles = new NativeList<int>(heatBuffer.Length, allocator);
        foreach (HeatBufferElement heatElement in heatBuffer)
        {
            var heat = heatElement.Heat;
            if (heat >= 0.1f)
            {
                fireTiles.Add(count);
            }
            count++;
        }

        float heatIncreaseSpeed = m_TileGridConfig.HeatIncreaseSpeed;
        void IncreaseHeatForElement(ref SystemState state, int tileIndex)
        {
            var heat = heatBuffer[tileIndex];

            if (heat.Heat < 1.0f)
            {
                heat.Heat += state.Time.DeltaTime * heatIncreaseSpeed;
                heatBuffer[tileIndex] = heat;
            }
        }
        
        foreach (var fireTile in fireTiles)
        {
            IncreaseHeatForElement(ref state, fireTile);
            
            // Spread to upper tile
            var upperTile = fireTile + m_TileGridConfig.Size;
            if (upperTile < heatBuffer.Length)
            {
                IncreaseHeatForElement(ref state, upperTile);
            }

            // Spread to lower tile
            var lowerTile = fireTile - m_TileGridConfig.Size;
            if (lowerTile >= 0)
            {
                IncreaseHeatForElement(ref state, lowerTile);
            }

            // Spread to left tile
            var leftTile = fireTile - 1;
            if (leftTile >= 0)
            {
                IncreaseHeatForElement(ref state, leftTile);
            }

            // Spread to right tile
            var rightTile = fireTile + 1;
            if (rightTile < heatBuffer.Length)
            {
                IncreaseHeatForElement(ref state, rightTile);
            }
        }
    }
}