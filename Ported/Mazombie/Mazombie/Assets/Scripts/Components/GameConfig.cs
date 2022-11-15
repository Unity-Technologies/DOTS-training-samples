
using Unity.Entities;

public struct GameConfig : IComponentData
{
    public Entity tile;
    public Entity wallPrefab;
    public Entity playerSpawnPrefab;
    public Entity zombiePrefab;
    public int mazeSize;
    public int num_zombies;
}