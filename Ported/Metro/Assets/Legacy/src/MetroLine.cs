using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;


public class MetroLine
{
    public string lineName;
    public int metroLine_index;
    public Color lineColour;
    public BezierPath bezierPath;
    public List<Train> trains;
    public List<Platform> platforms;
    public int maxTrains;
    public float maxTrainSpeed;
    public Vector3[] railPath;
    public int carriagesPerTrain;
    public float train_accelerationStrength = 0.0003f;
    public float train_brakeStrength = 0.01f;
    public float train_friction = 0.95f;
    public float speedRatio;
    public float carriageLength_onRail;
   
    public MetroLine(int metroLineIndex, int _maxTrains)
    {
        metroLine_index = metroLineIndex;
        maxTrains = _maxTrains;
        trains = new List<Train>();
        platforms = new List<Platform>();
        Update_ValuesFromMetro();
    }

    void Update_ValuesFromMetro()
    {
        Metro m = Metro.INSTANCE;
        lineName = m.LineNames[metroLine_index];
        lineColour = m.LineColours[metroLine_index];
        carriagesPerTrain = m.carriagesPerTrain[metroLine_index];
        if (carriagesPerTrain <= 0)
        {
            carriagesPerTrain = 1;
        }

        maxTrainSpeed = m.maxTrainSpeed[metroLine_index];
    }

    public void Create_RailPath(List<RailMarker> _outboundPoints)
    {
        bezierPath = new BezierPath();
        List<BezierPoint> _POINTS = bezierPath.points;
        int total_outboundPoints = _outboundPoints.Count;
        Vector3 currentLocation = Vector3.zero;

        // - - - - - - - - - - - - - - - - - - - - - - - -  OUTBOUND points
        for (int i = 0; i < total_outboundPoints; i++)
        {
            bezierPath.AddPoint(_outboundPoints[i].transform.position);
        }

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
        float platformOffset = Metro.BEZIER_PLATFORM_OFFSET;
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
        carriageLength_onRail = Get_distanceAsRailProportion(TrainCarriage.CARRIAGE_LENGTH) +
                                Get_distanceAsRailProportion(TrainCarriage.CARRIAGE_SPACING);

        // now that the rails have been laid - let's put the platforms on
        int totalPoints = bezierPath.points.Count;
        for (int i = 1; i < _outboundPoints.Count; i++)
        {
            int _plat_END = i;
            int _plat_START = i - 1;
            if (_outboundPoints[_plat_END].railMarkerType == RailMarkerType.PLATFORM_END &&
                _outboundPoints[_plat_START].railMarkerType == RailMarkerType.PLATFORM_START)
            {
                Platform _ouboundPlatform = AddPlatform(_plat_START, _plat_END);
                // now add an opposite platform!
                int opposite_START = totalPoints - (i + 1);
                int opposite_END = totalPoints - i;
                Platform _oppositePlatform = AddPlatform(opposite_START, opposite_END);
                _oppositePlatform.transform.eulerAngles =
                    _ouboundPlatform.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f);
                ;

                // pair these platforms as opposites
                _ouboundPlatform.PairWithOppositePlatform(_oppositePlatform);
                _oppositePlatform.PairWithOppositePlatform(_ouboundPlatform);
            }
        }

        var sortedPlatforms = from _PLATFORM in platforms
            orderby _PLATFORM.point_platform_START.index
            select _PLATFORM;
        platforms = sortedPlatforms.ToList();
        for (int i = 0; i < platforms.Count; i++)
        {
            Platform _P = platforms[i];
            _P.platformIndex = i;
            _P.nextPlatform = platforms[(i + 1) % platforms.Count];
        }

        speedRatio = bezierPath.GetPathDistance() * maxTrainSpeed;
        
        // Now, let's lay the rail meshes
        float _DIST = 0f;
        Metro _M = Metro.INSTANCE;
        while (_DIST < bezierPath.GetPathDistance())
        {
            float _DIST_AS_RAIL_FACTOR = Get_distanceAsRailProportion(_DIST);
            Vector3 _RAIL_POS = Get_PositionOnRail(_DIST_AS_RAIL_FACTOR);
            Vector3 _RAIL_ROT = Get_RotationOnRail(_DIST_AS_RAIL_FACTOR);
            GameObject _RAIL = (GameObject) Metro.Instantiate(_M.prefab_rail);
//            _RAIL.GetComponent<Renderer>().material.color = lineColour;
            _RAIL.transform.position = _RAIL_POS;
            _RAIL.transform.LookAt(_RAIL_POS - _RAIL_ROT);
            _DIST += Metro.RAIL_SPACING;
        }

    }

    Platform AddPlatform(int _index_platform_START, int _index_platform_END)
    {
        BezierPoint _PT_START = bezierPath.points[_index_platform_START];
        BezierPoint _PT_END = bezierPath.points[_index_platform_END];
        GameObject platform_OBJ =
            (GameObject) Metro.Instantiate(Metro.INSTANCE.prefab_platform, _PT_END.location, Quaternion.identity);
        Platform platform = platform_OBJ.GetComponent<Platform>();
        platform.SetupPlatform(this, _PT_START, _PT_END);
        platform_OBJ.transform.LookAt(bezierPath.GetPoint_PerpendicularOffset(_PT_END, -3f));
        platforms.Add(platform);
        return platform;
    }

    public void AddTrain(int _trainIndex, float _position)
    {
        trains.Add(new Train(_trainIndex, metroLine_index, _position, carriagesPerTrain));
    }

    public void UpdateTrains()
    {
        foreach (Train _t in trains)
        {
            _t.Update();
        }
    }

    public Vector3 Get_PositionOnRail(float _pos)
    {
        return bezierPath.Get_Position(_pos);
    }

    public Vector3 Get_RotationOnRail(float _pos)
    {
        return bezierPath.Get_NormalAtPosition(_pos);
    }

    public float Get_distanceAsRailProportion(float _realDistance)
    {
        return _realDistance / bezierPath.GetPathDistance();
    }

    public float Get_proportionAsDistance(float _proportion)
    {
        return bezierPath.GetPathDistance() * _proportion;
    }

    public int Get_RegionIndex(float _proportion)
    {
        return bezierPath.GetRegionIndex(Get_proportionAsDistance(_proportion));
    }

    public Platform Get_NextPlatform(float _currentPosition, Platform _currentPlatform = null)
    {
        Platform result = null;
        int totalPoints = bezierPath.points.Count;
        int currentRegionIndex = Get_RegionIndex(_currentPosition);

        // walk along the points and return the next END we encounter
        for (int i = 0; i < totalPoints; i++)
        {
            int testIndex = (currentRegionIndex + i) % totalPoints;
            // is TEST INDEX a platform end?
            foreach (Platform _P in platforms)
            {
                if (_P.point_platform_START.index == testIndex && _currentPlatform != _P)
                {
                    return _P;
                }
            }
        }

        return result;
    }

    public bool Has_ConnectionToMetroLine(MetroLine _targetLine)
    {
        foreach (Platform _P in platforms)
        {
            foreach (Platform _ADJ in _P.adjacentPlatforms)
            {
                if (_ADJ.parentMetroLine == _targetLine)
                {
                    return true;
                }
            }
        }

        return false;
    }
}