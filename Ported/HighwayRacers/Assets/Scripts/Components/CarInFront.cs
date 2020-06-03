using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarInFront : IComponentData
{
    public float TrackProgressCarInFront;
    public float Speed;
}