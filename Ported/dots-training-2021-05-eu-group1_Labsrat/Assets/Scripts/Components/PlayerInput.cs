using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInput : IComponentData
{
    public int TileIndex;
    public bool isMouseDown;
    // TODO: cursorPosition, isMouseDown etc ?
}
