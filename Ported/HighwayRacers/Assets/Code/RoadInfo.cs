using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RoadInfo : IComponentData
{
    public float2 StartXZ;
    public float2 EndXZ;
    public float LaneWidth;
    public int MaxLanes;

    //Move this probably
    public float CarLength;
}

public struct LaneInfo
{
    public float LaneLength;
}
