using Unity.Entities;

[GenerateAuthoringComponent]
public struct TrackPosition : IComponentData
{
    public float TrackProgress;
    public float Lane;
}