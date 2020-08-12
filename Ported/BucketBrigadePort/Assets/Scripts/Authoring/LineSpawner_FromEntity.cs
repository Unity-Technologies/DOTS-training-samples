using Unity.Entities;

// ReSharper disable once InconsistentNaming
public struct LineSpawner_FromEntity : IComponentData
{
    public int Count;
    public Entity Prefab;
}
