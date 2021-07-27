using Unity.Entities;
using Unity.Rendering;

[GenerateAuthoringComponent]
public struct BoardSpawner : IComponentData
{
    public Entity tilePrefab;
    public Entity wallPrefab;
    public int maxWalls;
    public int maxHoles;
}
