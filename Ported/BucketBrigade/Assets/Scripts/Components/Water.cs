using Unity.Entities;
using Unity.Mathematics;

public struct Water : IComponentData
{
    public float WaterLevel;
    public float3 Scale;
}
