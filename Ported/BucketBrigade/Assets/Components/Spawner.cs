using Unity.Entities;
using Unity.Mathematics;

public struct Spawner : IComponentData
{
    public Entity BotPrefab;
    public Entity BucketPrefab;
    public Entity FireCell;
    public Entity WaterCell;

    public float4 ScooperColor;
    public float4 FillerColor;
    public float4 ThrowerColor;
    public float4 PasserFullColor;
    public float4 PasserEmptyColor;
}