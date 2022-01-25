#if false
using Onboarding.BezierPath;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SplineFollowerSystem : SystemBase
{
    protected override void OnUpdate() {
        float time = (float)Time.ElapsedTime;
        float distance = time * 2.0f;

        Entities
            .WithAll<SplineFollower>()
            .ForEach((ref Translation translation,
                      ref Rotation rotation,
                      in Spline spline) =>
            {
                int dummySegment = 0;
                InterpolatePosition(spline, ref dummySegment, distance, out translation.Value);
            }).ScheduleParallel();
    }

    public static void InterpolatePosition(Spline data, ref int currentSegment, float distanceFromOrigin, out float3 value) {
        var splineData = data.splinePath.Value;
        float parametricT = FindSegmentFromDistance(ref splineData, ref currentSegment, distanceFromOrigin);

        InterpolatePosition(ref splineData, splineData.distanceToParametric[currentSegment].bezierIndex, parametricT, out value);
    }

    public static void InterpolatePosition(ref SplineData splineData, int splineIndex, float _t, out float3 value) {
        int offset = splineIndex * 3;
        float3 p0 = splineData.bezierControlPoints[offset + 0];
        float3 p1 = splineData.bezierControlPoints[offset + 1];
        float3 p2 = splineData.bezierControlPoints[offset + 2];
        float3 p3 = splineData.bezierControlPoints[offset + 3];
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

    private static float FindSegmentFromDistance(ref SplineData data, ref int currentSegment, float distanceFromOrigin) {
        float distance = math.clamp(distanceFromOrigin, 0, data.pathLength);
        int increment = 1;

        // If the user teleports around, the cache will not work properly, as it's assuming we are going at a reasonable speed
        // in the same direction.
        // To alleviate the issue, we assume we always more forward, except if we pass a distance that was supposed to already have
        // been treated, and would never be found in our segment list if we were going forward through the list
        if (distance < data.distanceToParametric[currentSegment].start)
            increment = -1;

        float parametricT = -1;
        for (int i = currentSegment; i >= 0 && i < data.distanceToParametric.Length; i += increment) {
            if (distance >= data.distanceToParametric[i].start && distance <= data.distanceToParametric[i].end) {
                float ratio = (distance - data.distanceToParametric[i].start) / (data.distanceToParametric[i].end - data.distanceToParametric[i].start);
                parametricT = InterpolateBezier1D(data.distanceToParametric[i], ratio);
                currentSegment = i;
                break;
            }
        }

        UnityEngine.Debug.Assert(parametricT != -1, "PathController.FindSegmentFromDistance() : The requested distance was not found within the path. Maybe you need to rebuild it ?");
        return parametricT;
    }

    public static float InterpolateBezier1D(ApproximatedCurveSegment data, float t) {
        // This is assuming the coefficients are DEVELOPED at baking time
        return data.p0 + t * (data.p1 + t * (data.p2 + data.p3 * t));
    }
}
#endif