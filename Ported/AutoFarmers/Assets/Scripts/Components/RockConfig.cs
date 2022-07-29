using Unity.Entities;
using Unity.Mathematics;


public struct RockConfig : IComponentData
{
    public Entity RockPrefab;
    public int NumRocks;
    public int2 RandomSizeMin;
    public int2 RandomSizeMax;
    public float minHeight;
    public float maxHeight;

    public RockState state;
}