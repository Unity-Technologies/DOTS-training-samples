using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class BezierPath
{
    public List<BezierPoint> points;
    public List<float> distances;
    public float distance = 0f;

    public struct DistRemapPoint
    {
        public float inputValue;
        public float outputValue;
    }

    public const int RemapTableSize = 500;
    public DistRemapPoint[] remapTable = new DistRemapPoint[RemapTableSize];
    
    public void CalculateRemapTable()
    {
        float increment = distance / RemapTableSize;
        float distanceAccum = 0.0f;
        Vector3 prevPoint = Get_Position(0.0f, false);
        
        remapTable[0].inputValue = 0;
        remapTable[0].outputValue = 0;

        remapTable[RemapTableSize - 1].inputValue = distance;
        remapTable[RemapTableSize - 1].outputValue = distance;
        
        for (int i = 1; i < RemapTableSize - 1; ++i)
        {
            remapTable[i].inputValue = increment * i;
            Vector3 newPoint = Get_Position(increment * i / distance, false);
            distanceAccum += Vector3.Distance(prevPoint, newPoint);
            remapTable[i].outputValue = distanceAccum;
            prevPoint = newPoint;
        }
    }
    
    float findLinearRemappedInput(float desiredValue)
    {
        float clampedDesiredValue = math.clamp(desiredValue, 0, distance);
        
        // improvement here, could be a binary search
        int lessIndex = 0;
        int moreIndex = 1;
        for (int i = 1; i < RemapTableSize; ++i)
        {
            if (remapTable[i].outputValue >= clampedDesiredValue)
            {
                moreIndex = i;
                lessIndex = i - 1;
                break;
            }
        }
        
        float interp = (desiredValue - remapTable[lessIndex].outputValue) / (remapTable[moreIndex].outputValue - remapTable[lessIndex].outputValue);
        return remapTable[moreIndex].inputValue * interp + remapTable[lessIndex].inputValue * (1 - interp);
    }

    public static float FindLinearRemappedInput(float desiredValue, NativeArray<DistRemapPoint> remapTable, float distance)
    {
        float clampedDesiredValue = math.clamp(desiredValue, 0, distance);
        
        // improvement here, could be a binary search
        int lessIndex = 0;
        int moreIndex = 1;
        for (int i = 1; i < RemapTableSize; ++i)
        {
            if (remapTable[i].outputValue >= clampedDesiredValue)
            {
                moreIndex = i;
                lessIndex = i - 1;
                break;
            }
        }
        
        float interp = (desiredValue - remapTable[lessIndex].outputValue) / (remapTable[moreIndex].outputValue - remapTable[lessIndex].outputValue);
        return remapTable[moreIndex].inputValue * interp + remapTable[lessIndex].inputValue * (1 - interp);
    }
    
    
    
    public BezierPath()
    {
        points = new List<BezierPoint>();
        distances = new List<float>();
        distance = 0f;
    }

    public void AddPoint(Vector3 _location)
    {
        BezierPoint result = new BezierPoint(points.Count, _location, _location, _location);
        points.Add(result);
        distances.Add(0f);
        if (points.Count > 1)
        {
            BezierPoint _prev = points[points.Count - 2];
            BezierPoint _current = points[points.Count - 1];
            SetHandles(ref _current, _prev.location);

            points[points.Count - 1] = _current;
            distances[points.Count - 1] = 0f;
        }
    }

    void SetHandles(ref BezierPoint _point, Vector3 _prevPointLocation)
    {
        Vector3 _dist_PREV_CURRENT = Vector3.Normalize(_point.location - _prevPointLocation);

        _point.SetHandles(_dist_PREV_CURRENT);     
    }

    public void MeasurePath()
    {
        distance = 0f;
        BezierPoint point = points[0];
        float distanceAlongPath = 0.000001f;
        points[0] = point;
        distances[0] = distanceAlongPath;

        for (int i = 1; i < points.Count; i++)
        {
            MeasurePoint(i, i-1);
        }
        // add last stretch (return loop to point ZERO)
        distance += Get_AccurateDistanceBetweenPoints(0, points.Count - 1);
        CalculateRemapTable();
    }

    public float Get_AccurateDistanceBetweenPoints(int _current, int _prev)
    {
        BezierPoint _currentPoint = points[_current];
        BezierPoint _prevPoint = points[_prev];
        float measurementIncrement = 1f / MetroDefines.BEZIER_MEASUREMENT_SUBDIVISIONS;
        float regionDistance = 0f;
        for (int i = 0; i < MetroDefines.BEZIER_MEASUREMENT_SUBDIVISIONS; i++)
        {
            float _CURRENT_SUBDIV = i * measurementIncrement;
            float _NEXT_SOBDIV = (i + 1) * measurementIncrement;
            regionDistance += Vector3.Distance(BezierLerp(_prevPoint, _currentPoint, _CURRENT_SUBDIV),
                BezierLerp(_prevPoint, _currentPoint, _NEXT_SOBDIV));
        }

        return regionDistance;
    }

    public void MeasurePoint(int _currentPoint, int _prevPoint) {
        
        distance += Get_AccurateDistanceBetweenPoints(_currentPoint, _prevPoint);
        
        BezierPoint point = points[_currentPoint];
        points[_currentPoint] = point;
        distances[_currentPoint] = distance;
    }

    public Vector3 Get_NormalAtPosition(float _position, bool useRemapTable)
    {
        Vector3 _current = Get_Position(_position, useRemapTable);
        Vector3 _ahead = Get_Position((_position + 0.0001f) % 1f, useRemapTable);
        return (_ahead - _current) / Vector3.Distance(_ahead, _current);
    }

    public Vector3 Get_TangentAtPosition(float _position, bool useRemapTable)
    {
        Vector3 normal = Get_NormalAtPosition(_position, useRemapTable);
        return new Vector3(-normal.z, normal.y, normal.x);
    }

    public Vector3 GetPoint_PerpendicularOffset(ref BezierPoint _point, ref float distanceAlongPath, float _offset, bool useRemapTable)
    {
        return _point.location + Get_TangentAtPosition(distanceAlongPath / distance, useRemapTable) * _offset;
    }

    public Vector3 Get_Position(float _progress, bool useRemapTable)
    {
        float progressDistance = distance * _progress;
        if (useRemapTable)
            progressDistance = findLinearRemappedInput(progressDistance);
        
        int pointIndex_region_start = GetRegionIndex(progressDistance);
        int pointIndex_region_end = (pointIndex_region_start + 1) % points.Count;

        // get start and end bez points
        BezierPoint point_region_start = points[pointIndex_region_start];
        BezierPoint point_region_end = points[pointIndex_region_end];
        float distanceAlongPath_start = distances[pointIndex_region_start];
        float distanceAlongPath_end = distances[pointIndex_region_end];
        // lerp between the points to arrive at PROGRESS
        float pathProgress_start = distanceAlongPath_start;
        float pathProgress_end = (pointIndex_region_end != 0) ? distanceAlongPath_end : distance;
        float regionProgress = (progressDistance - pathProgress_start) / (pathProgress_end - pathProgress_start);

        // do your bezier lerps
        // Round 1 --> Origins to handles, handle to handle
        Vector3 returnVal = BezierLerp(point_region_start, point_region_end, regionProgress);
        points[pointIndex_region_start] = point_region_start;
        points[pointIndex_region_end] = point_region_end;
        distances[pointIndex_region_start] = distanceAlongPath_start;
        distances[pointIndex_region_end] = distanceAlongPath_end;
        return returnVal;
    }

    public int GetRegionIndex(float _progress)
    {
        int result = 0;
        int totalPoints = points.Count;
        for (int i = 0; i < totalPoints; i++)
        {
            float distanceAlongPath = distances[i];
            if (distanceAlongPath <= _progress)
            {
                if (i == totalPoints - 1)
                {
                    // end wrap
                    result = i;
                    break;
                }
                else if (distances[i + 1] >= _progress)
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

    public Vector3 BezierLerp(BezierPoint _pointA, BezierPoint _pointB, float _progress)
    {
        // Round 1 --> Origins to handles, handle to handle
        Vector3 l1_a_aOUT = Vector3.Lerp(_pointA.location, _pointA.handle_out, _progress);
        Vector3 l2_bIN_b = Vector3.Lerp(_pointB.handle_in, _pointB.location, _progress);
        Vector3 l3_aOUT_bIN = Vector3.Lerp(_pointA.handle_out, _pointB.handle_in, _progress);
        // Round 2 
        Vector3 l1_to_l3 = Vector3.Lerp(l1_a_aOUT, l3_aOUT_bIN, _progress);
        Vector3 l3_to_l2 = Vector3.Lerp(l3_aOUT_bIN, l2_bIN_b, _progress);
        // Final Round
        Vector3 result = Vector3.Lerp(l1_to_l3, l3_to_l2, _progress);
        return result;
    }

    public float GetPathDistance()
    {
        return distance;
    }
}

public struct BezierPoint
{
    public Vector3 location, handle_in, handle_out;

    public BezierPoint(int _index, Vector3 _location, Vector3 _handle_in, Vector3 _handle_out)
    {
        location = _location;
        handle_in = _handle_in;
        handle_out = _handle_out;
    }

    public void SetHandles(Vector3 _distance)
    {
        _distance *= MetroDefines.BEZIER_HANDLE_REACH;
        handle_in = location - _distance;
        handle_out = location + _distance;
    }
}