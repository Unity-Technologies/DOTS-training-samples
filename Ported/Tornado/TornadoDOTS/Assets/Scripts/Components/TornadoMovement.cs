using Unity.Entities;

[GenerateAuthoringComponent]
public struct TornadoMovement : IComponentData
{
    public float XFrequency;
    public float ZFrequency;
}