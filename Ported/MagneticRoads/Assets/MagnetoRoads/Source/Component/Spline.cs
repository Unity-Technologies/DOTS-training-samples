using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Spline : IComponentData
{
    public float3 startPos;
    public float3 startTangent;
    public float3 endPos;
    public float3 endTangent;
    public quaternion startRot;
    public quaternion endRot;
}
