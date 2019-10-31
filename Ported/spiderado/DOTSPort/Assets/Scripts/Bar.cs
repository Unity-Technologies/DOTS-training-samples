using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct Bar : IComponentData, IEquatable<Bar>
{
    public Entity point1;
    public Entity point2;

    public float length;
    //public Matrix4x4 matrix;
    public float3 prevDelta;
    public float3 minBounds;
    public float3 maxBounds;
    //public Color color;
    public float thickness;

    public bool Equals(Bar other) {
        return point1 == other.point1 
            && point2 == other.point2;
    }
}
