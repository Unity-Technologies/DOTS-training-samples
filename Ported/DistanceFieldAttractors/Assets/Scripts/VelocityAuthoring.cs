using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
[WriteGroup(typeof(LocalToWorld))]
public struct Velocity : IComponentData
{
    public float3 Value;
}
