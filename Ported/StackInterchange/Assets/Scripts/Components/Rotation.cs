using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct Rotation : IComponentData
{
    public float4 Value;
}