using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PhysicsData : IComponentData
{
    public float3 a;
    public float3 v;
    public float floor;
}