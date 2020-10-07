using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class LineCreationSystem : SystemBase
{
    EntityArchetype railArchetype;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        railArchetype = EntityManager.CreateArchetype(typeof(RailPoint), typeof(RailLength), typeof(TrainCount));
        
        RequireSingletonForUpdate<MetroBuilder>();
    }

    protected override void OnUpdate()
    {
        var metroBuilder = GetSingleton<MetroBuilder>();

        Entities.WithStructuralChanges()
            .ForEach((Entity entity,
                      in DynamicBuffer<RailMarkerPosition> railMarkerPositions,
                      in DynamicBuffer<RailMarkerPlatformIndex> railMarkerPlatformIndices,
                      in TrainCount trainCount) =>
            {
                Create_RailPath(railMarkerPositions, railMarkerPlatformIndices, metroBuilder.RailPrefab, trainCount);
                EntityManager.DestroyEntity(entity);
            }).Run();
    }

    void Create_RailPath(DynamicBuffer<RailMarkerPosition> positions, DynamicBuffer<RailMarkerPlatformIndex> indices, Entity railPrefab,
                           TrainCount trainCount)
    {
        var bezierPath = new BezierPath();
        List<BezierPoint> _POINTS = bezierPath.points;
        int total_outboundPoints = positions.Length;

        // - - - - - - - - - - - - - - - - - - - - - - - -  OUTBOUND points
        for (int i = 0; i < total_outboundPoints; i++)
        {
            bezierPath.AddPoint(positions[i].Value);
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
        /* carriageLength_onRail = Get_distanceAsRailProportion(bezierPath, TrainCarriage.CARRIAGE_LENGTH) +
                                    Get_distanceAsRailProportion(bezierPath, TrainCarriage.CARRIAGE_SPACING); */

        /*
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
        */

        var railEntity = EntityManager.CreateEntity(railArchetype);
        EntityManager.SetComponentData(railEntity, new RailLength { Value = bezierPath.GetPathDistance() });
        EntityManager.SetComponentData(railEntity, trainCount);

        // Now, let's lay the rail meshes
        float _DIST = 0f;
        while (_DIST < bezierPath.GetPathDistance())
        {
            float _DIST_AS_RAIL_FACTOR = Get_distanceAsRailProportion(bezierPath, _DIST);
            Vector3 _RAIL_POS = bezierPath.Get_Position(_DIST_AS_RAIL_FACTOR);
            Vector3 _RAIL_ROT = bezierPath.Get_NormalAtPosition(_DIST_AS_RAIL_FACTOR);

            EntityManager.GetBuffer<RailPoint>(railEntity).Add(new RailPoint { Position = _RAIL_POS });
            
            var rail = EntityManager.Instantiate(railPrefab);
            EntityManager.SetComponentData(rail, new Rotation { Value = quaternion.LookRotation(_RAIL_ROT, new float3(0f, 1f, 0f))});
            EntityManager.SetComponentData(rail, new Translation { Value = _RAIL_POS });
            _DIST += Metro.RAIL_SPACING;
        }
    }

    float Get_distanceAsRailProportion(BezierPath bezierPath, float _realDistance)
    {
        return _realDistance / bezierPath.GetPathDistance();
    }
}
