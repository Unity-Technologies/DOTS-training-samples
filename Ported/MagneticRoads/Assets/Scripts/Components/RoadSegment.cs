using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;

namespace Components
{
    public struct RoadSegment: IComponentData
    {
        [ReadOnly] public RoadTerminator Start;
        [ReadOnly] public RoadTerminator End;
    }
}

public struct RoadTerminator
{
    public float3 Position;
    public float3 Normal;
    public float3 Tangent;
}