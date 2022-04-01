using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PositionComponent : IComponentData
{
    public float3 Position;
    public float3 SmoothPosition;
}