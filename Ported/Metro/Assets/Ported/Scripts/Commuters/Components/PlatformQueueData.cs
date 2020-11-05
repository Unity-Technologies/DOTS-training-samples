using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlatformQueueData : IComponentData
{
    public Entity Platform;
}