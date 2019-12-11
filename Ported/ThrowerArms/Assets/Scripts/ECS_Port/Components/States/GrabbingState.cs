using Unity.Entities;

public struct GrabbedState : IComponentData
{
    public Entity GrabbingEntity;
}

public struct GrabbingState : IComponentData
{
}