using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
public struct Color : IComponentData
{
    public float4 Value;
}

[GenerateAuthoringComponent]
public struct ColorAuthoring : IComponentData
{
    public UnityEngine.Color Color;
}
