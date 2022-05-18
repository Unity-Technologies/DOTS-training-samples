using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
partial struct FireSpreadingSystem : ISystem
{
    float m_Timer;
    const float k_WaitTime = 2.0f;

    TileGridConfig m_TileGridConfig;
    TileGrid m_TileGrid;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TileGridConfig>();
        state.RequireForUpdate<TileGrid>();
        state.RequireForUpdate<HeatBufferElement>();
        
        m_Timer = 0.0f;
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

        m_Timer += state.Time.DeltaTime;

        if (m_Timer >= k_WaitTime)
        {
            m_Timer = 0.0f;
            
            foreach (var tile in SystemAPI.Query<TileRWAspect>())
            {
                if (tile.Heat > 0.0f && tile.Heat < 1.0f)
                {
                    tile.Heat += 0.1f;
                    
                    // Spread fire
                    // Check upper tile
                    if (tile.Position.x < m_TileGridConfig.Size - 1)
                    {
                        int upperTileRow = tile.Position.x + 1;
                        int upperTileIndex = upperTileRow * m_TileGridConfig.Size + tile.Position.y;
                        
                        var upperTileHeat = heatBuffer[upperTileIndex];
                        upperTileHeat.Heat = 0.1f;
                        heatBuffer[upperTileIndex] = upperTileHeat;
                    }
                    
                    // Check lower tile
                    if (tile.Position.x > 0)
                    {
                        
                    }
                    
                    // Check left tile
                    if (tile.Position.y > 0)
                    {
                        
                    }
                    
                    // Check right tile
                    if (tile.Position.y < m_TileGridConfig.Size - 1)
                    {
                        
                    }
                }
            }
        }
    }
}