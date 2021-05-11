using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Line
{
    public BezierPath bezierPath;

    // scalar values stored in the 0 - Length range (Not 0 - 1)
    public float[] PlatformStopPoint;

    public void Create_RailPath(List<RailMarker> _outboundPoints)
    {
        bezierPath = new BezierPath();
        List<BezierPoint> _POINTS = bezierPath.points;
        int total_outboundPoints = _outboundPoints.Count;
        Vector3 currentLocation = Vector3.zero;

        List<int> stopPointIndices = new List<int>();
        // - - - - - - - - - - - - - - - - - - - - - - - -  OUTBOUND points
        for (int i = 0; i < total_outboundPoints; i++)
        {
            bezierPath.AddPoint(_outboundPoints[i].transform.position);

            if (_outboundPoints[i].railMarkerType == RailMarkerType.PLATFORM_END)
            {
                stopPointIndices.Add(i);
                stopPointIndices.Add(2 * total_outboundPoints - i);
            }
        }
        
        stopPointIndices.Sort();
        

        // fix the OUTBOUND handles
        for (int i = 0; i <= total_outboundPoints - 1; i++)
        {
            BezierPoint _currentPoint = _POINTS[i];
            if (i == 0)
            {
                _currentPoint.SetHandles(_POINTS[1].location - _currentPoint.location);
            }
            else if (i == total_outboundPoints - 1)
            {
                _currentPoint.SetHandles(_currentPoint.location - _POINTS[i - 1].location);
            }
            else
            {
                _currentPoint.SetHandles(_POINTS[i + 1].location - _POINTS[i - 1].location);
            }
        }

        bezierPath.MeasurePath();

        // - - - - - - - - - - - - - - - - - - - - - - - -  RETURN points
        float platformOffset = MetroDefines.BEZIER_PLATFORM_OFFSET;
        List<BezierPoint> _RETURN_POINTS = new List<BezierPoint>();
        for (int i = total_outboundPoints - 1; i >= 0; i--)
        {
            Vector3 _targetLocation = bezierPath.GetPoint_PerpendicularOffset(bezierPath.points[i], platformOffset);
            bezierPath.AddPoint(_targetLocation);
            _RETURN_POINTS.Add(_POINTS[_POINTS.Count - 1]);
        }

        // fix the RETURN handles
        for (int i = 0; i <= total_outboundPoints - 1; i++)
        {
            BezierPoint _currentPoint = _RETURN_POINTS[i];
            if (i == 0)
            {
                _currentPoint.SetHandles(_RETURN_POINTS[1].location - _currentPoint.location);
            }
            else if (i == total_outboundPoints - 1)
            {
                _currentPoint.SetHandles(_currentPoint.location - _RETURN_POINTS[i - 1].location);
            }
            else
            {
                _currentPoint.SetHandles(_RETURN_POINTS[i + 1].location - _RETURN_POINTS[i - 1].location);
            }
        }

        bezierPath.MeasurePath();

        PlatformStopPoint = new float[stopPointIndices.Count];
        
        for(int i = 0; i < stopPointIndices.Count; ++i)
        {
            int index = stopPointIndices[i];
            PlatformStopPoint[i] = bezierPath.points[index].distanceAlongPath;
        }
    }
}


public class RailGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetupMetroLines();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int numberOfLines = 0;
    public Line[] metroLines = null;
    public GameObject prefabRail;
    
    void SetupMetroLines()
    {
        metroLines = new Line[numberOfLines];
        for (int i = 0; i < numberOfLines; i++)
        {
            // Find all of the relevant RailMarkers in the scene for this line
            List<RailMarker> _relevantMarkers = FindObjectsOfType<RailMarker>().Where(m => m.metroLineID == i)
                .OrderBy(m => m.pointIndex).ToList();

            // Only continue if we have something to work with
            if (_relevantMarkers.Count > 1)
            {
                Line _newLine = new Line();
                _newLine.Create_RailPath(_relevantMarkers);
                metroLines[i] = _newLine;
                
                
                // Now, let's lay the rail meshes
                float dist = 0f;
                while (dist < _newLine.bezierPath.GetPathDistance())
                {
                    // convert distance value to 0 - 1 range
                    float distAsInterpolant = dist / _newLine.bezierPath.GetPathDistance();

                    Vector3 railPosition = _newLine.bezierPath.Get_Position(distAsInterpolant);

                    Vector3 normalAtRailPosition = _newLine.bezierPath.Get_NormalAtPosition(distAsInterpolant);
                    
                    // rail that has been created
                    GameObject _rail = GameObject.Instantiate(prefabRail);
                    
                    //            _RAIL.GetComponent<Renderer>().material.color = lineColour;
                    _rail.transform.position = railPosition;
                    _rail.transform.LookAt(railPosition - normalAtRailPosition);
                    dist += MetroDefines.RAIL_SPACING;
                }
                
            }
            else
            {
                Debug.LogWarning("Insufficient RailMarkers found for line: " + i +
                                 ", you need to add the outbound points");
            }
        }

        // now destroy all RailMarkers
        foreach (RailMarker _RM in FindObjectsOfType<RailMarker>())
        {
            Destroy(_RM);
        }
        
    }

    
    
}
