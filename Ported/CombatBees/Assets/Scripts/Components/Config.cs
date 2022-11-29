using Unity.Entities;

public struct Config : IComponentData
{
    public Entity beePrefab;
    public int startBeeCount;
    public float minimumBeeSize;
    public float maximumBeeSize;
}