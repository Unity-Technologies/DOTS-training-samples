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
        
        railArchetype = EntityManager.CreateArchetype(typeof(RailPoint), typeof(RailPointDistance), typeof(RailLength),
                                                      typeof(TrainCount), typeof(CarriageCount), typeof(BufferPlatform));
        
        RequireSingletonForUpdate<MetroBuilder>();
    }

    protected override void OnUpdate()
    {
        var metroBuilder = GetSingleton<MetroBuilder>();

        NativeList<Entity> allPlatforms = new NativeList<Entity>(Allocator.Temp); // Used to collect all platforms that are spawned in all lines

        Entities.WithStructuralChanges()
            .ForEach((Entity entity,
                      in DynamicBuffer<RailMarkerPosition> railMarkerPositions,
                      in DynamicBuffer<RailMarkerPlatformIndex> railMarkerPlatformIndices,
                      in TrainCount trainCount,
                      in CarriageCount carriageCount) =>
            {
                var railIndices = railMarkerPlatformIndices.ToNativeArray(Allocator.Temp);
                Create_RailPath(railMarkerPositions, railIndices, metroBuilder.PlatformPrefab, metroBuilder.RailPrefab, trainCount, carriageCount, allPlatforms);
                railIndices.Dispose();
                EntityManager.DestroyEntity(entity);
            }).Run();

        Entities
            .WithName("connect_adjacent_platforms")
            .WithStructuralChanges()
            .ForEach((Entity platform, ref AdjacentPlatform adjacency, in Translation translation, in Rotation rotation) =>
            {
                // 1. Find closest platform
                float3 platformCenter = translation.Value + math.rotate(rotation.Value, new float3(10.5f, 0, 4));

                float closestDistanceSquared = 0.0f;
                Entity closestPlatform = Entity.Null;
                for (int i = 0; i < allPlatforms.Length; i++)
                {
                    Entity otherPlatform = allPlatforms[i];
                    if (platform == otherPlatform) continue;

                    float3 otherPlatformCenter = GetComponent<Translation>(otherPlatform).Value + math.rotate(GetComponent<Rotation>(otherPlatform).Value, new float3(10.5f, 0, 4));

                    float distanceSquared = math.distancesq(platformCenter, otherPlatformCenter);
                    if (closestPlatform == Entity.Null || distanceSquared < closestDistanceSquared)
                    {
                        closestDistanceSquared = distanceSquared;
                        closestPlatform = otherPlatform;
                    }
                }

                // 2. Find opposite platform
                var sameStationPlatforms = GetBuffer<SameStationPlatformBufferElementData>(platform);
                Entity oppositePlatform = sameStationPlatforms[0].Value;

                // 3. If the closest platform isn't the opposite platform, make closest adjacent to this
                if (closestPlatform != Entity.Null && closestPlatform != oppositePlatform)
                {
                    adjacency.Value = closestPlatform;
                }

                // 4. Propagate known same station platforms
                if (adjacency.Value != Entity.Null)
                {
                    // Acquire knowledge of the adjacent platform and every platform it already knew
                    var knownByAdjacent = GetBuffer<SameStationPlatformBufferElementData>(adjacency.Value);

                    NativeList<Entity> newlyKnownPlatforms = new NativeList<Entity>(1 + knownByAdjacent.Length, Allocator.Temp);

                    newlyKnownPlatforms.Add(adjacency.Value);
                    for (int i = 0; i < knownByAdjacent.Length; i++)
                    {
                        newlyKnownPlatforms.Add(knownByAdjacent[i].Value);
                    }

                    // Find to whom it needs to propagate the new info
                    var knownByMe = GetBuffer<SameStationPlatformBufferElementData>(platform);

                    NativeList<Entity> oldKnownPlatforms = new NativeList<Entity>(1 + knownByMe.Length, Allocator.Temp);

                    oldKnownPlatforms.Add(platform);
                    for (int i = 0; i < knownByMe.Length; i++)
                    {
                        oldKnownPlatforms.Add(knownByMe[i].Value);
                    }

                    // Propagate
                    for (int i = 0; i < oldKnownPlatforms.Length; i++)
                    {
                        Entity target = oldKnownPlatforms[i];

                        var knownBuffer = EntityManager.GetBuffer<SameStationPlatformBufferElementData>(target);
                        for (int j = 0; j < newlyKnownPlatforms.Length; j++)
                        {
                            var source = newlyKnownPlatforms[j];

                            if (source == target) continue;

                            bool alreadyExists = false;
                            for (int k = 0; k < knownBuffer.Length; k++)
                            {
                                var known = knownBuffer[k].Value;
                                if (source == known)
                                {
                                    alreadyExists = true;
                                    break;
                                }
                            }

                            if (!alreadyExists)
                            {
                                knownBuffer.Add(new SameStationPlatformBufferElementData() { Value = source });
                            }
                        }
                    }

                    // Free lists
                    newlyKnownPlatforms.Dispose();
                    oldKnownPlatforms.Dispose();
                }
            }).Run();

        allPlatforms.Dispose();
    }

    void Create_RailPath(DynamicBuffer<RailMarkerPosition> positions, NativeArray<RailMarkerPlatformIndex> platformIndices, Entity platformPrefab,
                         Entity railPrefab, TrainCount trainCount, CarriageCount carriageCount, NativeList<Entity> allPlatforms)
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

        var platformCount = platformIndices.Length * 2;
        var platforms = EntityManager.Instantiate(platformPrefab, platformCount, Allocator.Temp);
        var platformTranslations = new NativeArray<float3>(platformCount, Allocator.Temp);
        allPlatforms.AddRange(platforms);

        // now that the rails have been laid - let's put the platforms on
        int totalPoints = bezierPath.points.Count;
        for (int i = 0; i < platformIndices.Length; i++)
        {
            Entity outboundPlatform = platforms[i];
            int _plat_END = platformIndices[i];
            int _plat_START = _plat_END - 1;
            platformTranslations[i] = InitializePlatform(outboundPlatform, bezierPath, _plat_START, _plat_END, -3f);

            var inboundPlatformIndex = platforms.Length - 1 - i;
            Entity inboundPlatform = platforms[inboundPlatformIndex];
            int opposite_START = totalPoints - (_plat_END + 1);
            int opposite_END = totalPoints - _plat_END;
            platformTranslations[inboundPlatformIndex] = InitializePlatform(inboundPlatform, bezierPath, opposite_START, opposite_END, -3f);

            // pair these platforms as opposites
            var outboundSameStationPlatforms = GetBuffer<SameStationPlatformBufferElementData>(outboundPlatform);
            outboundSameStationPlatforms.Add(new SameStationPlatformBufferElementData() { Value = inboundPlatform });

            var inboundSameStationPlatforms = GetBuffer<SameStationPlatformBufferElementData>(inboundPlatform);
            inboundSameStationPlatforms.Add(new SameStationPlatformBufferElementData() { Value = outboundPlatform });
        }

        // Hopefully we don't need that, since we tried to maintain the order at initialization time above.
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
        EntityManager.SetComponentData(railEntity, trainCount);
        EntityManager.SetComponentData(railEntity, carriageCount);

        var railPoints = new NativeList<RailPoint>(Allocator.Temp);

        // Now, let's lay the rail meshes
        float _DIST = 0f;
        while (_DIST < bezierPath.GetPathDistance())
        {
            float _DIST_AS_RAIL_FACTOR = Get_distanceAsRailProportion(bezierPath, _DIST);
            Vector3 _RAIL_POS = bezierPath.Get_Position(_DIST_AS_RAIL_FACTOR);
            Vector3 _RAIL_ROT = bezierPath.Get_NormalAtPosition(_DIST_AS_RAIL_FACTOR);

            railPoints.Add((float3)_RAIL_POS);

            var rail = EntityManager.Instantiate(railPrefab);
            EntityManager.SetComponentData(rail, new Rotation { Value = quaternion.LookRotation(_RAIL_ROT, new float3(0f, 1f, 0f))});
            EntityManager.SetComponentData(rail, new Translation { Value = _RAIL_POS });
            _DIST += Metro.RAIL_SPACING;
        }

        // Calculate final world-space distances
        var railPointDistances = new NativeArray<RailPointDistance>(railPoints.Length, Allocator.Temp);

        var distance = 0f;
        for (int i = 0; i < railPoints.Length - 1; i++)
        {
            distance += math.length((float3) railPoints[i] - (float3) railPoints[i + 1]);
            railPointDistances[i] = distance;
        }
        distance += math.length((float3) railPoints[0] - (float3) railPoints[railPoints.Length - 1]);
        railPointDistances[railPointDistances.Length - 1] = distance;

        EntityManager.GetBuffer<RailPoint>(railEntity).AddRange(railPoints);
        EntityManager.GetBuffer<RailPointDistance>(railEntity).AddRange(railPointDistances);

        EntityManager.SetComponentData<RailLength>(railEntity, distance);
        
        // Setup platforms positions
        for (int i = 0; i < platformCount; i++)
        {
            var platformTranslation = platformTranslations[i];
            var nearestIndex = 0;
            var nearestSqrDistance = math.distancesq(railPoints[0], platformTranslation);
            
            for (int j = 1; j < railPoints.Length; j++)
            {
                var point = railPoints[j];
                var sqrDistance = math.distancesq(point, platformTranslation);
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearestIndex = j;
                }
            }
            
            EntityManager.AddComponentData<Position>(platforms[i], (float)railPointDistances[nearestIndex]);
        }

        EntityManager.GetBuffer<BufferPlatform>(railEntity).AddRange(platforms.Reinterpret<BufferPlatform>());

        railPoints.Dispose();
        railPointDistances.Dispose();
        platforms.Dispose();
        platformTranslations.Dispose();
    }

    float3 InitializePlatform(Entity platform, BezierPath bezierPath, int _index_platform_START, int _index_platform_END, float lookAtOffset)
    {
        BezierPoint _PT_START = bezierPath.points[_index_platform_START];
        BezierPoint _PT_END = bezierPath.points[_index_platform_END];

        EntityManager.SetComponentData(platform, new Translation { Value = _PT_END.location });
        
        var lookAtPoint = bezierPath.GetPoint_PerpendicularOffset(_PT_END, lookAtOffset);
        EntityManager.SetComponentData(platform, new Rotation
        {
            Value = quaternion.LookRotation(math.normalize(lookAtPoint - _PT_END.location), new float3(0f, 1f, 0f))
        });
        
        return _PT_END.location;
    }

    float Get_distanceAsRailProportion(BezierPath bezierPath, float _realDistance)
    {
        return _realDistance / bezierPath.GetPathDistance();
    }
}
