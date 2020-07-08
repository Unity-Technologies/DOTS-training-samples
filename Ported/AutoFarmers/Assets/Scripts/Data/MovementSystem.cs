using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MovementSystem : IComponentData
{
    public float2 Value;
}