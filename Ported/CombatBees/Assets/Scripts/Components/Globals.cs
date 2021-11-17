using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Globals : IComponentData
{
    public Entity BeePrefab;
    public Entity FoodPrefab;
    public Entity GibletPrefab;
    public int StartingFoodCount;
    public float3 BoundsMin;
    public float3 BoundsMax;
    public float3 BlueHiveCenter;
    public float3 YellowHiveCenter;
    public float HiveDepth;
}
