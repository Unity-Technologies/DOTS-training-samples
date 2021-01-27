using System;

using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;

public struct Map : IComponentData
{
    public int2 dimensions;

    public Entity homePrefab;
    public float homeRadius;
    public float4 homeColor;

    public Entity foodPrefab;
    public float foodRadius;
    public float4 foodColor;
    public float2 foodLocation;

    public Entity obstaclePrefab;
    public float obstacleRadius;
    public float4 obstacleColor;

    public int numberOfRings;
    public float openingDegrees;
}
