using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

static public class BezierHelpers
{
    // float3 handleOffset = GetHandlesOffset(_current, _prevPointLocation):
    // float3 handleIn = _current - handleOffset;
    // float3 handleOut = _current + handleOffset;
    static public float3 GetHandlesOffset(float3 _position, float3 _prevPointLocation)
    {
        return math.normalize(_position - _prevPointLocation);
    }

    static public float3 GetHandleIn(float3 position, float3 offset)
    {
        return position - offset * Globals.BEZIER_HANDLE_REACH;
    }

    static public float3 GetHandleOut(float3 position, float3 offset)
    {
        return position + offset * Globals.BEZIER_HANDLE_REACH;
    }

    static public float3 BezierLerp(float3 positionA, float3 handleOutA, float3 positionB, float3 handleInB, float progress)
    {
        // Round 1 --> Origins to handles, handle to handle
        float3 l1_a_aOUT = math.lerp(positionA, handleOutA, progress);
        float3 l2_bIN_b = math.lerp(handleInB, positionB, progress);
        float3 l3_aOUT_bIN = math.lerp(handleOutA, handleInB, progress);
        // Round 2 
        float3 l1_to_l3 = math.lerp(l1_a_aOUT, l3_aOUT_bIN, progress);
        float3 l3_to_l2 = math.lerp(l3_aOUT_bIN, l2_bIN_b, progress);
        // Final Round
        return math.lerp(l1_to_l3, l3_to_l2, progress);
    }

    static public float GetAccurateDistanceBetweenPoints(float3 positionA, float3 handleOutA, float3 positionB, float3 handleInB)
    {
        float measurementIncrement = 1f / Globals.BEZIER_MEASUREMENT_SUBDIVISIONS;
        float regionDistance = 0f;
        for (int i = 0; i < Globals.BEZIER_MEASUREMENT_SUBDIVISIONS - 1; i++)
        {
            float _CURRENT_SUBDIV = i * measurementIncrement;
            float _NEXT_SOBDIV = (i + 1) * measurementIncrement;
            regionDistance += math.distance(BezierLerp(positionA, handleOutA, positionB, handleInB, _CURRENT_SUBDIV),
                BezierLerp(positionA, handleOutA, positionB, handleInB, _NEXT_SOBDIV));
        }

        return regionDistance;
    }

    static public float MeasurePath(float3[] positions, float3[] handlesIn, float3[] handlesOut, int count, out float[] distances)
    {
        float distance = 0f;
        distances = new float[positions.Length];
        distances[0] = 0.000001f;
        for (int i = 1; i < count; i++)
        {
            float3 positionA    = positions[i - 1];
            float3 handleOutA   = handlesOut[i - 1];
            float3 positionB    = positions[i];
            float3 handleInB    = handlesIn[i];
            distance += GetAccurateDistanceBetweenPoints(positionA, handleOutA, positionB, handleInB);
            distances[i] = distance;
        }
        // add last stretch (return loop to point ZERO)
        distance += GetAccurateDistanceBetweenPoints(positions[0], handlesOut[0], positions[count - 1], handlesIn[count - 1]);

        return distance;
    }

    static public int GetRegionIndex(float3[] positions, float[] distances, float _progressIsAbsoluteValue)
    {
        int result = 0;
        int totalPoints = positions.Length;
        for (int i = 0; i < totalPoints; i++)
        {
            float dist = distances[i];
            if (dist <= _progressIsAbsoluteValue)
            {
                if (i == totalPoints - 1)
                {
                    // end wrap
                    result = i;
                    break;
                }
                else if (distances[i + 1] >= _progressIsAbsoluteValue)
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

    static public float3 GetPosition(float3[] positions, float3[] handlesIn, float3[] handlesOut, float[] distances, float totalDist, float _progress)
    {
        float progressDistance = totalDist * _progress;
        int pointIndex_region_start = GetRegionIndex(positions, distances, progressDistance);
        int pointIndex_region_end = (pointIndex_region_start + 1) % positions.Length;

        // get start and end bez points
        float point_region_start_dist = distances[pointIndex_region_start];
        float point_region_end_dist = distances[pointIndex_region_end];
        float3 point_region_start_pos = positions[pointIndex_region_start];
        float3 point_region_end_pos = positions[pointIndex_region_end];
        float3 point_region_start_hOut = handlesOut[pointIndex_region_start];
        float3 point_region_end_hInt = handlesIn[pointIndex_region_end];
        // lerp between the points to arrive at PROGRESS
        float pathProgress_start = point_region_start_dist / totalDist;
        float pathProgress_end = (pointIndex_region_end != 0) ? point_region_end_dist / totalDist : 1f;
        float regionProgress = (_progress - pathProgress_start) / (pathProgress_end - pathProgress_start);

        // do your bezier lerps
        // Round 1 --> Origins to handles, handle to handle
        return BezierLerp(point_region_start_pos, point_region_start_hOut, point_region_end_pos, point_region_end_hInt, regionProgress);
    }

    static public float3 GetNormalAtPosition(float3[] positions, float3[] handlesIn, float3[] handlesOut, float[] distances, float totalDist, float _position)
    {
        float3 _current = GetPosition(positions, handlesIn, handlesOut, distances, totalDist, _position);
        float3 _ahead = GetPosition(positions, handlesIn, handlesOut, distances, totalDist, (_position + 0.0001f) % 1f);

        return (_ahead - _current) / math.distance(_ahead, _current);
    }

    static public float3 GetTangentAtPosition(float3[] positions, float3[] handlesIn, float3[] handlesOut, float[] distances, float totalDist, float _position)
    {
        float3 normal = GetNormalAtPosition(positions, handlesIn, handlesOut, distances, totalDist, _position);
        return new float3(-normal.z, normal.y, normal.x);
    }

    static public float3 GetPointPerpendicularOffset(float3 pos, float distanceAlongPath, float3[] positions, float3[] handlesIn, float3[] handlesOut, float[] distances, float totalDist, float _offset)
    {
        return pos + GetTangentAtPosition(positions, handlesIn, handlesOut, distances, totalDist, distanceAlongPath / totalDist) * _offset;
    }
}
