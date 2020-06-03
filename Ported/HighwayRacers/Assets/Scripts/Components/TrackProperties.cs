using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TrackProperties : IComponentData
{
    public float TrackLength;
    public float2 TrackStartingPoint;
    public float LaneSeparation;
}