using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AABB : IComponentData
{
    public float3 center;
    public float3 halfSize;
}
