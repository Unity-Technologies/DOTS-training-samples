using Unity.Entities;

public struct BeeSpawner : IComponentData
{
    public Entity BlueBeePrefab;
    public Entity RedBeePrefab;
    public int BlueBeeCount;
    public int RedBeeCount;
}
