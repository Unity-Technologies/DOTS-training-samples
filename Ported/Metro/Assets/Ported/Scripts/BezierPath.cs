using System.Collections.Generic;
using Unity.Mathematics;

public class BezierPath
{
    public List<BezierPoint> points;
    private float pathLength;
    private float distance = 0f;

    public BezierPath()
    {
        points = new List<BezierPoint>();
        distance = 0f;
    }

    public BezierPoint AddPoint(int index, float3 _location)
    {
        BezierPoint result = new BezierPoint(index, _location, _location, _location);
        points.Insert(index, result);
        if (points.Count > 1)
        {
            BezierPoint _prev = points[index - 1];
            BezierPoint _current = points[index];
            SetHandles(_current, _prev.location);
        }

        return result;
    }
    
    void SetHandles(BezierPoint _point, float3 _prevPointLocation)
    {
        float3 _dist_PREV_CURRENT = math.normalize(_point.location - _prevPointLocation);

        _point.SetHandles(_dist_PREV_CURRENT);     
    }

    public void MeasurePath()
    {
        distance = 0f;
        points[0].distanceAlongPath = 0.000001f;
        for (int i = 1; i < points.Count; i++)
        {
            MeasurePoint(i, i-1);
        }
        // add last stretch (return loop to point ZERO)
        distance += Get_AccurateDistanceBetweenPoints(0, points.Count - 1);
    }

    public float Get_AccurateDistanceBetweenPoints(int _current, int _prev)
    {
        BezierPoint _currentPoint = points[_current];
        BezierPoint _prevPoint = points[_prev];
        float measurementIncrement = 1f / Globals.BEZIER_MEASUREMENT_SUBDIVISIONS;
        float regionDistance = 0f;
        for (int i = 0; i < Globals.BEZIER_MEASUREMENT_SUBDIVISIONS- 1; i++)
        {
            float _CURRENT_SUBDIV = i * measurementIncrement;
            float _NEXT_SOBDIV = (i + 1) * measurementIncrement;
            regionDistance += math.distance(BezierLerp(_prevPoint, _currentPoint, _CURRENT_SUBDIV),
                BezierLerp(_prevPoint, _currentPoint, _NEXT_SOBDIV));
        }

        return regionDistance;
    }

    public void MeasurePoint(int _currentPoint, int _prevPoint) {
            distance += Get_AccurateDistanceBetweenPoints(_currentPoint, _prevPoint);
            points[_currentPoint].distanceAlongPath = distance;
    }

    public float3 Get_NormalAtPosition(float _position)
    {
        float3 _current = Get_Position(_position);
        float3 _ahead = Get_Position((_position + 0.0001f) % 1f);
        return (_ahead - _current) / math.distance(_ahead, _current);
    }

    public float3 Get_TangentAtPosition(float _position)
    {
        float3 normal = Get_NormalAtPosition(_position);
        return new float3(-normal.z, normal.y, normal.x);
    }

    public float3 GetPoint_PerpendicularOffset(BezierPoint _point, float _offset)
    {
        return _point.location + Get_TangentAtPosition(_point.distanceAlongPath / distance) * _offset;
    }

    public float3 Get_Position(float _progress)
    {
        float progressDistance = distance * _progress;
        int pointIndex_region_start = GetRegionIndex(progressDistance);
        int pointIndex_region_end = (pointIndex_region_start + 1) % points.Count;

        // get start and end bez points
        BezierPoint point_region_start = points[pointIndex_region_start];
        BezierPoint point_region_end = points[pointIndex_region_end];
        // lerp between the points to arrive at PROGRESS
        float pathProgress_start = point_region_start.distanceAlongPath / distance;
        float pathProgress_end = (pointIndex_region_end != 0) ?  point_region_end.distanceAlongPath / distance : 1f;
        float regionProgress = (_progress - pathProgress_start) / (pathProgress_end - pathProgress_start);

        // do your bezier lerps
        // Round 1 --> Origins to handles, handle to handle
        return BezierLerp(point_region_start, point_region_end, regionProgress);
    }

    public int GetRegionIndex(float _progress)
    {
        int result = 0;
        int totalPoints = points.Count;
        for (int i = 0; i < totalPoints; i++)
        {
            BezierPoint _PT = points[i];
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

    public float3 BezierLerp(BezierPoint _pointA, BezierPoint _pointB, float _progress)
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

    public float GetPathDistance()
    {
        return distance;
    }
}

public class BezierPoint
{
    public int index;
    public float3 location, handle_in, handle_out;
    public float distanceAlongPath = 0f;
    public List<string> tags;

    public BezierPoint(int _index, float3 _location, float3 _handle_in, float3 _handle_out)
    {
        index = _index;
        location = _location;
        handle_in = _handle_in;
        handle_out = _handle_out;
        tags = new List<string>();
    }

    public void SetHandles(float3 _distance)
    {
        _distance *= Globals.BEZIER_HANDLE_REACH;
        handle_in = location - _distance;
        handle_out = location + _distance;
    }
}