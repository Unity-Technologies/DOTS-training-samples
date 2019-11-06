using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct HomebaseComponent : IComponentData
{
    public float4 Color;
    public int PlayerId;
}
