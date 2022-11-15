
using Unity.Entities;

public struct GameConfig : IComponentData
{
    public Entity tile;
    public Entity wallPrefab;
    public int mazeSize;
}