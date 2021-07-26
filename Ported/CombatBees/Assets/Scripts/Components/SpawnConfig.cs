using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnConfig : IComponentData
{
    public float3 BeeSize;
    public float3 BaseSize;
    public float3 ResourceSize;
}
