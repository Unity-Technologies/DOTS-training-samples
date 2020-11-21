using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct FieldAuthoring : IComponentData
{
    public float3 size;
    public float gravity;
}
