using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct HomebaseComponent : IComponentData
{
    [GhostDefaultField(10, false)]
    public float4 Color;
    [GhostDefaultField]
    public int PlayerId;
}
