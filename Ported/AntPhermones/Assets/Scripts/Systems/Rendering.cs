using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = Unity.Mathematics.Random;

[CreateAfter(typeof(Spawner))]
public partial struct Rendering: ISystem
{
    const string k_PheromoneTrailGameObjectPath = "PheromoneTrail";
    int m_MapSize;
    bool m_Initialized;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Pheromone>();
        state.RequireForUpdate<Colony>();
        m_Initialized = false;
    }

    void Initialize()
    {
        if (m_Initialized)
            return;

        var colony = SystemAPI.GetSingleton<Colony>();
        m_MapSize = (int)colony.mapSize;

        var gameObject = GameObject.Find(k_PheromoneTrailGameObjectPath);
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        var material = meshRenderer.material;
        var texture2D = new Texture2D(m_MapSize,m_MapSize, TextureFormat.RFloat, false);
        material.mainTexture = texture2D;

        var transform = gameObject.GetComponent<Transform>();
        transform.localScale = new Vector3(m_MapSize, m_MapSize, 1);
        transform.localPosition = new Vector3(m_MapSize/2, m_MapSize/2, 0);

        m_Initialized = true;
    }

    public void OnUpdate(ref SystemState state)
    {
        Initialize();
      
        // Ant color
        var antRenderingJob = new AntRenderingJob();
        state.Dependency = antRenderingJob.Schedule(state.Dependency);

        // Pheromone trail
        var gameObject = GameObject.Find(k_PheromoneTrailGameObjectPath);
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        var material = meshRenderer.material;
        var texture2D = material.mainTexture as Texture2D;

        var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>();
        texture2D.SetPixelData(pheromones.AsNativeArray(), 0, 0);
        texture2D.Apply();
    }
}
