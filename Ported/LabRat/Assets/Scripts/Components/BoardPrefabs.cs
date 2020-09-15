using Unity.Entities;

[GenerateAuthoringComponent]
public struct BoardPrefabs : IComponentData
{
    public Entity basePrefab;
    public Entity cellPrefab;
}
