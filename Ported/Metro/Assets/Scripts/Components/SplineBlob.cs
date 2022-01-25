using Onboarding.BezierPath;
using Unity.Entities;
using Unity.Mathematics;

public struct SplineData
{
    public BlobArray<float3> bezierControlPoints;
    public BlobArray<ApproximatedCurveSegment> distanceToParametric;
    public float pathLength;
}

public struct Spline : IComponentData
{
    public BlobAssetReference<SplineData> splinePath;
}

