using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInput : IComponentData
{
    public int TileIndex;
    public Cardinals ArrowDirection;
    public bool isMouseDown;
    public int CurrentArrowIndex;
    // TODO: cursorPosition, isMouseDown etc ?
}
