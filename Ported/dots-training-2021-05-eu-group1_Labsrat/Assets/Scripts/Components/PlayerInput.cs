using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInput : IComponentData
{
    public int TileIndex;
    public Cardinals ArrowDirection;
    public bool isMouseDown;
    // TODO: cursorPosition, isMouseDown etc ?
}
