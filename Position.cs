using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct Position : IComponentData
{
    public float3 position;
}