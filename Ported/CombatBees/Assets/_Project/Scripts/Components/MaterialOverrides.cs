using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[Serializable]
[MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
public struct OverridableMaterial_Color : IComponentData
{
    public float4 Value;
}

[Serializable]
[MaterialProperty("_Smoothness", MaterialPropertyFormat.Float)]
public struct OverridableMaterial_Smoothness : IComponentData
{
    public float Value;
}
