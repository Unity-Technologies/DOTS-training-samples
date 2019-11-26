using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Material obstacleMaterial;
    public Material resourceMaterial;
    public Material colonyMaterial;
    public Mesh obstacleMesh;
    public Mesh colonyMesh;
    public Mesh resourceMesh;
    public int mapSize = 128;
    public int bucketResolution;
    public int obstacleRingCount;
    [Range(0f, 1f)]
    public float obstaclesPerRing;
    public float obstacleRadius;

    Matrix4x4[][] m_ObstacleMatrices;
    float2[,][] m_ObstacleBuckets;
    Matrix4x4 m_ResourceMatrix;
    Matrix4x4 m_ColonyMatrix;
    
    float2[] m_Obstacles;
    public float2[] Obstacles
    {
        get
        {
            if (m_Obstacles == null)
                GenerateObstacles();
            return m_Obstacles;
        }
    }

    public float2[,][] ObstacleBuckets
    {
        get
        {
            if (m_ObstacleBuckets == null)
                GenerateObstacles();
            return m_ObstacleBuckets;
        }
    }
    
    public Vector2 ColonyPosition => 0.5f * mapSize * Vector2.one;
    
    Vector2 m_ResourcePosition;
    bool m_HasResourcePosition;
    public Vector2 ResourcePosition
    {
        get
        {
            if (!m_HasResourcePosition)
            {
                Random.InitState(5);
                float resourceAngle = Random.value * 2f * Mathf.PI;
                var v = .475f * mapSize * new Vector2(Mathf.Cos(resourceAngle), Mathf.Sin(resourceAngle));
                m_ResourcePosition = .5f * mapSize * Vector2.one + v; 
            }
            m_HasResourcePosition = true;
            return m_ResourcePosition;
        }
    }

    const int k_InstancesPerBatch = 1023;

    void GenerateObstacles()
    {
        Random.InitState(0);
        var output = m_Obstacles = MapGeneration.GenerateObstaclePositions(mapSize, obstacleRingCount, obstaclesPerRing, obstacleRadius).ToArray();
        m_ObstacleMatrices = new Matrix4x4[Mathf.CeilToInt((float)output.Length / k_InstancesPerBatch)][];
        for (int i = 0; i < m_ObstacleMatrices.Length; i++)
        {
            m_ObstacleMatrices[i] = new Matrix4x4[Mathf.Min(k_InstancesPerBatch, output.Length - i * k_InstancesPerBatch)];
            for (int j = 0; j < m_ObstacleMatrices[i].Length; j++)
            {
                m_ObstacleMatrices[i][j] = Matrix4x4.TRS(new float3(output[i * k_InstancesPerBatch + j] / mapSize, 0), Quaternion.identity, new Vector3(obstacleRadius * 2f, obstacleRadius * 2f, 1f) / mapSize);
            }
        }

        List<float2>[,] tempObstacleBuckets = new List<float2>[bucketResolution, bucketResolution];

        for (int x = 0; x < bucketResolution; x++)
        {
            for (int y = 0; y < bucketResolution; y++)
            {
                tempObstacleBuckets[x, y] = new List<float2>();
            }
        }

        for (int i = 0; i < m_Obstacles.Length; i++)
        {
            Vector2 pos = m_Obstacles[i];

            int startX = Mathf.FloorToInt((pos.x - obstacleRadius) / mapSize * bucketResolution);
            int endX = Mathf.FloorToInt((pos.x + obstacleRadius) / mapSize * bucketResolution);
            for (int x = startX; x <= endX; x++)
            {
                if (x < 0 || x >= bucketResolution)
                {
                    continue;
                }
                int startY = Mathf.FloorToInt((pos.y - obstacleRadius) / mapSize * bucketResolution);
                int endY = Mathf.FloorToInt((pos.y + obstacleRadius) / mapSize * bucketResolution);
                for (int y = startY; y <= endY; y++)
                {
                    if (y < 0 || y >= bucketResolution)
                    {
                        continue;
                    }
                    tempObstacleBuckets[x, y].Add(m_Obstacles[i]);
                }
            }
        }

        m_ObstacleBuckets = new float2[bucketResolution, bucketResolution][];
        for (int x = 0; x < bucketResolution; x++)
        {
            for (int y = 0; y < bucketResolution; y++)
            {
                m_ObstacleBuckets[x, y] = tempObstacleBuckets[x, y].ToArray();
            }
        }
    }

    void Start()
    {
        m_ColonyMatrix = Matrix4x4.TRS(ColonyPosition / mapSize, Quaternion.identity, new Vector3(4f, 4f, .1f) / mapSize);
        m_ResourceMatrix = Matrix4x4.TRS(ResourcePosition / mapSize, Quaternion.identity, new Vector3(4f, 4f, .1f) / mapSize);
    }

    int m_NumUpdates;
    SimulationSystemGroup m_SimulationSystemGroup;
    void FixedUpdate()
    {
        int updatesFinished = m_NumUpdates;
        m_NumUpdates++;
        if (updatesFinished == 1)
            return;
        if (m_SimulationSystemGroup == null)
            m_SimulationSystemGroup = World.AllWorlds[0].GetExistingSystem<SimulationSystemGroup>();
        m_SimulationSystemGroup.Update();
    }
    
    void Update()
    {
        HandleKeyboardInput();
        m_NumUpdates = 1;
        for (int i = 0; i < m_ObstacleMatrices.Length; i++)
        {
            Graphics.DrawMeshInstanced(obstacleMesh, 0, obstacleMaterial, m_ObstacleMatrices[i]);
        }

        Graphics.DrawMesh(colonyMesh, m_ColonyMatrix, colonyMaterial, 0);
        Graphics.DrawMesh(resourceMesh, m_ResourceMatrix, resourceMaterial, 0);
    }
    
    static void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 2f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 3f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = 4f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Time.timeScale = 5f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Time.timeScale = 6f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Time.timeScale = 7f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Time.timeScale = 8f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Time.timeScale = 9f;
        }
    }
}