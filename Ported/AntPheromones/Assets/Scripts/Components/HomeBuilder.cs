using Unity.Entities;
using Unity.Mathematics;

public struct HomeBuilder  : IComponentData
{
    public Entity homePrefab;
    public float homeRadius;
    public float4 homeColor;
}
