using Onboarding.BezierPath;
using Unity.Mathematics;

public static class SplineInterpolationHelper
{
    // You can pass 0 as the currentSegment the first time, this is caching as a return value where the lookup found the distance last call
    // so if you keep passing the same member each time you request an interpolation, the call will return very quickly
    public static void InterpolatePosition(ref SplineDataBlob splineDataBlob, ref int currentSegment, float distanceFromOrigin, out float3 value) 
    {
        float parametricT = FindSegmentFromDistance(ref splineDataBlob, ref currentSegment, distanceFromOrigin);
        InterpolatePosition(ref splineDataBlob, splineDataBlob.distanceToParametric[currentSegment].bezierIndex, parametricT, out value);
    }
    
    public static void InterpolatePositionAndDirection(ref SplineDataBlob splineDataBlob, ref int currentSegment, float distanceFromOrigin, out float3 pos, out float3 dir)
    {
        float parametricT = FindSegmentFromDistance(ref splineDataBlob, ref currentSegment, distanceFromOrigin);

        InterpolatePosition(ref splineDataBlob, splineDataBlob.distanceToParametric[currentSegment].bezierIndex, parametricT, out pos);
        InterpolateDirection(ref splineDataBlob, splineDataBlob.distanceToParametric[currentSegment].bezierIndex, parametricT, out dir);
    }

    public static void InterpolatePosition(ref SplineDataBlob dataBlob, int splineIndex, float _t, out float3 value) {
        int offset = splineIndex * 3;
        float3 p0 = dataBlob.bezierControlPoints[offset + 0];
        float3 p1 = dataBlob.bezierControlPoints[offset + 1];
        float3 p2 = dataBlob.bezierControlPoints[offset + 2];
        float3 p3 = dataBlob.bezierControlPoints[offset + 3];
        double t = _t;
        double t2 = t * t;
        double t3 = t2 * t;
        double _1_t = 1 - t;
        double _1_t_2 = _1_t * _1_t;
        double _1_t_3 = _1_t_2 * _1_t;
        value = new float3((float)(p0.x * _1_t_3 + 3 * p1.x * _1_t_2 * t + 3 * p2.x * _1_t * t2 + p3.x * t3),
                            (float)(p0.y * _1_t_3 + 3 * p1.y * _1_t_2 * t + 3 * p2.y * _1_t * t2 + p3.y * t3),
                            (float)(p0.z * _1_t_3 + 3 * p1.z * _1_t_2 * t + 3 * p2.z * _1_t * t2 + p3.z * t3));
    }
    
    // --------------------------------------------------------------------------------------------
    public static void InterpolateDirection(ref SplineDataBlob  dataBlob, int splineIndex, float _t, out float3 value)
    {
        int offset = splineIndex * 3;
        float3 p0 = dataBlob.bezierControlPoints[offset + 0];
        float3 p1 = dataBlob.bezierControlPoints[offset + 1];
        float3 p2 = dataBlob.bezierControlPoints[offset + 2];
        float3 p3 = dataBlob.bezierControlPoints[offset + 3];

        float _1_t = 1f - _t;
        float t2 = _t * _t;
        value = new float3(3f * _1_t * _1_t * (p1.x - p0.x) + 6f * _1_t * _t * (p2.x - p1.x) + 3f * t2 * (p3.x - p2.x),
            3f * _1_t * _1_t * (p1.y - p0.y) + 6f * _1_t * _t * (p2.y - p1.y) + 3f * t2 * (p3.y - p2.y),
            3f * _1_t * _1_t * (p1.z - p0.z) + 6f * _1_t * _t * (p2.z - p1.z) + 3f * t2 * (p3.z - p2.z));
    }

    private static float FindSegmentFromDistance(ref SplineDataBlob splineDataBlob, ref int currentSegment, float distanceFromOrigin)
    {
        float loopedDistance = distanceFromOrigin > 0 ? distanceFromOrigin % splineDataBlob.pathLength : splineDataBlob.pathLength + (distanceFromOrigin % splineDataBlob.pathLength);
        int increment = 1;

        // If the user teleports around, the cache will not work properly, as it's assuming we are going at a reasonable speed
        // in the same direction.
        // To alleviate the issue, we assume we always more forward, except if we pass a distance that was supposed to already have
        // been treated, and would never be found in our segment list if we were going forward through the list
        if (loopedDistance < splineDataBlob.distanceToParametric[currentSegment].start)
            increment = -1;

        float parametricT = -1;
        for (int i = currentSegment; i >= 0 && i < splineDataBlob.distanceToParametric.Length; i += increment) {
            if (loopedDistance >= splineDataBlob.distanceToParametric[i].start && loopedDistance <= splineDataBlob.distanceToParametric[i].end) {
                float ratio = (loopedDistance - splineDataBlob.distanceToParametric[i].start) / (splineDataBlob.distanceToParametric[i].end - splineDataBlob.distanceToParametric[i].start);
                parametricT = InterpolateBezier1D(ref splineDataBlob.distanceToParametric[i], ratio);
                currentSegment = i;
                break;
            }
        }

        UnityEngine.Debug.Assert(parametricT != -1, "PathController.FindSegmentFromDistance() : The requested distance was not found within the path. Maybe you need to rebuild it ?");
        return parametricT;
    }

    public static float InterpolateBezier1D(ref ApproximatedCurveSegment data, float t) {
        // This is assuming the coefficients are DEVELOPED at baking time
        return data.p0 + t * (data.p1 + t * (data.p2 + data.p3 * t));
    }
}
