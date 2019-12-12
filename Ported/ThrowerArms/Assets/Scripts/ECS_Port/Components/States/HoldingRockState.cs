using Unity.Entities;
using Unity.Mathematics;

public struct GrabbedState : IComponentData
{
    public Entity GrabbingEntity;
    public float3 localPosition;
}

public struct HoldingRockState : IComponentData
{
}