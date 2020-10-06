using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct Position : IComponentData
{
    public float3 Value;
}