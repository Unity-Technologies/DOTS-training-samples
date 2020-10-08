using Unity.Entities;

[GenerateAuthoringComponent]
public struct CommuterTask_MoveToPlatform : IComponentData
{
    public Entity TargetPlatform;
}
