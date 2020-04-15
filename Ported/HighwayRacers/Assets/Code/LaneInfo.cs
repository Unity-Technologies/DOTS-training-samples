using Unity.Entities;
using Unity.Mathematics;

public struct LaneInfo : IComponentData
{
    public float2 StartXZ;
    public float2 EndXZ;
}
