using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Linq;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using System;


public struct CurrentVertex : IBufferElementData
{
    float3 Value;
    public static implicit operator Vector3(CurrentVertex rhs) { return rhs.Value; }
    public static implicit operator CurrentVertex(Vector3 rhs) { return new CurrentVertex() { Value = rhs }; }
    public static implicit operator float3(CurrentVertex rhs) { return rhs.Value; }
    public static implicit operator CurrentVertex(float3 rhs) { return new CurrentVertex() { Value = rhs }; }
}

public struct PreviousVertex : IBufferElementData
{
    float3 Value;
    public static implicit operator Vector3(PreviousVertex rhs) { return rhs.Value; }
    public static implicit operator PreviousVertex(Vector3 rhs) { return new PreviousVertex() { Value = rhs }; }
    public static implicit operator float3(PreviousVertex rhs) { return rhs.Value; }
    public static implicit operator PreviousVertex(float3 rhs) { return new PreviousVertex() { Value = rhs }; }
}

public struct Force : IBufferElementData
{
    float3 Value;
    public static implicit operator Vector3(Force rhs) { return rhs.Value; }
    public static implicit operator Force(Vector3 rhs) { return new Force() { Value = rhs }; }
    public static implicit operator float3(Force rhs) { return rhs.Value; }
    public static implicit operator Force(float3 rhs) { return new Force() { Value = rhs }; }
}

public struct ClothComponent : IComponentData
{
    public float3 Gravity;
    public BlobAssetReference<ClothBlobAsset> constraints;
}

public class ClothRenderComponent : IComponentData
{
    public Mesh     Mesh;
    public Material Material;
}