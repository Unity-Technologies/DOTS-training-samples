using Unity.Entities;

public struct Spawner : IComponentData
{
    public Entity BotPrefab;
    public Entity BucketPrefab;
    public Entity FireCell;
    public Entity WaterCell;
}


