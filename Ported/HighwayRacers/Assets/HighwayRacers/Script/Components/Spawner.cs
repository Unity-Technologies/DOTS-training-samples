using Unity.Entities;

public struct Spawner : IComponentData
{
    public int CarCount;
    public Entity CarPrefab;
}
