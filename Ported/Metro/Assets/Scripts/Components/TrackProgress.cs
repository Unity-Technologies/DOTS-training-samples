using Unity.Entities;

[GenerateAuthoringComponent]
public struct TrackProgress : IComponentData
{
    public float Value;
}
