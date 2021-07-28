using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
[GenerateAuthoringComponent]
public struct GameConfigComponent : IComponentData
{
    public int SimulationSize;
    public float WaterRefillRate;
    public float WaterMaxScale;
    public float MinBucketScale;
    public float MaxBucketScale;
    public int BucketCount;
    public Color32 EmptyBucketColor;
    public Color32 FullBucketColor;
    public Entity BucketPrefab;
    public Entity FlameCellPrefab;
    public Color FlameDefaultColor;
    public Color FlameBurnColor;
    public Color FlameColdColor;
    public int HeatTrasferRadius;
    public float HeatFallOff;
    public float FlashPoint;
    public int startingFireCount;
    public float FlameScaleMax;
    public int ChainsCount;
    public int ChainSize;
    public float ChainAssessPeriod;
    public Entity BotPrefab;
    public Color ScooperBotColor;
    public Color ThrowerBotColor;
    public Color PasserFullBotColor;
    public Color PasserEmptyBotColor;
    public float BotSpeed;
}
