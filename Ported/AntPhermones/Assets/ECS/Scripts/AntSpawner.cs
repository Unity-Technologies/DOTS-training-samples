using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ObstaclePosition : IBufferElementData
{
    public float3 Value;
}

struct AntSpawner : IComponentData
{
    public float3 Origin;
    public float3 ColonyPosition;
    public float3 FoodPosition;
    public Entity AntPrefab;
    public Entity ColonyPrefab;
    public Entity FoodPrefab;
    public Entity ObstaclePrefab;
    public int NbAnts;
    public int ObstacleRingCount;
    public float ObstacleRadius;
    public float ObstaclesPerRing;
    public float MapSize;
    public float GoalSteerStrength;
    public float FoodRadius;
    public float ColonyRadius;
}
