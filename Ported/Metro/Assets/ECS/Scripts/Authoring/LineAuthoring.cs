using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LineAuthoring : MonoBehaviour
{
    public Color Colour;
    public int MaxTrains;
    public int CarriagesPerTrain;
    public float MaxTrainSpeed = 0.002f;
}

public class LineBaker : Baker<LineAuthoring>
{
    public override void Bake(LineAuthoring authoring)
    {
        AddComponent(new Line
        {
            Name = authoring.name,
            Colour = authoring.Colour,
            MaxTrains = authoring.MaxTrains,
            CarriagesPerTrain = authoring.CarriagesPerTrain,
            MaxTrainSpeed = authoring.MaxTrainSpeed
        });

        using var points = GetPoints();
        using var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var bezierData = ref blobBuilder.ConstructRoot<BezierData>();
        
        var pointsBuilder = blobBuilder.Allocate(ref bezierData.Points, points.Length);
        for (var p = 0; p < points.Length; p++)
            pointsBuilder[p] = points[p];

        AddComponent(new BezierPath
        {
            Data = blobBuilder.CreateBlobAssetReference<BezierData>(Allocator.Persistent)
        });
    }

    private NativeArray<BezierPoint> GetPoints()
    {
        var markers = GetComponentsInChildren<RailMarkerAuthoring>();
        var points = new NativeArray<BezierPoint>(markers.Length, Allocator.Persistent);
        for (var m = 0; m < markers.Length; m++)
        {
            points[m] = new BezierPoint
            {
                location = markers[m].transform.position,
            };
        }

        return points;
    }

//     private void CreatePointsBuffer(List<RailMarker> _outboundPoints)
//     {
//         var points = new List<BezierPoint>();
//         var total_outboundPoints = _outboundPoints.Count;
//         var currentLocation = Vector3.zero;
//
//         // - - - - - - - - - - - - - - - - - - - - - - - -  OUTBOUND points
//         for (int i = 0; i < total_outboundPoints; i++)
//         {
//             bezierPath.AddPoint(_outboundPoints[i].transform.position);
//         }
//
//         // fix the OUTBOUND handles
//         for (int i = 0; i <= total_outboundPoints - 1; i++)
//         {
//             BezierPoint _currentPoint = points[i];
//             if (i == 0)
//             {
//                 _currentPoint.SetHandles(points[1].location - _currentPoint.location);
//             }
//             else if (i == total_outboundPoints - 1)
//             {
//                 _currentPoint.SetHandles(_currentPoint.location - points[i - 1].location);
//             }
//             else
//             {
//                 _currentPoint.SetHandles(points[i + 1].location - points[i - 1].location);
//             }
//         }
//
//         bezierPath.MeasurePath();
//
//         // - - - - - - - - - - - - - - - - - - - - - - - -  RETURN points
//         float platformOffset = Metro.BEZIER_PLATFORM_OFFSET;
//         List<BezierPoint> _RETURN_POINTS = new List<BezierPoint>();
//         for (int i = total_outboundPoints - 1; i >= 0; i--)
//         {
//             Vector3 _targetLocation = bezierPath.GetPoint_PerpendicularOffset(bezierPath.points[i], platformOffset);
//             bezierPath.AddPoint(_targetLocation);
//             _RETURN_POINTS.Add(points[points.Count - 1]);
//         }
//
//         // fix the RETURN handles
//         for (int i = 0; i <= total_outboundPoints - 1; i++)
//         {
//             BezierPoint _currentPoint = _RETURN_POINTS[i];
//             if (i == 0)
//             {
//                 _currentPoint.SetHandles(_RETURN_POINTS[1].location - _currentPoint.location);
//             }
//             else if (i == total_outboundPoints - 1)
//             {
//                 _currentPoint.SetHandles(_currentPoint.location - _RETURN_POINTS[i - 1].location);
//             }
//             else
//             {
//                 _currentPoint.SetHandles(_RETURN_POINTS[i + 1].location - _RETURN_POINTS[i - 1].location);
//             }
//         }
//
//         bezierPath.MeasurePath();
//         carriageLength_onRail = Get_distanceAsRailProportion(TrainCarriage.CARRIAGE_LENGTH) +
//                                 Get_distanceAsRailProportion(TrainCarriage.CARRIAGE_SPACING);
//
//         // now that the rails have been laid - let's put the platforms on
//         int totalPoints = bezierPath.points.Count;
//         for (int i = 1; i < _outboundPoints.Count; i++)
//         {
//             int _plat_END = i;
//             int _plat_START = i - 1;
//             if (_outboundPoints[_plat_END].railMarkerType == RailMarkerType.PLATFORM_END &&
//                 _outboundPoints[_plat_START].railMarkerType == RailMarkerType.PLATFORM_START)
//             {
//                 Platform _ouboundPlatform = AddPlatform(_plat_START, _plat_END);
//                 // now add an opposite platform!
//                 int opposite_START = totalPoints - (i + 1);
//                 int opposite_END = totalPoints - i;
//                 Platform _oppositePlatform = AddPlatform(opposite_START, opposite_END);
//                 _oppositePlatform.transform.eulerAngles =
//                     _ouboundPlatform.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f);
//                 ;
//
//                 // pair these platforms as opposites
//                 _ouboundPlatform.PairWithOppositePlatform(_oppositePlatform);
//                 _oppositePlatform.PairWithOppositePlatform(_ouboundPlatform);
//             }
//         }
//
//         var sortedPlatforms = from _PLATFORM in platforms
//             orderby _PLATFORM.point_platform_START.index
//             select _PLATFORM;
//         platforms = sortedPlatforms.ToList();
//         for (int i = 0; i < platforms.Count; i++)
//         {
//             Platform _P = platforms[i];
//             _P.platformIndex = i;
//             _P.nextPlatform = platforms[(i + 1) % platforms.Count];
//         }
//
//         speedRatio = bezierPath.GetPathDistance() * maxTrainSpeed;
//         
//         // Now, let's lay the rail meshes
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
//     }
}