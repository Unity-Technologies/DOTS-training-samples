using System;
using TMPro;
using Unity.Core;
using Unity.Entities;
using UnityEngine;

[CreateAfter(typeof(Spawner))]
public partial struct Rendering: ISystem
{
    const string k_PheromoneTrailGameObjectPath = "PheromoneTrail";
    const string k_TotalFoodTextGameObjectPath = "Canvas/foodCount";
    const string k_FoodRateTextGameObjectPath = "Canvas/foodRate";
    const string k_HighestFoodRateTextGameObjectPath = "Canvas/highestFoodRate";
    int m_MapSize;
    bool m_Initialized;
    Colony m_Colony;
    
    double m_FoodRateStartTime;
    int m_FoodRateStartFoodCount;
    float m_LastFoodRate;

    float m_HighestFoodRate;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Pheromone>();
        state.RequireForUpdate<Colony>();
        state.RequireForUpdate<Stats>();
        m_Initialized = false;
    }

    void Initialize(ref SystemState state)
    {
        if (m_Initialized)
            return;

        m_Colony = SystemAPI.GetSingleton<Colony>();
        m_MapSize = (int)m_Colony.mapSize;

        var gameObject = GameObject.Find(k_PheromoneTrailGameObjectPath);
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        var material = meshRenderer.material;
        var texture2D = new Texture2D(m_MapSize,m_MapSize, TextureFormat.RGFloat, false);
        material.mainTexture = texture2D;

        var transform = gameObject.GetComponent<Transform>();
        transform.localScale = new Vector3(m_MapSize, m_MapSize, 1);
        transform.localPosition = new Vector3(m_MapSize/2, m_MapSize/2, 0);

        m_FoodRateStartTime = SystemAPI.Time.ElapsedTime;
        m_FoodRateStartFoodCount = 0;
        m_LastFoodRate = 0;
        
        m_HighestFoodRate = PlayerPrefs.GetFloat("Ants.highestFoodRate", 0);
        
        m_Initialized = true;
    }

    public void OnUpdate(ref SystemState state)
    {
        Initialize(ref state);
      
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
        
        // Stats
        var stats = SystemAPI.GetSingleton<Stats>();
        var textMeshPro = GameObject.Find(k_TotalFoodTextGameObjectPath).GetComponent<TextMeshProUGUI>();
        textMeshPro.SetText($"Food Count: {stats.foodCount}");
        
        var currentTime =  SystemAPI.Time.ElapsedTime;
        if (currentTime - m_FoodRateStartTime > m_Colony.foodRateSampleDuration)
        {
            m_LastFoodRate = (float) ((stats.foodCount - m_FoodRateStartFoodCount) / (currentTime - m_FoodRateStartTime));
            m_FoodRateStartTime = currentTime;
            m_FoodRateStartFoodCount = stats.foodCount;

            if (m_LastFoodRate > m_HighestFoodRate)
            {
                m_HighestFoodRate = m_LastFoodRate;
                PlayerPrefs.SetFloat("Ants.highestFoodRate", m_HighestFoodRate);
            }
        }
        textMeshPro = GameObject.Find(k_FoodRateTextGameObjectPath).GetComponent<TextMeshProUGUI>();
        textMeshPro.SetText($"Food Rate: {m_LastFoodRate:F1}");
        
        textMeshPro = GameObject.Find(k_HighestFoodRateTextGameObjectPath).GetComponent<TextMeshProUGUI>();
        textMeshPro.SetText($"All-Time Best: {m_HighestFoodRate:F1}");
        
    }
}


