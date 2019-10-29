using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct Bar : IComponentData
{
    Point point1;
    Point point2;
    float length;
    Matrix4x4 matrix;
    float3 prevDelta;
    float3 minBounds;
    float3 maxBounds;
    Color color;
    float thickness;
}
