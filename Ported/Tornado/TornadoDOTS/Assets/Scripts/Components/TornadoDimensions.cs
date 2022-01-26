using Unity.Entities;

[GenerateAuthoringComponent]
public struct TornadoDimensions : IComponentData
{
    public float TornadoRadius;
    public float TornadoHeight;
}