using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SizeComponent : IComponentData
{
    public float3 Value;
}
