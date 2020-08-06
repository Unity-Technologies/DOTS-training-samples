using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class AntDefaults : MonoBehaviour
{
    public int antCount = 1000;
    public int mapSize = 128;
    public int bucketResolution = 64;
    public Vector3 antSize;
    public float antSpeed = 0.2f;
    
    [Range(0f,1f)]
    public float antAccel = 0.07f;
    public float trailAddSpeed = 0.3f;
    public float trailDecay = 0.9985f;
    public float randomSteering = 0.14f;
    public float pheromoneSteerStrength = 0.015f;
    public float wallSteerStrength = 0.12f;
    public float goalSteerStrength = 0.04f;
    public float outwardStrength = 0.003f;
    public float inwardStrength = 0.003f;
    public int rotationResolution = 360;
    public int obstacleRingCount = 3;
    public float obstaclesPerRing = 0.8f;
    public float obstacleRadius = 2.0f;

    public Texture2D colisionMap;
    public Texture2D pheromoneMap;

    [SerializeField] NativeArray<float>[] m_PheromoneMapBuffers = new NativeArray<float>[2];
    [SerializeField] int m_CurrentBuffer;
    int m_BufferSize;

    public int bufferSize => m_BufferSize;

    public NativeArray<float> GetCurrentPheromoneMapBuffer() => m_PheromoneMapBuffers[m_CurrentBuffer];

    public void SwapPheromoneBuffer()
    {
        int otherBuffer = (++m_CurrentBuffer) % 2;
        m_PheromoneMapBuffers[m_CurrentBuffer].CopyTo(m_PheromoneMapBuffers[otherBuffer]);
        pheromoneMap.LoadRawTextureData<float>(m_PheromoneMapBuffers[m_CurrentBuffer]);
        pheromoneMap.Apply();
        m_CurrentBuffer = otherBuffer;
    }

    public void InitPheromoneBuffers(int width, int height)
    {
        m_BufferSize = width * height;
        for (int i = 0; i < 2; ++i)
            m_PheromoneMapBuffers[i] = new NativeArray<float>(width * height, Allocator.Persistent);
        pheromoneMap.LoadRawTextureData<float>(m_PheromoneMapBuffers[0]);
        m_CurrentBuffer = 1;
    }
    
    public int PheromoneIndex(int x, int y)
    {
        return x + y * mapSize;
    }

    public float GetPheromoneAt(int x, int y)
    {
        int index = PheromoneIndex(x,y);
        return GetCurrentPheromoneMapBuffer()[index];
    }
    
    public void SetPheromoneAt(int x, int y,float value)
    {
        int index = PheromoneIndex(x,y);
        m_PheromoneMapBuffers[m_CurrentBuffer][index] = value;
    }
    
    public void IncPheromoneAt(int x, int y,float value)
    {
        int index = PheromoneIndex(x,y);
        m_PheromoneMapBuffers[m_CurrentBuffer][index] += value;
    }
    
    public void IncPheromoneAtWithClamp(int x, int y,float value, float clampValue)
    {
        int index = PheromoneIndex(x,y);
        m_PheromoneMapBuffers[m_CurrentBuffer][index] += value;
        if (m_PheromoneMapBuffers[m_CurrentBuffer][index]>clampValue) {
            m_PheromoneMapBuffers[m_CurrentBuffer][index] = clampValue;
        }
    }

    
    void OnDestroy()
    {
        for (int i = 0; i < 2; ++i)
            m_PheromoneMapBuffers[i].Dispose();
    }
}
