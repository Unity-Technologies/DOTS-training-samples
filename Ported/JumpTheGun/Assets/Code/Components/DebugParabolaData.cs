using Unity.Entities;

public struct DebugParabolaSpawnerTag : IComponentData
{
}

public struct DebugParabolaData : IComponentData
{
    public int SampleCount;
    public Entity SamplePrefab;
}
