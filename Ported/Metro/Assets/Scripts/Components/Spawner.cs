using Unity.Entities;

public struct Spawner : IComponentData
{
    public Entity LanePrefab;
    public int LaneCount;
    public Entity CarPrefab;
    public float CarFrequency;
}