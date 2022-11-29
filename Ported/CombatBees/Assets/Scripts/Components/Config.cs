using Unity.Entities;

public struct Config : IComponentData
{
    public Entity beePrefab;
    public Entity particlePrefab;
    public int startBeeCount;
    public int beesPerResource;
    public float minimumBeeSize;
    public float maximumBeeSize;
}