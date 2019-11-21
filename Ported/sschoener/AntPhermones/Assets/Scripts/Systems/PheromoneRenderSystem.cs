using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class PheromoneRenderSystem : JobComponentSystem
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

    protected override unsafe JobHandle OnUpdate(JobHandle inputDeps)
    {
        var pheromoneRenderer = EntityManager.GetSharedComponentData<PheromoneRenderData>(m_PheromoneRendererQuery.GetSingletonEntity());
        if (m_PheromoneTexture == null)
        {
            var map = GetSingleton<MapSettingsComponent>();
            m_PheromoneTexture = new Texture2D(map.MapSize, map.MapSize);
            m_PheromoneTexture.wrapMode = TextureWrapMode.Mirror;
            m_PheromoneMaterial = new Material(pheromoneRenderer.Material);
            m_PheromoneMaterial.mainTexture = m_PheromoneTexture;
            m_Colors = new Color[m_PheromoneTexture.width * m_PheromoneTexture.height];
        }
        pheromoneRenderer.Renderer.sharedMaterial = m_PheromoneMaterial;

        using (new ProfilerMarker("CopyColors").Auto())
        {
            fixed (Color* colors = m_Colors)
            {
                var pheromoneBuffer = EntityManager.GetBuffer<PheromoneBuffer>(m_PheromoneQuery.GetSingletonEntity());
                new CopyColorJob
                {
                    Pheromones = pheromoneBuffer,
                    Colors = colors
                }.Schedule(pheromoneBuffer.Length, 128, inputDeps).Complete();
            }
        }

        using (new ProfilerMarker("UpdateTexture").Auto())
        {
            m_PheromoneTexture.SetPixels(m_Colors);
            m_PheromoneTexture.Apply();
        }

        return default;
    }
    
    unsafe struct CopyColorJob : IJobParallelFor
    {
        [ReadOnly]
        public DynamicBuffer<PheromoneBuffer> Pheromones;
        [NativeDisableUnsafePtrRestriction]
        public Color* Colors;

        public void Execute(int index)
        {
            Colors[index].r = Pheromones[index];
        }
    }
}