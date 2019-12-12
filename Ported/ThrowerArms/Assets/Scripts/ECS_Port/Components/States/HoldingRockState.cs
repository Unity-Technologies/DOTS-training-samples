using Unity.Entities;

public struct GrabbedState : IComponentData
{
    public Entity GrabbingEntity;
}

public struct HoldingRockState : IComponentData
{
}