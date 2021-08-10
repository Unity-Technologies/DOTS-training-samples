using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

/// <summary>
///     Singleton gameplay param data for the AntSimulation.
/// </summary>
[NoAlias]
[GenerateAuthoringComponent]
public struct AntSimulationParams : IComponentData
{
    [GhostField]
    [Range(0f, 500_000f)]
    public int antCount;
    [GhostField]
    public int mapSize;
    [GhostField]
    public float3 antSize;
    [GhostField]
    public float antSpeedSearching;
    [GhostField]
    public float antSpeedHoldingFood;
    [GhostField]
    public float pheromoneAddSpeedWithFood;
    [GhostField]
    public float pheromoneAddSpeedWhenSearching;
    [GhostField]
    [Range(0f, 1f)]
    public float pheromoneDecay;
    [GhostField]
    public float randomSteeringStrength;
    [GhostField]
    public float pheromoneSteerStrengthWithFood;
    [GhostField]
    public float pheromoneSteerStrengthWhenSearching;
    [GhostField]
    public float wallSteerStrength;
    [GhostField]
    public float seenTargetSteerStrength;
    [GhostField]
    public float colonySteerStrength;
    [GhostField]
    public int antRotationResolution;
    [GhostField]
    public float obstacleRadius;
    [GhostField]
    public bool renderAnts;
    [GhostField]
    public bool addWallsToTexture;
    [GhostField]
    public bool renderTargets;
    [GhostField]
    public ushort ticksForAntToDie;
    [GhostField]
    public float colonyRadius;


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
        seenTargetSteerStrength = 1,
        colonySteerStrength = 0.05f,
        antRotationResolution = 360,
        obstacleRadius = 2,
        renderAnts = true,
        addWallsToTexture = true,
        renderTargets = true,
        ticksForAntToDie = 5000,
        colonyRadius = 3
    };
}