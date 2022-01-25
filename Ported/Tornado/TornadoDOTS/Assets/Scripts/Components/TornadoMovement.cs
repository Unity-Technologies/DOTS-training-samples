using Unity.Entities;

[GenerateAuthoringComponent]
public struct TornadoMovement : IComponentData
{
    public float XFrequency;
    public float ZFrequency;
    public float Amplitude;
    public float MaxHeight;
}