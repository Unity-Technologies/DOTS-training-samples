using Unity.Entities;

public struct DebugParabolaSpawnerTag : IComponentData
{
}

public struct DebugParabolaSampleTag : IComponentData
{
    public int id;
}

public struct DebugParabolaData : IComponentData
{
    public float Duration;
    public int SampleCount;
    public Entity SamplePrefab;
}
