using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BeeMovement : IComponentData
{
    public float3 Velocity;
    public float Size;
}