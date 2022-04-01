﻿using Unity.Entities;
using Unity.Mathematics;

using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityGizmos = UnityEngine.Gizmos;
using UnityGUI = UnityEngine.GUI;
using UnityTransform = UnityEngine.Transform;
using UnityVector3 = UnityEngine.Vector3;
using UnityColor = UnityEngine.Color;

public class LineAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private int carriageCount;
    [SerializeField] private float carriageLength;
    [SerializeField] private int trainCount;
    [SerializeField] private float maxSpeed;
    [SerializeField] private bool debugRender;
    [SerializeField] private Color lineColor;

    [SerializeField] private int spawnMultiplier = 1;
    [SerializeField] private float spawnMultipleGridSize = 100.0f;
    [SerializeField] private float spawnMultipleGridSizeZScale = 1.0f;

    struct StationData
    {
        public float3 position;
        public quaternion rotation;
        public float outboundBezierPosition;
        public float returnBezierPosition;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var children = gameObject.GetComponentsInChildren<LineMarkerAuthoring>();

        if (spawnMultiplier > 1)
        {
            for (int i = 0; i < spawnMultiplier; i++)
            {
                for (int j = 0; j < spawnMultiplier; j++)
                {
                    float3 offset = new float3(spawnMultipleGridSize * i, 0, spawnMultipleGridSize * spawnMultipleGridSizeZScale * j);

                    Entity gridDuplicate = conversionSystem.CreateAdditionalEntity(this);
                    SpawnLine(children, dstManager, gridDuplicate, conversionSystem, offset);
                }
            }
        }
        else
        {
            SpawnLine(children, dstManager, entity, conversionSystem, float3.zero);
        }

    }

    void SpawnLine(LineMarkerAuthoring[] children, EntityManager dstManager, Entity entity, GameObjectConversionSystem conversionSystem, float3 offset)
    {
        dstManager.AddComponent<LineMarkerBufferElement>(entity);

        var dynamicBuffer = dstManager.GetBuffer<LineMarkerBufferElement>(entity);
        foreach (var childMarker in children)
        {
            dynamicBuffer.Add(new LineMarkerBufferElement
            {
                IsPlatform = childMarker.MarkerType == LineMarkerType.Platform,
                Position = childMarker.gameObject.transform.position
            });
        }

        // Bezier Curve!
        dstManager.AddComponent<BezierPointBufferElement>(entity);

        var bezierCurve = dstManager.GetBuffer<BezierPointBufferElement>(entity);
        List<StationData> stationIndices;
        float curveDistance = PopulateBezierFromMarkers(ref bezierCurve, children.ToList(), offset, out stationIndices);

        dstManager.AddComponent<StationPositionBufferElement>(entity);

        NativeArray<StationPositionBufferElement> stationPositionNativeArray;
        CreateStations(dstManager, stationIndices, out stationPositionNativeArray, conversionSystem);
        var stationPositions = dstManager.GetBuffer<StationPositionBufferElement>(entity);
        stationPositions.AddRange(stationPositionNativeArray);

        dstManager.AddComponentData(entity, new LineComponent
        {
            CarriageCount = carriageCount,
            CarriageLength = carriageLength,
            TrainCount = trainCount,
            MaxSpeed = maxSpeed,
            LineLength = curveDistance
        });

        if (debugRender)
        {
            dstManager.AddComponent<ShouldDebugRenderBezier>(entity);
        }

        dstManager.AddComponentData<LineTotalDistanceComponent>(entity,
            new LineTotalDistanceComponent { Value = curveDistance });
        dstManager.AddComponentData<LineIDComponent>(entity, new LineIDComponent { Line = entity });
        dstManager.AddComponentData<ColorComponent>(entity, new ColorComponent() { Value = (Vector4)lineColor });
    }

    // Populates a dynamic buffer with bezier curve information, returning the total distance.
    // An analogue of MetroLine.Create_RailPath
    float PopulateBezierFromMarkers(ref DynamicBuffer<BezierPointBufferElement> bezierCurve,
        List<LineMarkerAuthoring> markers, float3 trackOffset, out List<StationData> stations)
    {
        float totalPathDistance = 0;
        int totalMarkers = markers.Count;
        float3 currentLocation = float3.zero;

        List<int> stationIndices = new List<int>();
        stations = new List<StationData>();

        // - - - - - - - - - - - - - - - - - - - - - - - -  OUTBOUND points
        for (int i = 0; i < totalMarkers; i++)
        {
            BezierHelpers.AddPoint(ref bezierCurve, (float3)(markers[i].transform.position) + trackOffset);
        }

        // fix the OUTBOUND handles
        for (int i = 0; i <= totalMarkers - 1; i++)
        {
            if (markers[i].MarkerType == LineMarkerType.Platform)
            {
                stationIndices.Add(i);
            }


            BezierPointBufferElement currentPoint = bezierCurve[i];
            if (i == 0)
            {
                currentPoint = BezierHelpers.SetHandles(currentPoint, bezierCurve[1].Location - currentPoint.Location);

            }
            else if (i == totalMarkers - 1)
            {
                currentPoint = BezierHelpers.SetHandles(currentPoint, currentPoint.Location - bezierCurve[i - 1].Location);
            }
            else
            {
                currentPoint = BezierHelpers.SetHandles(currentPoint, bezierCurve[i + 1].Location - bezierCurve[i - 1].Location);
            }

            bezierCurve[i] = currentPoint;
        }

        totalPathDistance = BezierHelpers.MeasurePath(ref bezierCurve);

        // - - - - - - - - - - - - - - - - - - - - - - - -  RETURN points
        float platformOffset = BezierHelpers.BEZIER_PLATFORM_OFFSET;
        for (int i = totalMarkers - 1; i >= 0; i--)
        {
            float3 targetLocation =
                BezierHelpers.GetPointPerpendicularOffset(bezierCurve, totalPathDistance, bezierCurve[i], platformOffset);
            BezierHelpers.AddPoint(ref bezierCurve, targetLocation);
        }

        // This could be off-by-one
        int returnPointOffset = totalMarkers;

        // fix the RETURN handles
        for (int i = 0, j = stations.Count - 1; i <= totalMarkers - 1; i++)
        {
            BezierPointBufferElement currentReturnPoint = bezierCurve[i + returnPointOffset];
            if (i == 0)
            {
                currentReturnPoint = BezierHelpers.SetHandles(currentReturnPoint, bezierCurve[1 + returnPointOffset].Location - currentReturnPoint.Location);
            }
            else if (i == totalMarkers - 1)
            {
                currentReturnPoint = BezierHelpers.SetHandles(currentReturnPoint, currentReturnPoint.Location - bezierCurve[i - 1 + returnPointOffset].Location);
            }
            else
            {
                currentReturnPoint = BezierHelpers.SetHandles(currentReturnPoint, bezierCurve[i + 1 + returnPointOffset].Location - bezierCurve[i - 1 + returnPointOffset].Location);
            }

            bezierCurve[i + returnPointOffset] = currentReturnPoint;
        }

        totalPathDistance = BezierHelpers.MeasurePath(ref bezierCurve);

        for (int i = 0; i < stationIndices.Count; i++)
        {
            float t = bezierCurve[stationIndices[i]].DistanceAlongPath / totalPathDistance;

            var lookRot = quaternion.LookRotation(
                BezierHelpers.GetNormalAtPosition(bezierCurve, totalPathDistance, t),
                new float3(0, 1, 0));

            StationData stationData = new StationData
            {
                position = (bezierCurve[stationIndices[i]].Location +
                    bezierCurve[(2 * totalMarkers) - 1 - stationIndices[i]].Location) / 2,
                rotation = lookRot,
                outboundBezierPosition = bezierCurve[stationIndices[i]].DistanceAlongPath / totalPathDistance,
                returnBezierPosition = bezierCurve[(2 * totalMarkers) - 1 - stationIndices[i]].DistanceAlongPath / totalPathDistance,
            };
            stations.Add(stationData);
        }


        return totalPathDistance;
    }

    void CreateStations(EntityManager dstManager, List<StationData> stations,
        out NativeArray<StationPositionBufferElement> stationPositions, GameObjectConversionSystem conversionSystem)
    {
        StationPositionBufferElement[] stationPositionArray =
            new StationPositionBufferElement[stations.Count * 2];

        for (int i = 0; i < stations.Count; i++)
        {
            Entity station = conversionSystem.CreateAdditionalEntity(this);
            float platormARelativePosition = stations[i].outboundBezierPosition;
            float platormBRelativePosition = stations[i].returnBezierPosition;

            stationPositionArray[i].positionAlongRail = platormARelativePosition;
            stationPositionArray[2 * stations.Count - 1 - i].positionAlongRail = platormBRelativePosition;

            dstManager.AddComponentData(station, new StationComponent
            {
                PlatformATrackPosition = platormARelativePosition,
                PlatformBTrackPosition = platormBRelativePosition
            });
            dstManager.AddComponentData(station, new Translation { Value = stations[i].position });
            dstManager.AddComponentData(station, new Rotation { Value = stations[i].rotation });
        }

        stationPositions = new NativeArray<StationPositionBufferElement>(stationPositionArray, Allocator.Temp);
    }
}
