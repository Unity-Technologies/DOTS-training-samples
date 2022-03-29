using UnityVector3 = UnityEngine.Vector3;

using Unity.Entities;
using Unity.Mathematics;

public static class BezierHelpers
{
    public const float BEZIER_HANDLE_REACH = 0.15f;
    public const int BEZIER_MEASUREMENT_SUBDIVISIONS = 2;
    public const float BEZIER_PLATFORM_OFFSET = 3f;
    
    // Adds a new point to the given DynamicBuffer of BezierPointBufferElements, returning it afterwards.
    public static BezierPointBufferElement AddPoint(ref DynamicBuffer<BezierPointBufferElement> bezierPoints, UnityVector3 markerPosition)
    {
        BezierPointBufferElement result = new BezierPointBufferElement
        {
            Location = markerPosition,
            HandleIn = markerPosition,
            HandleOut = markerPosition
        };
        bezierPoints.Add(result);
        if (bezierPoints.Length > 1)
        {
            BezierPointBufferElement _prev = bezierPoints[bezierPoints.Length - 2];
            BezierPointBufferElement _current = bezierPoints[bezierPoints.Length - 1];
            SetHandles(_current, _prev.Location);
        }

        return result;
    }

    // Taking a Bezier point and a previous point, returns a new point with the same location but new control handles.
    public static BezierPointBufferElement SetHandles(BezierPointBufferElement point, BezierPointBufferElement prevPoint)
    {
        float3 directionVector = math.normalize(point.Location - prevPoint.Location);
        return SetHandles(point, directionVector);
    }

    // Taking a Bezier point and a direction vector, returns a new point with the same location but new control handles.
    public static BezierPointBufferElement SetHandles(BezierPointBufferElement point, float3 directionVector)
    {
        directionVector *= BEZIER_HANDLE_REACH;
        BezierPointBufferElement transformedPoint = point;
        transformedPoint.HandleIn = point.Location - directionVector;
        transformedPoint.HandleOut = point.Location + directionVector;

        return transformedPoint;
    }

    // Gets a corrected distance between two points along the Bezier curve, accurate to a certain resolution.
    public static float GetAccurateDistanceBetweenPoints(BezierPointBufferElement currentPoint,
        BezierPointBufferElement previousPoint)
    {
        float measurementIncrement = 1f / BEZIER_MEASUREMENT_SUBDIVISIONS;
        float regionDistance = 0f;
        for (int i = 0; i < BEZIER_MEASUREMENT_SUBDIVISIONS- 1; i++)
        {
            float currentSubdivsion = i * measurementIncrement;
            float nextSubdivision = (i + 1) * measurementIncrement;
            regionDistance += math.distance(BezierLerp(previousPoint, currentPoint, currentSubdivsion),
                                            BezierLerp(previousPoint, currentPoint, nextSubdivision));
        }

        return regionDistance;
    }

    // Measures total distance of path and also caches distances inside points themselves.
    public static float MeasurePath(ref DynamicBuffer<BezierPointBufferElement> bezierPoints)
    {
        float distance = 0f;
        
        BezierPointBufferElement firstPoint = bezierPoints[0];
        firstPoint.DistanceAlongPath = 0.000001f;
        bezierPoints[0] = firstPoint;
        
        for (int i = 1; i < bezierPoints.Length; i++)
        {
            BezierPointBufferElement currentPoint = bezierPoints[i];
            BezierPointBufferElement prevPoint = bezierPoints[i - 1];
            
            distance += GetAccurateDistanceBetweenPoints(currentPoint, prevPoint);
            currentPoint.DistanceAlongPath = distance;
            bezierPoints[i] = currentPoint;
        }
        // add last stretch (return loop to point ZERO)
        BezierPointBufferElement lastPoint = bezierPoints[bezierPoints.Length - 1];
        distance += GetAccurateDistanceBetweenPoints(firstPoint, lastPoint);

        return distance;
    }

    public static float3 BezierLerp(BezierPointBufferElement pointA, BezierPointBufferElement pointB, float progress)
    {
        // Round 1 --> Origins to handles, handle to handle
        float3 l1_a_aOUT = math.lerp(pointA.Location, pointA.HandleOut, progress);
        float3 l2_bIN_b = math.lerp(pointB.HandleIn, pointB.Location, progress);
        float3 l3_aOUT_bIN = math.lerp(pointA.HandleOut, pointB.HandleIn, progress);
        // Round 2 
        float3 l1_to_l3 = math.lerp(l1_a_aOUT, l3_aOUT_bIN, progress);
        float3 l3_to_l2 = math.lerp(l3_aOUT_bIN, l2_bIN_b, progress);
        // Final Round
        float3 result = math.lerp(l1_to_l3, l3_to_l2, progress);
        return result;
    }
    
    public static float3 GetPointPerpendicularOffset(DynamicBuffer<BezierPointBufferElement> bezierCurve, float pathDistance, BezierPointBufferElement point, float offset)
    {
        return point.Location + GetTangentAtPosition(bezierCurve, pathDistance, point.DistanceAlongPath / pathDistance) * offset;
    }
    
    public static float3 GetNormalAtPosition(DynamicBuffer<BezierPointBufferElement> bezierCurve, float pathDistance, float position)
    {
        float3 _current = GetPosition(bezierCurve, pathDistance, position);
        float3 _ahead = GetPosition(bezierCurve, pathDistance, (position + 0.0001f) % 1f);
        float distance = math.distance(_ahead, _current);

        return (_ahead - _current) / distance;
    }

    public static float3 GetTangentAtPosition(DynamicBuffer<BezierPointBufferElement> bezierCurve, float pathDistance, float position)
    {
        float3 normal = GetNormalAtPosition(bezierCurve, pathDistance, position);
        return new float3(-normal.z, normal.y, normal.x);
    }

    public static float3 GetPosition(DynamicBuffer<BezierPointBufferElement> bezierCurve, float pathDistance, float progress)
    {
        float progressDistance = pathDistance * progress;
        int pointIndex_region_start = GetRegionIndex(bezierCurve, progressDistance);
        int pointIndex_region_end = (pointIndex_region_start + 1) % bezierCurve.Length;

        // get start and end bez points
        BezierPointBufferElement point_region_start = bezierCurve[pointIndex_region_start];
        BezierPointBufferElement point_region_end = bezierCurve[pointIndex_region_end];
        // lerp between the points to arrive at PROGRESS
        float pathProgress_start = point_region_start.DistanceAlongPath / pathDistance;
        float pathProgress_end = (pointIndex_region_end != 0) ?  point_region_end.DistanceAlongPath / pathDistance : 1f;
        float regionProgress = (progress - pathProgress_start) / (pathProgress_end - pathProgress_start);

        // do your bezier lerps
        // Round 1 --> Origins to handles, handle to handle
        return BezierLerp(point_region_start, point_region_end, regionProgress);
    }
    
    public static int GetRegionIndex(DynamicBuffer<BezierPointBufferElement> bezierCurve, float progress)
    {
        int result = 0;
        int totalPoints = bezierCurve.Length;
        for (int i = 0; i < totalPoints; i++)
        {
            BezierPointBufferElement point = bezierCurve[i];
            if (point.DistanceAlongPath <= progress)
            {
                if (i == totalPoints - 1)
                {
                    // end wrap
                    result = i;
                    break;
                }
                else if (bezierCurve[i + 1].DistanceAlongPath >= progress)
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
}
