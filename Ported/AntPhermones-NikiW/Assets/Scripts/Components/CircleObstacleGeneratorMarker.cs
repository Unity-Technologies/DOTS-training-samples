using System;
using Unity.Entities;
using UnityEngine;

/// <summary>
///     Denotes that the <see cref="ObstacleManagementSystem"/> will create an obstacle at this location.
///     Entity is destroyed once created.
/// </summary>
[GenerateAuthoringComponent]
public struct CircleObstacleGeneratorMarker : IComponentData
{
    [Range(1, 10)]
    public int ringCount;
    [Range(1f, 500f)]
    public float radius;
    [Range(0f, 1f)]
    public float obstaclesPerRingNormalized;
    public uint seed;
}
