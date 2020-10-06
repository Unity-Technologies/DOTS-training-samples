using Unity.Entities;

[GenerateAuthoringComponent]
public struct Board : IComponentData
{
    public Entity tilePrefab;
    public Entity wallPrefab;
    public int size;
    public float yNoise;
    public int wallCount;
    public int holeCount;
}
