using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public struct BarElement : IBufferElementData
{
    public static implicit operator Vector2Int(BarElement e) { return e.Value; }
    public static implicit operator BarElement(Vector2Int e) { return new BarElement { Value = e }; }

    public Vector2Int Value;
};

public struct BarLengthElement : IBufferElementData
{
    public static implicit operator float(BarLengthElement e) { return e.Value; }
    public static implicit operator BarLengthElement(float e) { return new BarLengthElement { Value = e }; }

    public float Value;
};

public struct VertexStateCurrentElement : IBufferElementData
{
    public static implicit operator float3(VertexStateCurrentElement e) { return e.Value; }
    public static implicit operator Vector3(VertexStateCurrentElement e) { return e.Value; }
    public static implicit operator VertexStateCurrentElement(float3 e) { return new VertexStateCurrentElement { Value = e }; }
    public static implicit operator VertexStateCurrentElement(Vector3 e) { return new VertexStateCurrentElement { Value = e }; }

    public float3 Value;
};

public struct VertexStateOldElement : IBufferElementData
{
    public static implicit operator float3(VertexStateOldElement e) { return e.Value; }
    public static implicit operator Vector3(VertexStateOldElement e) { return e.Value; }
    public static implicit operator VertexStateOldElement(float3 e) { return new VertexStateOldElement { Value = e }; }
    public static implicit operator VertexStateOldElement(Vector3 e) { return new VertexStateOldElement { Value = e }; }
    
    public float3 Value;
};

<<<<<<< HEAD
public struct ClothInstance : IComponentData {
    public float4x4 worldToLocalMatrix;
    public float localY0;
};

public struct ClothConstraint
{
    public ushort x;
    public ushort y;
    public ushort pinPair; // bbbbbbbbbbbbbbb | pair.x | pair.y
    public ushort length;  // 8.8
};

public struct ClothBarSimEcs : ISharedComponentData, IEquatable<ClothBarSimEcs> {
    public NativeArray<ClothConstraint> constraints;
    public NativeArray<byte> pinState;

    public override int GetHashCode()
    {
        int h0 = constraints.GetHashCode();
        int h1 = pinState.GetHashCode();

        return h0 * 31771 ^ h1;
=======
public struct ClothBarSimEcs : ISharedComponentData, IEquatable<ClothBarSimEcs> {
    public NativeArray<Vector2Int> bars;
    public NativeArray<float> barLengths;
    public NativeArray<int> pins;

    public override int GetHashCode()
    {
        int h0 = bars.GetHashCode();
        int h1 = barLengths.GetHashCode();
        int h2 = pins.GetHashCode();

        return h0 ^ h1 ^ h2;
>>>>>>> JobClothSim cleanup
    }

    public bool Equals(ClothBarSimEcs other)
    {
        return GetHashCode() == other.GetHashCode();
    }    
};
