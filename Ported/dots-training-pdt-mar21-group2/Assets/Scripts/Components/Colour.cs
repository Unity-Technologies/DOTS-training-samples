using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Colour : IComponentData
{
    public float4 Value;
}
