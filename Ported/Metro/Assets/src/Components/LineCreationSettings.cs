using Unity.Entities;
using Unity.Mathematics;

public struct LineCreationSettings : IComponentData
{
    public int TrainCount;
    public int CarriageCount;
    public float4 Color;
}
