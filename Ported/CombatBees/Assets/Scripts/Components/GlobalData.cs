using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct GlobalData : IComponentData
{
    public Entity BeePrefab;
    public Entity FoodPrefab;
    public Entity GibletPrefab;
    public Entity ExplosionPrefab;
    public int StartingFoodCount;
    public int BeeCount;
    public int BeeExplosionCount;
    public float3 BoundsMin;
    public float3 BoundsMax;
    public float3 BlueHiveCenter;
    public float3 YellowHiveCenter;
    public float HiveDepth;
    public float MinimumSpeed;
    public float3 TurnbackZone;
    public float TurnbackWidth;
    public float3 FlutterMagnitude;
    public float3 FlutterInterval;
    public float DecayTime;
    public float TimeBetweenIdleUpdates;
}
