using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class BezierUtilities
{
    public static float3 Get_Position(float _progress, ref MetroLineBlob metroLine)
    {
        float progressDistance = metroLine.Distance * _progress;
        int pointIndex_region_start = GetRegionIndex(progressDistance, ref metroLine.Path);
        int pointIndex_region_end = (pointIndex_region_start + 1) % metroLine.Path.Length;

        // get start and end bez points
        LinePoint point_region_start = metroLine.Path[pointIndex_region_start];
        LinePoint point_region_end = metroLine.Path[pointIndex_region_end];
        // lerp between the points to arrive at PROGRESS
        float pathProgress_start = point_region_start.distanceAlongPath / metroLine.Distance;
        float pathProgress_end = (pointIndex_region_end != 0) ?  point_region_end.distanceAlongPath / metroLine.Distance : 1f;
        float regionProgress = (_progress - pathProgress_start) / (pathProgress_end - pathProgress_start);

        // do your bezier lerps
        // Round 1 --> Origins to handles, handle to handle
        return BezierLerp(point_region_start, point_region_end, regionProgress);
    }
    
    public static int GetRegionIndex(float _progress, ref BlobArray<LinePoint> points)
    {
        int result = 0;
        int totalPoints = points.Length;
        for (int i = 0; i < totalPoints; i++)
        {
            LinePoint _PT = points[i];
            if (_PT.distanceAlongPath <= _progress)
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
    
    public static float3 BezierLerp(LinePoint _pointA, LinePoint _pointB, float _progress)
    {
        // Round 1 --> Origins to handles, handle to handle
        float3 l1_a_aOUT = math.lerp(_pointA.location, _pointA.handle_out, _progress);
        float3 l2_bIN_b = math.lerp(_pointB.handle_in, _pointB.location, _progress);
        float3 l3_aOUT_bIN = math.lerp(_pointA.handle_out, _pointB.handle_in, _progress);
        // Round 2 
        float3 l1_to_l3 = math.lerp(l1_a_aOUT, l3_aOUT_bIN, _progress);
        float3 l3_to_l2 = math.lerp(l3_aOUT_bIN, l2_bIN_b, _progress);
        // Final Round
        float3 result = math.lerp(l1_to_l3, l3_to_l2, _progress);
        return result;
    }

    public static float3 Get_NormalAtPosition(float _position, ref MetroLineBlob metroLine)
    {
        float3 _current = Get_Position(_position, ref metroLine);
        float3 _ahead = Get_Position((_position + 0.0001f) % 1f, ref metroLine);
        return (_ahead - _current) / math.distance(_ahead, _current);
    }
}
