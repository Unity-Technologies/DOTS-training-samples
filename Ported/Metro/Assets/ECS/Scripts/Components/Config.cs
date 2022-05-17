using Unity.Entities;

public struct Config : IComponentData
{
    public Entity TrainPrefab;
    public int TrainsToSpawn;
}