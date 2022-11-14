
using Unity.Entities;

public struct GameConfig : IComponentData
{
    public Entity tile;

    public int mazeSize;
    public float cellSize;
}