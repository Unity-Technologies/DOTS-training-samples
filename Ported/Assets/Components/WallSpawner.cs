using Unity.Entities;

[GenerateAuthoringComponent]
public struct WallSpawner : IComponentData
{
    public int wallCount;
    public float wallThickness;
    public Entity wallPrefab;
}