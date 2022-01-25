using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public class Velocity : IComponentData
{
    public float3 Value;
}