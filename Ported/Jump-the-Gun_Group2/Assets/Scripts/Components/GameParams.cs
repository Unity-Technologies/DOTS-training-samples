using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Hybrid.Internal;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct GameParams : IComponentData
{
    public Entity TilePrefab;
    public Entity CannonBallPrefab;
    public float TerrainMin;
    public float TerrainMax;
    public int2 TerrainDimensions;
    public Entity CannonPrefab;
    public Entity CannonBarrel;
    public int CannonCount;
    public float CannonCooldown;
    public float4 colorA;
    public float4 colorB;
    public Entity PlayerPrefab;
    public float collisionStepMultiplier;
    public float playerParabolaPrecision;
}
