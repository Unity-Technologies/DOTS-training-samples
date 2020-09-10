using Unity.Entities;

[GenerateAuthoringComponent]
public struct Player : IComponentData
{
    public int CurrentArrow;
}
public struct PlayerArrow : IBufferElementData
{
    public const int MaxArrowsPerPlayer = 3;
    public Entity Arrow;
}