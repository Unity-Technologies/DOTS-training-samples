using Unity.Entities;

public struct ReachForTargetState : IComponentData
{
    public float ReachTimer;
    public Entity TargetEntity;
    public float TargetSize;
}