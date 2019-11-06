using Unity.Entities;

[GenerateAuthoringComponent]
public struct OverlayColorComponent : IComponentData
{
    public byte Color;
}