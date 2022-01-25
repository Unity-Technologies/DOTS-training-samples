using Unity.Entities;
using Unity.Mathematics;

public struct MapSpawner : IComponentData
{
    public Entity TilePrefab;
    public Entity WallPrefab;
    public float4 TileOddColor;
    public float4 TileEvenColor;
}
