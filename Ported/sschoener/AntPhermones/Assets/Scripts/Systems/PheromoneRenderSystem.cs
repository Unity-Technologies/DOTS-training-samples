using System;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class PheromoneRenderSystem : ComponentSystem
{
    EntityQuery m_PheromoneQuery;
    EntityQuery m_PheromoneRendererQuery;
    
    Texture2D m_PheromoneTexture;
    Material m_PheromoneMaterial;
    Color[] m_Colors;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_PheromoneQuery = GetEntityQuery(ComponentType.ReadOnly<PheromoneBuffer>());
        m_PheromoneRendererQuery = GetEntityQuery(ComponentType.ReadOnly<PheromoneRenderData>());
    }

    protected override void OnUpdate()
    {
        if (m_PheromoneTexture == null)
        {
            var pheromoneRenderer = EntityManager.GetSharedComponentData<PheromoneRenderData>(m_PheromoneRendererQuery.GetSingletonEntity());
            var map = GetSingleton<MapSettingsComponent>();
            m_PheromoneTexture = new Texture2D(map.MapSize, map.MapSize);
            m_PheromoneTexture.wrapMode = TextureWrapMode.Mirror;
            m_PheromoneMaterial = new Material(pheromoneRenderer.Material);
            m_PheromoneMaterial.mainTexture = m_PheromoneTexture;
            pheromoneRenderer.Renderer.sharedMaterial = m_PheromoneMaterial;
            m_Colors = new Color[m_PheromoneTexture.width * m_PheromoneTexture.height];
        }
        
        var pheromoneBuffer = EntityManager.GetBuffer<PheromoneBuffer>(m_PheromoneQuery.GetSingletonEntity());
        for (int i = 0; i < pheromoneBuffer.Length; i++)
            m_Colors[i].r = pheromoneBuffer[i];
        m_PheromoneTexture.SetPixels(m_Colors);
        m_PheromoneTexture.Apply();
    }
}