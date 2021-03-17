using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BlobCreationUtilities
{
    public static (LinePoint[] linePoints, PlatformBlob[] platforms) Create_RailPath(List<RailMarker> _outboundPoints)
    {
        List<PlatformBlob> platformBlobs = new List<PlatformBlob>();
        
        // List<Platform> platforms = new List<Platform>();
        
        var bezierPath = new BezierPath();
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
        
        // TODO Probably relevant if we need to factor in carriage length
        // carriageLength_onRail = Get_distanceAsRailProportion(TrainCarriage.CARRIAGE_LENGTH) +
        //                         Get_distanceAsRailProportion(TrainCarriage.CARRIAGE_SPACING);

        // now that the rails have been laid - let's put the platforms on
        int totalPoints = bezierPath.points.Count;
        for (int i = 1; i < _outboundPoints.Count; i++)
        {
            int _plat_END = i;
            int _plat_START = i - 1;
            if (_outboundPoints[_plat_END].railMarkerType == RailMarkerType.PLATFORM_END &&
                _outboundPoints[_plat_START].railMarkerType == RailMarkerType.PLATFORM_START)
            {
                PlatformBlob _ouboundPlatform = AddPlatform(_plat_START, _plat_END);
                // now add an opposite platform!
                int opposite_START = totalPoints - (i + 1);
                int opposite_END = totalPoints - i;
                PlatformBlob _oppositePlatform = AddPlatform(opposite_START, opposite_END);
                
                // TODO We need to spawn the Entity version of the prefab somewhere here skipping
                // _oppositePlatform.transform.eulerAngles =
                //     _ouboundPlatform.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f);
                ;

                // pair these platforms as opposites
                // _ouboundPlatform.PairWithOppositePlatform(_oppositePlatform);
                // _oppositePlatform.PairWithOppositePlatform(_ouboundPlatform);
            }
        }

        var sortedPlatforms = from _PLATFORM in platformBlobs orderby _PLATFORM.PlatformStartIndex select _PLATFORM;
        platformBlobs = sortedPlatforms.ToList();
        
        // for (int i = 0; i < platforms.Count; i++)
        // {
        //     Platform _P = platforms[i];
        //     _P.platformIndex = i;
        //     _P.nextPlatform = platforms[(i + 1) % platforms.Count];
        // }
        //
        // speedRatio = bezierPath.GetPathDistance() * maxTrainSpeed;
        
        // TODO Now, let's lay the rail meshes
//         float _DIST = 0f;
//         Metro _M = Metro.INSTANCE;
//         while (_DIST < bezierPath.GetPathDistance())
//         {
//             float _DIST_AS_RAIL_FACTOR = Get_distanceAsRailProportion(_DIST);
//             Vector3 _RAIL_POS = Get_PositionOnRail(_DIST_AS_RAIL_FACTOR);
//             Vector3 _RAIL_ROT = Get_RotationOnRail(_DIST_AS_RAIL_FACTOR);
//             GameObject _RAIL = (GameObject) Metro.Instantiate(_M.prefab_rail);
// //            _RAIL.GetComponent<Renderer>().material.color = lineColour;
//             _RAIL.transform.position = _RAIL_POS;
//             _RAIL.transform.LookAt(_RAIL_POS - _RAIL_ROT);
//             _DIST += Metro.RAIL_SPACING;
//         }


        PlatformBlob AddPlatform(int _index_platform_START, int _index_platform_END)
        {
            var platform = new PlatformBlob {PlatformStartIndex = _index_platform_START, PlatformEndIndex = _index_platform_END};
            platformBlobs.Add(platform);
            return platform;
        }
        
        // TODO Very Important bits that instantiate platform models, skipping for now
        // Platform AddPlatform(int _index_platform_START, int _index_platform_END)
        // {
        //     
        //     // BezierPoint _PT_START = bezierPath.points[_index_platform_START];
        //     // BezierPoint _PT_END = bezierPath.points[_index_platform_END];
        //     // GameObject platform_OBJ =
        //     //     (GameObject) Metro.Instantiate(Metro.INSTANCE.prefab_platform, _PT_END.location, Quaternion.identity);
        //     // Platform platform = platform_OBJ.GetComponent<Platform>();
        //     // platform.SetupPlatform(this, _PT_START, _PT_END);
        //     // platform_OBJ.transform.LookAt(bezierPath.GetPoint_PerpendicularOffset(_PT_END, -3f));
        //     // platforms.Add(platform);
        //     // return platform;
        // }

        var linePoints = new LinePoint[bezierPath.points.Count];
        // Converting Bezier to Our line points
        for (var i = 0; i < bezierPath.points.Count; i++)
        {
            var bezierPoint = bezierPath.points[i];
            linePoints[i] = new LinePoint
            {
                index = bezierPoint.index,
                location = bezierPoint.location,
                handle_in = bezierPoint.handle_in,
                handle_out = bezierPoint.handle_out,
                distanceAlongPath = bezierPoint.distanceAlongPath
            };
        }

        return (linePoints.ToArray(), platformBlobs.ToArray());
    }
}
