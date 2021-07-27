using Unity.Entities;
using Unity.Rendering;

[GenerateAuthoringComponent]
public struct BoardSpawner : IComponentData
{
    public int maxWalls;
    public Entity tilePrefab;
    public Entity wallPrefab;
}
