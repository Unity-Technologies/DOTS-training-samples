using Unity.Entities;
using Unity.Mathematics;

public struct RockSpawner : IComponentData
{
    public int Attempts;
    public Entity RockPrefab;
    public int2 GridSize;
}