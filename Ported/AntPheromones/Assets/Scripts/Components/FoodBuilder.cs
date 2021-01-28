using Unity.Entities;
using Unity.Mathematics;

public struct FoodBuilder : IComponentData
{
    public Entity foodPrefab;
    public float foodRadius;
    public float4 foodColor;
    public float2 foodLocation;
    public int numFood;
}
