using Unity.Entities;

public struct BeeSpawner : IComponentData 
{
    public Entity BeePrefab;
    public int BeeCount;
    public int BeeCountFromResource;

    public int MaxSpeed;
}

public struct ResourceSpawner : IComponentData 
{
    public Entity ResourcePrefab;
    public int ResourceCount;
}