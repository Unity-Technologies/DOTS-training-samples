using Unity.Entities;

struct Config : IComponentData
{
    public Entity CarPrefab;
    public int CarCount;
    public float BrakingDistanceThreshold;
}