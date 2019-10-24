using Unity.Entities;
using Unity.Mathematics;

public struct BezierUtils
{
    public static float3 GetPositionAtT(BlobAssetReference<Curve> curve, float t)
    {
        var progressDistance = curve.Value.distance * t;
        var pointIndex_region_start = GetRegionIndex(curve, progressDistance);
        var pointIndex_region_end = (pointIndex_region_start + 1) % curve.Value.points.Length;

        // get start and end bez points
        var point_region_start = curve.Value.points[pointIndex_region_start];
        var point_region_end = curve.Value.points[pointIndex_region_end];
        // lerp between the points to arrive at PROGRESS
        var pathProgress_start = point_region_start.distanceAlongPath /  curve.Value.distance;
        var pathProgress_end = (pointIndex_region_end != 0) ?  point_region_end.distanceAlongPath /  curve.Value.distance : 1f;
        var regionProgress = (t - pathProgress_start) / (pathProgress_end - pathProgress_start);

        // do your bezier lerps
        // Round 1 --> Origins to handles, handle to handle
        return BezierLerp(point_region_start, point_region_end, regionProgress);
    }

    public static float Get_AccurateDistanceBetweenPoints(BlobAssetReference<Curve> curve, int current, int prev)
    {
        var currentPoint = curve.Value.points[current];
        var prevPoint = curve.Value.points[prev];
        const float measurementIncrement = 1f / Metro.BEZIER_MEASUREMENT_SUBDIVISIONS;
        var regionDistance = 0f;

        for (var i = 0; i < Metro.BEZIER_MEASUREMENT_SUBDIVISIONS- 1; i++)
        {
            var _CURRENT_SUBDIV = i * measurementIncrement;
            var _NEXT_SOBDIV = (i + 1) * measurementIncrement;
            var bezierLerpCurrent = BezierLerp(prevPoint, currentPoint, _CURRENT_SUBDIV);
            var bezierLerpNext = BezierLerp(prevPoint, currentPoint, _NEXT_SOBDIV);
            regionDistance += math.distance(bezierLerpCurrent, bezierLerpNext);
        }

        return regionDistance;
    }

    public static float3 GetNormalAtT(BlobAssetReference<Curve> curve, float t)
    {
        var current = GetPositionAtT(curve, t);
        var ahead = GetPositionAtT(curve, (t + 0.0001f) % 1f);
        var rot = (ahead - current) / math.distance(ahead, current);
        return rot;
    }

    static float3 BezierLerp(BezierPt _pointA, BezierPt _pointB, float _progress)
    {
        // Round 1 --> Origins to handles, handle to handle
        var l1_a_aOUT = math.lerp(_pointA.location, _pointA.handle_out, _progress);
        var l2_bIN_b = math.lerp(_pointB.handle_in, _pointB.location, _progress);
        var l3_aOUT_bIN = math.lerp(_pointA.handle_out, _pointB.handle_in, _progress);
        // Round 2
        var l1_to_l3 = math.lerp(l1_a_aOUT, l3_aOUT_bIN, _progress);
        var l3_to_l2 = math.lerp(l3_aOUT_bIN, l2_bIN_b, _progress);
        // Final Round
        return math.lerp(l1_to_l3, l3_to_l2, _progress);
    }

    static int GetRegionIndex(BlobAssetReference<Curve> curve, float _progress)
    {
        var result = 0;
        var totalPoints = curve.Value.points.Length;
        for (int i = 0; i < totalPoints; i++)
        {
            var _PT = curve.Value.points[i];
            if (_PT.distanceAlongPath <= _progress)
            {
                if (i == totalPoints - 1)
                {
                    // end wrap
                    result = i;
                    break;
                }
                else if (curve.Value.points[i + 1].distanceAlongPath >= _progress)
                {
                    // start < progress, end > progress <-- thats a match
                    result = i;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        return result;
    }

    static void MeasurePoint(BlobAssetReference<Curve> curve, ref float distance, int currentPoint, int prevPoint) {
        distance += Get_AccurateDistanceBetweenPoints(curve, currentPoint, prevPoint);
        curve.Value.points[currentPoint].distanceAlongPath = distance;
    }
}
