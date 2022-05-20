using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct TileRenderingSystem : ISystem
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
        m_TileGridConfig = SystemAPI.GetSingleton<TileGridConfig>();
        m_TileGrid = SystemAPI.GetSingleton<TileGrid>();

        var heatBuffer = state.EntityManager.GetBuffer<HeatBufferElement>(m_TileGrid.entity);
        
        int count = 0;
        foreach (var tile in SystemAPI.Query<TileAspect>().WithAll<Combustable>())
        {
            float heat = heatBuffer[count].Heat;
  
            if (heat < 0.01f)
            {
                // Render tile as grass
                tile.BaseColor.ValueRW.Value = m_TileGridConfig.GrassColor;
            } else if (heat < 0.3f)
            {
                // Render as light fire
                tile.BaseColor.ValueRW.Value = m_TileGridConfig.LightFireColor;
            } else if (heat < 0.7f)
            {
                // Render as medium fire
                tile.BaseColor.ValueRW.Value = m_TileGridConfig.MediumFireColor;
            }
            else
            {
                // Render as intense fire
                tile.BaseColor.ValueRW.Value = m_TileGridConfig.IntenseFireColor;
            }
            
            tile.Scale.ValueRW.Value = new float3(m_TileGridConfig.CellSize, heat*m_TileGridConfig.HeatScalingFactor + 0.1f, m_TileGridConfig.CellSize);
            
            count++;
        }
    }
}