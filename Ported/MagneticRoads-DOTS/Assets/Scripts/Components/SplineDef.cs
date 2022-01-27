using Unity.Entities;
using Unity.Mathematics;

public struct SplineDef : IComponentData
{
    public int splineId;
    
    public float3 startPoint;
    public float3 anchor1;
    public float3 anchor2;
    public float3 endPoint;
    
    public int3 startNormal;
    public int3 endNormal;
    public int3 startTangent;
    public int3 endTangent;
    public int twistMode;

    public float2 offset;
    public float measuredLength;
}
