using Unity.Entities;

[GenerateAuthoringComponent]
public struct SegmentCounter : IComponentData
{
    public int Value;
}