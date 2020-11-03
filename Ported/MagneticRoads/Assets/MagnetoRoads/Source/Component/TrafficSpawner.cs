using Unity.Entities;

public struct TrafficSpawner : IComponentData
{
    public Entity CarPrefab;
    public Entity SimpleIntersectionPrefab;
    public Entity DoubleIntersectionPrefab;
    public Entity TripleIntersectionPrefab;
}
