using Unity.Entities;
using Unity.Mathematics;

public struct PheromoneMapSetting : IComponentData
{
    public int Size;
    public float2 Offset;
    public float WorldSize;
}
