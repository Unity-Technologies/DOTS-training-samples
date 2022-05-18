using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct TileRenderingSystem : ISystem
{
    ComponentDataFromEntity<URPMaterialPropertyBaseColor> m_URPMaterialPropertyBaseColorFromEntity;
    ComponentDataFromEntity<NonUniformScale> m_NonUniformScaleFromEntity;

    TileGridConfig m_TileGridConfig;
    TileGrid m_TileGrid;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TileGridConfig>();
        state.RequireForUpdate<TileGrid>();
        state.RequireForUpdate<HeatBufferElement>();

        m_URPMaterialPropertyBaseColorFromEntity = state.GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();
        m_NonUniformScaleFromEntity = state.GetComponentDataFromEntity<NonUniformScale>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_URPMaterialPropertyBaseColorFromEntity.Update(ref state);
        m_NonUniformScaleFromEntity.Update(ref state);
        
        int count = 0;
        foreach (var tile in SystemAPI.Query<TileROAspect>())
        {
            var heatBuffer = state.EntityManager.GetBuffer<HeatBufferElement>(m_TileGrid.entity);

            float heat = heatBuffer[count].Heat;

            var baseColor = m_URPMaterialPropertyBaseColorFromEntity[tile.Self];
            var scale = m_NonUniformScaleFromEntity[tile.Self];

            if (heat == 0.0f)
            {
                // Render tile as grass
                baseColor.Value = m_TileGridConfig.GrassColor;
                scale.Value.y = 0.1f;
            } else if (heat < 0.4f)
            {
                // Render as light fire
                baseColor.Value = m_TileGridConfig.LightFireColor;
                scale.Value.y = tile.Heat*5 + 0.1f;
            } else if (heat < 0.8f)
            {
                // Render as medium fire
                baseColor.Value = m_TileGridConfig.MediumFireColor;
                scale.Value.y = tile.Heat*5 + 0.1f;
            }
            else
            {
                // Render as intense fire
                baseColor.Value = m_TileGridConfig.IntenseFireColor;
                scale.Value.y = tile.Heat*5 + 0.1f;
            }
            
            m_URPMaterialPropertyBaseColorFromEntity[tile.Self] = baseColor;
            m_NonUniformScaleFromEntity[tile.Self] = scale;

            count++;
        }
    }
}