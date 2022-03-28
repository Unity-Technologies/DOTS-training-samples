using Unity.Entities;

public struct Spawner : IComponentData
{
    public Entity BotPrefab;
    public Entity WaterPoolPrefab;
    public Entity FlameCellPrefab;
    public Entity BucketPrefab;

    public int TeamCount;
    public int MembersCount;
    public int FireDimension;
}
