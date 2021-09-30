using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;

public class SplineDataAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] float returnRailsOffset = 10.0f;
    [SerializeField] float splineHandleGain = 1f;

    [Header("Train Count Range")]
    [SerializeField] int trainCountLower = 3;
    [SerializeField] int trainCountUpper = 6;
    
    [Header("Carriages Per Train Range")]
    [SerializeField] int carriagesPerTrainLower = 3;
    [SerializeField] int carriagesPerTrainUpper = 8;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        using (var splineBlobBuilder = new BlobBuilder(Allocator.Temp))
        {
            ref var splineDataArray = ref splineBlobBuilder.ConstructRoot<SplineBlobAssetArray>();

            var splineArray = splineBlobBuilder.Allocate(ref splineDataArray.splineBlobAssets, transform.childCount);
            var lineId = 0;
            var randomizer = new Random(1234);
            foreach (Transform child in transform)
            {
                var railMarkers = child.GetComponentsInChildren<RailMarker>();
                ref var newSplineBlobAsset = ref splineArray[lineId++];
                var fullMarkersData = CreateActualRailMarkers(railMarkers, out var nbPlatforms);
                
                var splinePoints = splineBlobBuilder.Allocate(ref newSplineBlobAsset.equalDistantPoints, pointCount - 1);
                var splinePlatformPositions = splineBlobBuilder.Allocate(ref newSplineBlobAsset.unitPointPlatformPositions, nbPlatforms);
                
                CalculatePoints(fullMarkersData, ref splinePoints, ref splinePlatformPositions, ref newSplineBlobAsset.length);
                newSplineBlobAsset.TrainCount = randomizer.NextInt(trainCountLower, trainCountUpper);
                newSplineBlobAsset.CarriagesPerTrain = randomizer.NextInt(carriagesPerTrainLower, carriagesPerTrainUpper);
            }

            dstManager.AddComponentData(entity, new SplineDataReference
            {
                BlobAssetReference = splineBlobBuilder.CreateBlobAssetReference<SplineBlobAssetArray>(Allocator.Persistent)
            });
        }
    }

    void OnDrawGizmos()
    {
        foreach (Transform child in transform)
        {
            var markersData = CreateActualRailMarkers(child.GetComponentsInChildren<RailMarker>(), out var nbPlatforms);
            for (int i = 0; i < markersData.Length - 1; i++)
            {
                Gizmos.color = i < markersData.Length / 2 ? Color.red : Color.green;
                Gizmos.DrawLine(markersData[i].position, markersData[i + 1].position);
            }
        }
    }

    public struct RailMarkerData
    {
        public RailMarkerType railMarkerType;
        public float3 position;
    }

    public const int pointCount = 1000;
    public const float increment = 1 / ((float) pointCount - 1);

    public void CalculatePoints(in RailMarkerData[] railMarkers, ref BlobBuilderArray<float3> outPoints, ref BlobBuilderArray<float> platformPositions, ref float splineDataDistance)
    {
        var distArray = new NativeArray<float>(pointCount, Allocator.Temp);
        var distMarkerSegment = new NativeArray<float>(railMarkers.Length, Allocator.Temp);

        var pointCountPerMarkerSegment = pointCount / (railMarkers.Length);
        var tIntervalPerMarkerSegment = 1 / ((float) pointCountPerMarkerSegment - 1);
        var totalDistance = 0f;
        float[] platformDistances = new float[platformPositions.Length];
        for (int markerIndex = 0; markerIndex < railMarkers.Length; markerIndex++)
        {
            GetSegmentPoints(railMarkers, markerIndex,
                out var startPoint,
                out var endPoint,
                out var startHandle,
                out var endHandle);

            var lastPoint = startPoint;
            var distanceIndex = 0;

            for (var i = 0; i < pointCountPerMarkerSegment; i++)
            {
                var t = tIntervalPerMarkerSegment * i;

                var point = CubicBezierOfSegment(startPoint, startHandle, endHandle, endPoint, t);

                totalDistance += math.distance(point, lastPoint);
                distanceIndex = i + markerIndex * pointCountPerMarkerSegment;
                distArray[distanceIndex] = totalDistance;

                lastPoint = point;
            }

            distMarkerSegment[markerIndex] = totalDistance;
        }

        splineDataDistance = totalDistance;

        var lastDistancedPoint = railMarkers.First().position;
        int platformIndex = 0;
        int prevMarkerIndex = -1;
        for (var i = 0; i < pointCount - 1; i++)
        {
            var percentage = i * increment;
            var (t, markerIndex) = Sample(distArray, distMarkerSegment, percentage, totalDistance, pointCountPerMarkerSegment);

            GetSegmentPoints(railMarkers, markerIndex,
                out var startPoint,
                out var endPoint,
                out var startHandle,
                out var endHandle);

            outPoints[i] = CubicBezierOfSegment(startPoint, startHandle, endHandle, endPoint, t);

            Debug.DrawLine(lastDistancedPoint, outPoints[i], Color.green);
            lastDistancedPoint = outPoints[i];

            if (prevMarkerIndex != markerIndex
                && railMarkers[markerIndex].railMarkerType == RailMarkerType.PLATFORM_END
                && railMarkers[prevMarkerIndex].railMarkerType == RailMarkerType.PLATFORM_START)
            {
                platformPositions[platformIndex++] = (pointCount - 1) * distMarkerSegment[prevMarkerIndex] / totalDistance;
            }

            prevMarkerIndex = markerIndex;
        }

        distArray.Dispose();
        distMarkerSegment.Dispose();
    }

    public RailMarkerData[] CreateActualRailMarkers(in RailMarker[] srcRailMarkers, out int nbPlatforms)
    {
        var actualRailMarkers = new List<RailMarker>(srcRailMarkers);
        var markersData = actualRailMarkers.Select(m => new RailMarkerData() {railMarkerType = m.railMarkerType, position = m.transform.position}).ToList();
        var nbSrcPoints = srcRailMarkers.Length;
        nbPlatforms = 0;
        for (int i = nbSrcPoints - 1; i >= 0; i--)
        {
            Vector3 offsetDir;
            var srcCurRailMarker = srcRailMarkers[i];
            var srcPrevRailMarker = i < nbSrcPoints - 1 ? srcRailMarkers[i + 1] : null;
            var srcNextRailMarker = i > 0 ? srcRailMarkers[i - 1] : null;

            if (i == 0)
            {
                var railDir = Vector3.Normalize(srcPrevRailMarker.transform.position - srcCurRailMarker.transform.position);
                offsetDir = -Vector3.Cross(Vector3.up, railDir).normalized;
            }
            else if (i == nbSrcPoints - 1)
            {
                var railDir = Vector3.Normalize(srcNextRailMarker.transform.position - srcCurRailMarker.transform.position);
                offsetDir = Vector3.Cross(Vector3.up, railDir).normalized;
            }
            else
            {
                var curPosition = srcCurRailMarker.transform.position;
                var railDirNext = Vector3.Normalize(srcNextRailMarker.transform.position - curPosition);
                var railDirPrev = Vector3.Normalize(srcPrevRailMarker.transform.position - curPosition);
                railDirNext.y = 0;
                railDirPrev.y = 0;
                offsetDir = Vector3.Slerp(railDirPrev.normalized, railDirNext.normalized, 0.5f);
                offsetDir = Vector3.Normalize(offsetDir);
                if (Vector3.SignedAngle(offsetDir, railDirNext, Vector3.up) > 0)
                    offsetDir *= -1.0f;
            }

            var returnPos = srcCurRailMarker.transform.position + offsetDir * returnRailsOffset;

            RailMarkerType newRailMarkerType = RailMarkerType.ROUTE;
            if (srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_START)
                newRailMarkerType = RailMarkerType.PLATFORM_END;
            if (srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_END)
                newRailMarkerType = RailMarkerType.PLATFORM_START;
            markersData.Add(new RailMarkerData {railMarkerType = newRailMarkerType, position = returnPos});

            if (srcPrevRailMarker != null &&
                srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_START &&
                srcPrevRailMarker.railMarkerType == RailMarkerType.PLATFORM_END)
                nbPlatforms += 2;
        }

        return markersData.ToArray();
    }

    static float3 CubicBezierOfSegment(Vector3 startPoint, float3 startHandle, float3 endHandle, float3 endPoint, float t)
    {
        var startToStartHandle = math.lerp(startPoint, startHandle, t);
        var startHandleToEndHandle = math.lerp(startHandle, endHandle, t);
        var endHandleToEnd = math.lerp(endHandle, endPoint, t);

        var startToStartHandleToEndHandle = math.lerp(startToStartHandle, startHandleToEndHandle, t);
        var startHandleToEndHandleToEnd = math.lerp(startHandleToEndHandle, endHandleToEnd, t);

        var point = math.lerp(startToStartHandleToEndHandle, startHandleToEndHandleToEnd, t);
        return point;
    }

    void GetSegmentPoints(RailMarkerData[] railMarkers, int markerIndex, out float3 startPoint, out float3 endPoint, out float3 startHandle, out float3 endHandle)
    {
        startPoint = railMarkers[markerIndex].position;
        var backFromStartPos = railMarkers[markerIndex - 1 < 0 ? railMarkers.Length - 1 : markerIndex - 1].position;
        endPoint = railMarkers[(markerIndex + 1) % railMarkers.Length].position;
        var frontOfEndPoint = railMarkers[(markerIndex + 2) % railMarkers.Length].position;

        var startToEndDir = endPoint - startPoint;
        var backFromStartToStartDir = startPoint - backFromStartPos;
        var frontOfEndPointToEndDir = endPoint - frontOfEndPoint;

        startHandle = startPoint + (startToEndDir.Normalized() + backFromStartToStartDir.Normalized()).Normalized() * startToEndDir.Magnitude() * splineHandleGain;
        endHandle = endPoint + (frontOfEndPointToEndDir.Normalized() - startToEndDir.Normalized()).Normalized() * startToEndDir.Magnitude() * splineHandleGain;
    }

    [BurstCompile]
    public static (float t, int markerIndex) Sample(NativeArray<float> distances, NativeArray<float> distancesMarkerSegment, float percentage, in float totalLength, in int pointCountPerMarkerSegment)
    {
        percentage = math.frac(percentage);
        if (percentage <= 0)
            return (0, 0);
        var smpDist = percentage * totalLength;
        for (var markerIndex = -1; markerIndex < distancesMarkerSegment.Length - 1; markerIndex++)
        {
            var currentDistanceAtMarkerLevel = markerIndex == -1 ? 0 : distancesMarkerSegment[markerIndex];
            if (smpDist >= currentDistanceAtMarkerLevel && smpDist < distancesMarkerSegment[markerIndex + 1])
            {
                var distanceIndex = (markerIndex + 1) * pointCountPerMarkerSegment;
                for (var i = 0; i < pointCountPerMarkerSegment - 1; i++)
                {
                    var lowerDistance = distances[distanceIndex + i];
                    var upperDistance = distances[distanceIndex + i + 1];
                    if (smpDist >= lowerDistance && smpDist < upperDistance)
                    {
                        var ta = i / ((float) pointCountPerMarkerSegment - 1);
                        var tb = (i + 1) / ((float) pointCountPerMarkerSegment - 1);
                        var dRange = upperDistance - lowerDistance;
                        var dVal = smpDist - lowerDistance;
                        var tMid = dVal / dRange;
                        return (math.lerp(ta, tb, tMid), markerIndex + 1);
                    }
                }
            }
        }

        return (0, 0);
    }
}

public static class MathExtensionMethods
{
    public static float3 Normalized(this float3 val) => math.normalize(val);
    public static float3 Magnitude(this float3 val) => math.length(val);
}
