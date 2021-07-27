using Unity.Entities;
using Unity.Rendering;

[GenerateAuthoringComponent]
public struct BoardSpawner : IComponentData
{
    public int boardSize;
    public Entity tilePrefab;
    public Entity wallPrefab;
    public Entity goalPrefab;
    public int maxWalls;
    public int maxHoles;
}
