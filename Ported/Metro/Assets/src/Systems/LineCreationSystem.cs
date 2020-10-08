using System.Collections.Generic;
using Unity.Collections;
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
                var railIndices = railMarkerPlatformIndices.ToNativeArray(Allocator.Temp);
                Create_RailPath(railMarkerPositions, railIndices, metroBuilder.PlatformPrefab, metroBuilder.RailPrefab, trainCount);
                railIndices.Dispose();
                EntityManager.DestroyEntity(entity);
            }).Run();
    }

    void Create_RailPath(DynamicBuffer<RailMarkerPosition> positions, NativeArray<RailMarkerPlatformIndex> platformIndices, Entity platformPrefab,
                         Entity railPrefab, TrainCount trainCount)
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
        float platformOffset = 8f;
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

        var platforms = EntityManager.Instantiate(platformPrefab, platformIndices.Length * 2, Allocator.Temp);
        
        // now that the rails have been laid - let's put the platforms on
        int totalPoints = bezierPath.points.Count;
        for (int i = 0; i < platformIndices.Length; i++)
        {
            int _plat_END = platformIndices[i];
            int _plat_START = _plat_END - 1;
            InitializePlatform(platforms[i], bezierPath, _plat_START, _plat_END, -3f);
            // now add an opposite platform!
            int opposite_START = totalPoints - (_plat_END + 1);
            int opposite_END = totalPoints - _plat_END;
            InitializePlatform(platforms[platforms.Length - 1 - i], bezierPath, opposite_START, opposite_END, -3f);

            // pair these platforms as opposites
            // TODO:
            // _ouboundPlatform.PairWithOppositePlatform(_oppositePlatform);
            // _oppositePlatform.PairWithOppositePlatform(_ouboundPlatform);
        }

        /*
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
        */

        // speedRatio = bezierPath.GetPathDistance() * maxTrainSpeed;

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

    void InitializePlatform(Entity platform, BezierPath bezierPath, int _index_platform_START, int _index_platform_END, float lookAtOffset)
    {
        BezierPoint _PT_START = bezierPath.points[_index_platform_START];
        BezierPoint _PT_END = bezierPath.points[_index_platform_END];

        EntityManager.SetComponentData(platform, new Translation { Value = _PT_END.location });
        var lookAtPoint = bezierPath.GetPoint_PerpendicularOffset(_PT_END, lookAtOffset);
        EntityManager.SetComponentData(platform, new Rotation
        {
            Value = quaternion.LookRotation(math.normalize(lookAtPoint - _PT_END.location), new float3(0f, 1f, 0f))
        });
        
        // TODO:
        // platform.SetupPlatform(this, _PT_START, _PT_END);
    }

    float Get_distanceAsRailProportion(BezierPath bezierPath, float _realDistance)
    {
        return _realDistance / bezierPath.GetPathDistance();
    }
}
