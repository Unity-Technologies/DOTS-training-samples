using Unity.Entities;

[GenerateAuthoringComponent]
public struct Leader : IComponentData
{
    public Entity TrackData;
}