using Unity.Entities;
using Unity.Mathematics;

public struct Spline : IComponentData
{
    public float3 startPos;
    public float3 endPos;
    public quaternion startRot;
    public quaternion endRot;
}
