using Unity.Entities;
using Unity.Mathematics;

public struct Bot : IComponentData
{
    public int Index;
}

public struct PasserBot : IComponentData
{
    public float3 PickupPosition;
    public float3 DropoffPosition;
}

public struct ThrowerBot : IComponentData
{
}

public struct FillerBot : IComponentData
{
}

public struct GotoPickupLocation : IComponentData
{
}

public struct GotoDropoffLocation : IComponentData
{
}

public struct ScooperBot : IComponentData
{
    public Entity ChainStart;
}
