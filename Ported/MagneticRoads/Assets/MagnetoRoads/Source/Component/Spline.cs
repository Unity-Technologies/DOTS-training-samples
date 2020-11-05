using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Spline : IComponentData
{
    public float3 startPos;
    public float3 anchor1;
    public float3 anchor2;
    public float3 endPos;
    public quaternion startRot;
    public quaternion endRot;
}
