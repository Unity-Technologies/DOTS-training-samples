using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct FireSpreadingSystem : ISystem
{
    float m_Timer;
    const float k_WaitTime = 2.0f;

    ComponentDataFromEntity<URPMaterialPropertyBaseColor> m_URPMaterialPropertyBaseColorFromEntity;
    ComponentDataFromEntity<NonUniformScale> m_NonUniformScaleFromEntity;

    TileGridConfig m_TileGridConfig;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TileGridConfig>();
        
        m_Timer = 0.0f;

        m_URPMaterialPropertyBaseColorFromEntity = state.GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();
        m_NonUniformScaleFromEntity = state.GetComponentDataFromEntity<NonUniformScale>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Better way to do this?
        m_TileGridConfig = SystemAPI.GetSingleton<TileGridConfig>();
        
        m_URPMaterialPropertyBaseColorFromEntity.Update(ref state);
        m_NonUniformScaleFromEntity.Update(ref state);
        
        m_Timer += state.Time.DeltaTime;

        if (m_Timer >= k_WaitTime)
        {
            m_Timer = 0.0f;
            
            foreach (var tile in SystemAPI.Query<TileAspect>())
            {
                if (tile.Heat > 0.0f && tile.Heat < 1.0f)
                {
                    // Increase fire heat
                    float initialHeat = tile.Heat;
                    
                    tile.Heat += 0.1f;

                    var scale = m_NonUniformScaleFromEntity[tile.Self];
                    scale.Value.y = tile.Heat*5 + 0.1f;
                    m_NonUniformScaleFromEntity[tile.Self] = scale;

                    if (initialHeat < 0.4f)
                    {
                        var baseColor = m_URPMaterialPropertyBaseColorFromEntity[tile.Self];
                        baseColor.Value = m_TileGridConfig.MediumFireColor;
                        m_URPMaterialPropertyBaseColorFromEntity[tile.Self] = baseColor;
                    } else if (initialHeat < 0.8f)
                    {
                        var baseColor = m_URPMaterialPropertyBaseColorFromEntity[tile.Self];
                        baseColor.Value = m_TileGridConfig.IntenseFireColor;
                        m_URPMaterialPropertyBaseColorFromEntity[tile.Self] = baseColor;
                    }
                    
                    // Spread fire
                }
            }
        }
    }
}