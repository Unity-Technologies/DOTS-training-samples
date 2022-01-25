using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PP_Movement : IComponentData
{
    public float3 startLocation;
    public float3 endLocation;
    public float t;
}