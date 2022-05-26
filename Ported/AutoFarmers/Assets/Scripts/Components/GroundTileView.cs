using Unity.Entities;

public struct GroundTileView : IComponentData
{
    public int Index;
    public GroundTileState ViewState;
}
