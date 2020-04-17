using Unity.Entities;

[GenerateAuthoringComponent]
public struct LaneAssignment : ISharedComponentData
{
    public int Value;
}
