using Unity.Entities;
using Unity.Mathematics;

public struct SplineDefForCar : IComponentData
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

    public SplineDefForCar(SplineDef splineDef)
    {
        splineId = splineDef.splineId;

        startPoint = splineDef.startPoint;
        anchor1 = splineDef.anchor1;
        anchor2 = splineDef.anchor2;
        endPoint = splineDef.endPoint;

        startNormal = splineDef.startNormal;
        endNormal = splineDef.endNormal;
        startTangent = splineDef.startTangent;
        endTangent = splineDef.endTangent;
        twistMode = splineDef.twistMode;

        offset = splineDef.offset;
    }
}
