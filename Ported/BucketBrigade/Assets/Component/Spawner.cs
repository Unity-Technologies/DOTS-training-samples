using Unity.Entities;

public struct Spawner : IComponentData
{
    public Entity ScooperPrefab;
    public Entity BucketPrefab;
    public Entity FireCell;
    public Entity WaterCell;
}


