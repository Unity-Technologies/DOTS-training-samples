using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInput : IComponentData
{
    public int TileIndex;
    public Cardinals ArrowDirection;
    public bool IsMouseDown;
}
