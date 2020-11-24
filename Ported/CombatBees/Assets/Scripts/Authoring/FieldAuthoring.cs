using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct FieldAuthoring : IComponentData
{
    public float3 size;
    public float gravity;
}
