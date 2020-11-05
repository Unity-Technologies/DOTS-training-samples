using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Bot : IComponentData
{
    public int Index;
}

[GenerateAuthoringComponent]
public struct PasserBot : IComponentData
{
    public float3 PickupPosition;
    public float3 DropoffPosition;
}

[GenerateAuthoringComponent]
public struct ThrowerBot : IComponentData
{
}

[GenerateAuthoringComponent]
public struct FillerBot : IComponentData
{
    public Entity ChainStart;
}