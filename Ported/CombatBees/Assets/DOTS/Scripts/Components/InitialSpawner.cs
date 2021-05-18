using Unity.Entities;

public struct InitialSpawner : IComponentData 
{
    public Entity BeePrefab;
    public int BeeCount;

    public Entity ResourcePrefab;
    public int ResourceCount;

    public int MaxSpeed;
}

