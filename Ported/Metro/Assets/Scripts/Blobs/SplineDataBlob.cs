using Onboarding.BezierPath;
using Unity.Entities;
using Unity.Mathematics;

public struct SplineDataBlob
{
    public BlobArray<float3> bezierControlPoints;
    public BlobArray<ApproximatedCurveSegment> distanceToParametric;
    public float pathLength;
}
