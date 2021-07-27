using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Experimental;
using UnityEngine;

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
}
