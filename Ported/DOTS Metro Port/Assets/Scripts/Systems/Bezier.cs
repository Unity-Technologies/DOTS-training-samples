using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Mathematics;
using UnityEngine;

public class BezierPath
{
    static public void MeasurePath(Unity.Collections.NativeArray<BezierPoint> points)
    {
        var distance = 0f;

        for (int i = 1; i < points.Length; i++)
        {
            distance += Get_AccurateDistanceBetweenPoints(points, i, i - 1);
            var pointi = points[i];
            pointi.distanceAlongPath = distance;
            points[i] = pointi;
        }

        // add last stretch (return loop to point ZERO)
        distance += Get_AccurateDistanceBetweenPoints(points, 0, points.Length - 1);

        // store path lenth in first point
        var point0 = points[0];
        point0.distanceAlongPath = distance;
        points[0] = point0;
    }

    const int BEZIER_MEASUREMENT_SUBDIVISIONS = 20;

    static private float Get_AccurateDistanceBetweenPoints(Unity.Collections.NativeArray<BezierPoint> points, int _current, int _prev)
    {
        BezierPoint _currentPoint = points[_current];
        BezierPoint _prevPoint = points[_prev];
        float measurementIncrement = 1f / BEZIER_MEASUREMENT_SUBDIVISIONS;
        float regionDistance = 0f;
        for (int i = 0; i < BEZIER_MEASUREMENT_SUBDIVISIONS; i++)
        {
            float _CURRENT_SUBDIV = i * measurementIncrement;
            float _NEXT_SOBDIV = (i + 1) * measurementIncrement;
            var a = BezierLerp(points, _prev, _current, _CURRENT_SUBDIV);
            var b = BezierLerp(points, _prev, _current, _NEXT_SOBDIV);
            regionDistance += Vector3.Distance(a, b);
        }

        return regionDistance;
    }

    static private Vector3 BezierLerp(Unity.Collections.NativeArray<BezierPoint> points, int pointAIndex, int pointBIndex, float percentagePos)
    {
        float3 p0 = points[(pointAIndex - 1 + points.Length) % points.Length].location;
        float3 p1 = points[pointAIndex].location;
        float3 p2 = points[pointBIndex].location;
        float3 p3 = points[(pointBIndex + 1) % points.Length].location;

        float t0 = 0;
        float t1 = t0 + math.distance(p0, p1);
        float t2 = t1 + math.distance(p1, p2);
        float t3 = t2 + math.distance(p2, p3);
        float t = math.lerp(t1, t2, percentagePos);

        float3 a1 = ((t1 - t) * p0 + (t - t0) * p1) / (t1 - t0);
        float3 a2 = ((t2 - t) * p1 + (t - t1) * p2) / (t2 - t1);
        float3 a3 = ((t3 - t) * p2 + (t - t2) * p3) / (t3 - t2);

        float3 b1 = ((t2 - t) * a1 + (t - t0) * a2) / (t2 - t0);
        float3 b2 = ((t3 - t) * a2 + (t - t1) * a3) / (t3 - t1);

        float3 c = ((t2 - t) * b1 + (t - t1) * b2) / (t2 - t1);

        return c;
    }


     static public float3 Get_NormalAtPosition(Unity.Collections.NativeArray<BezierPoint> points, float percentagePos)
     {
        float3 _current = Get_Position(points, percentagePos);
         float3 _ahead = Get_Position(points, (percentagePos + 0.0001f) % 1f);
         return (_ahead - _current) / math.distance(_ahead, _current);
     }

     static public float3 Get_TangentAtPosition(Unity.Collections.NativeArray<BezierPoint> points, float percentagePos)
     {
         float3 normal = Get_NormalAtPosition(points, percentagePos);
         return new float3(-normal.z, normal.y, normal.x);
     }

     static public float3 GetPoint_PerpendicularOffset(Unity.Collections.NativeArray<BezierPoint> points, BezierPoint _point, float _offset)
     {
        float distance = Get_PathLength(points);
        return _point.location + Get_TangentAtPosition(points, _point.distanceAlongPath / distance) * _offset;
     }

     static public float3 Get_Position(Unity.Collections.NativeArray<BezierPoint> points, float percentagePos)
     {
        float distance = Get_PathLength(points);
        Assert.AreNotEqual(distance, 0.0f);

        float progressDistance = distance * percentagePos;
         int pointIndex_region_start = GetRegionIndex(points, progressDistance);
         int pointIndex_region_end = (pointIndex_region_start + 1) % points.Length;

         // get start and end bez points
         BezierPoint point_region_start = points[pointIndex_region_start];
         BezierPoint point_region_end = points[pointIndex_region_end];
         // lerp between the points to arrive at PROGRESS
         float pathProgress_start = (pointIndex_region_start == 0) ? 0.0f : point_region_start.distanceAlongPath / distance;
         float pathProgress_end = (pointIndex_region_end != 0) ?  point_region_end.distanceAlongPath / distance : 1f;
         float regionProgress = (percentagePos - pathProgress_start) / (pathProgress_end - pathProgress_start);

         // do your bezier lerps
         // Round 1 --> Origins to handles, handle to handle
         return BezierLerp(points, pointIndex_region_start, pointIndex_region_end, regionProgress);
     }

    static public float Get_PathLength(Unity.Collections.NativeArray<BezierPoint> points)
    {
        return points[0].distanceAlongPath;
    }

     static public int GetRegionIndex(Unity.Collections.NativeArray<BezierPoint> points, float _progress)
     {
         int result = 0;
         int totalPoints = points.Length;
         for (int i = 0; i < totalPoints; i++)
         {
             float currentPointStartDistance = (i == 0) ? 0.0f : points[i].distanceAlongPath;
             if (currentPointStartDistance <= _progress)
             {
                 if (i == totalPoints - 1)
                 {
                     // end wrap
                     result = i;
                     break;
                 }
                 else if (points[i + 1].distanceAlongPath >= _progress)
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
