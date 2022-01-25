using UnityEngine.Jobs;
using Unity.Collections;
using Onboarding.BezierPath;
using UnityEngine;
using Unity.Burst;

[BurstCompile(FloatMode = FloatMode.Fast)]
public struct EntityNavigationJob : IJobParallelForTransform
{
    [ReadOnly]
    public NativeArray<Vector3> controlPoints;
    [ReadOnly]
    public NativeArray<float> entitiesSpeed;
    [ReadOnly]
    public NativeArray<ApproximatedCurveSegment> pathDataCurves;
    [ReadOnly]
    public float deltaTime;
    [ReadOnly]
    public float pathLength;

    public NativeArray<float> entitiesDistancesFromSplineOrigin;
    public NativeArray<int> entitiesLastUsedCurveIndex;

    public void Execute(int index, TransformAccess transform)
    {
        int lastUsedCurveIndex = entitiesLastUsedCurveIndex[index];
        float distance = entitiesDistancesFromSplineOrigin[index] + deltaTime * entitiesSpeed[index];
        if (distance > pathLength)
        {
            distance = distance % pathLength;
            lastUsedCurveIndex = 0;
        }
        else if (distance < 0)
        {
            distance = pathLength + (distance % pathLength);
            lastUsedCurveIndex = 0;
        }
        
        entitiesDistancesFromSplineOrigin[index] = distance;

        float parametricT = FindSegmentFromDistance(ref lastUsedCurveIndex, distance);
        entitiesLastUsedCurveIndex[index] = lastUsedCurveIndex;

        int offset = pathDataCurves[lastUsedCurveIndex].bezierIndex * 3;
        Vector3 p0 = controlPoints[offset + 0];
        Vector3 p1 = controlPoints[offset + 1];
        Vector3 p2 = controlPoints[offset + 2];
        Vector3 p3 = controlPoints[offset + 3];
        double t = parametricT;
        double t2 = t * t;
        double t3 = t2 * t;
        double _1_t = 1 - t;
        double _1_t_2 = _1_t * _1_t;
        double _1_t_3 = _1_t_2 * _1_t;
        transform.position = new Vector3((float)(p0.x * _1_t_3 + 3 * p1.x * _1_t_2 * t + 3 * p2.x * _1_t * t2 + p3.x * t3),
                            (float)(p0.y * _1_t_3 + 3 * p1.y * _1_t_2 * t + 3 * p2.y * _1_t * t2 + p3.y * t3),
                            (float)(p0.z * _1_t_3 + 3 * p1.z * _1_t_2 * t + 3 * p2.z * _1_t * t2 + p3.z * t3));
    }

    private float FindSegmentFromDistance(ref int currentSegment, float distanceFromOrigin)
    {
        float distance = Mathf.Clamp(distanceFromOrigin, 0, pathLength);
        int increment = 1;

        // If the user teleports around, the cache will not work properly, as it's assuming we are going at a reasonable speed
        // in the same direction.
        // To alleviate the issue, we assume we always more forward, except if we pass a distance that was supposed to already have
        // been treated, and would never be found in our segment list if we were going forward through the list
        if (distance < pathDataCurves[currentSegment].start)
            increment = -1;

        float parametricT = -1;
        for (int i = currentSegment; i >= 0 && i < pathDataCurves.Length; i += increment)
        {
            if (distance >= pathDataCurves[i].start && distance <= pathDataCurves[i].end)
            {
                float ratio = (distance - pathDataCurves[i].start) / (pathDataCurves[i].end - pathDataCurves[i].start);
                parametricT = pathDataCurves[i].p0 + ratio * (pathDataCurves[i].p1 + ratio * (pathDataCurves[i].p2 + pathDataCurves[i].p3 * ratio));
                currentSegment = i;
                break;
            }
        }

        Debug.Assert(parametricT != -1, "PathController.FindSegmentFromDistance() : The requested distance was not found within the path. Maybe you need to rebuild it ?");
        return parametricT;
    }
}
