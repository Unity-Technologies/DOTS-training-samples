using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

/// <summary>
///     Singleton data for Ants.
/// </summary>
[NoAlias]
[GenerateAuthoringComponent]
public struct AntSimulationParams : IComponentData
{
    [Range(0f, 500_000f)]
    public int antCount;
    public int mapSize;
    public Vector3 antSize;
    public float antSpeedSearching;
    public float antSpeedHoldingFood;
    public float pheromoneAddSpeedWithFood;
    public float pheromoneAddSpeedWhenSearching;
    [Range(0f, 1f)]
    public float pheromoneDecay;
    public float randomSteeringStrength;
    public float pheromoneSteerStrengthWithFood;
    public float pheromoneSteerStrengthWhenSearching;
    public float wallSteerStrength;
    public float unknownFoodResourceSteerStrength;
    public float seenTargetSteerStrength;
    public float colonySteerStrength;
    public int antRotationResolution;
    public int obstacleRingCount;
    public float obstacleRadius;
    public bool renderAnts;
    public bool renderObstacles;
    public bool addWallsToTexture;
    public bool renderTargets;
    public ushort ticksForAntToDie;
    public float colonyRadius;
    public Entity npcAntPrefab;
    public Entity playerAntPrefab;
    public Entity obstaclePrefab;
    public Entity antSimulationRuntimeDataPrefab;
    public Entity foodPheromonesPrefab;
    public Entity colonyPheromonesPrefab;
    
    public static AntSimulationParams Default = new AntSimulationParams
    {
        mapSize = 128,
        antSize = new Vector3(0.008f, 0.004f, 0.004f),
        antSpeedSearching = 0.2f,
        antSpeedHoldingFood = 0.15f,
        pheromoneAddSpeedWithFood = 0.05f,
        pheromoneAddSpeedWhenSearching = 0.01f,
        pheromoneDecay = 0.99f,
        randomSteeringStrength = 0.3f,
        pheromoneSteerStrengthWithFood = 0.1f,
        pheromoneSteerStrengthWhenSearching = 0.05f,
        wallSteerStrength = 0.12f,
        unknownFoodResourceSteerStrength = 0.01f,
        seenTargetSteerStrength = 1,
        colonySteerStrength = 0.05f,
        antRotationResolution = 360,
        obstacleRingCount = 3,
        obstacleRadius = 2,
        renderAnts = true,
        renderObstacles = false,
        addWallsToTexture = true,
        renderTargets = true,
        ticksForAntToDie = 5000,
        colonyRadius = 3,
    };

}