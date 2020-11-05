using Unity.Entities;
using Unity.Mathematics;

public struct Spawner : IComponentData
{
    public Entity BotPrefab;
    public Entity BucketPrefab;
    public Entity FireCell;
    public Entity WaterCell;

    public float4 ScooperColor;
}


