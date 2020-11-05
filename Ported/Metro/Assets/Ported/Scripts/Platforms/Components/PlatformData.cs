using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlatformData : IComponentData
{
    public Entity FrontStairsBottom;
    public Entity FrontStairsTop;
    public Entity BackStairsBottom;
    public Entity BackStairsTop;
}
