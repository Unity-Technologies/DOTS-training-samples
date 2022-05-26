using Unity.Entities;

[GenerateAuthoringComponent]
public struct LaneComponent : IComponentData
{
    public int LaneNumber;
}

public struct CarElement: IBufferElementData
{
    public Entity CarEntity;
}
