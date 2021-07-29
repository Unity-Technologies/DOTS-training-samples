using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Blood : IComponentData
{
    public float3 Speed;
    public double SpawnTime;
}
