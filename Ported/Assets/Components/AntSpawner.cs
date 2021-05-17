using Unity.Entities;

[GenerateAuthoringComponent]
public struct AntSpawner : IComponentData
{
    public Entity AntPrefab;
    public int AntCount;
}
