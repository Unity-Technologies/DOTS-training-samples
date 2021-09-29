using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
public class SplineDataAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity
{
    public float returnRailsOffset = 10.0f;

    void Start()
    {
        foreach (Transform child in transform)
        {
            var railMarkers = child.GetComponentsInChildren<RailMarker>();
            CreateActualRailMarkers(railMarkers, true);
        }
    }

    void Update()
    {
        foreach (Transform child in transform)
        {
            var railMarkers = child.GetComponentsInChildren<RailMarker>();
            var markersData = CreateActualRailMarkers(railMarkers, false);
            for (int i = 0; i < markersData.Length - 1; i++)
            {
                Debug.DrawLine(markersData[i].position, markersData[i+1].position, i < markersData.Length/2 ? Color.red : Color.green);
            }
        }
    }

    public struct RailMarkerData
    {
        public RailMarkerType railMarkerType;
        public Vector3 position;
    }
    
    public const int pointCount = 1000;
    public const float increment = 1 / ((float) pointCount-1);
    public void CalculatePoints(in RailMarkerData[] railMarkers, ref BlobBuilderArray<float3> outPoints, ref BlobBuilderArray<float> platformPositions, ref float splineDataDistance)
    {
        var distArray = new NativeArray<float>(pointCount, Allocator.Temp);
        var distMarkerSegment = new NativeArray<float>(railMarkers.Length, Allocator.Temp);
        
        var pointCountPerMarkerSegment = pointCount/(railMarkers.Length);
        var tIntervalPerMarkerSegment = 1 / ((float) pointCountPerMarkerSegment-1);
        var totalDistance = 0f;
        for (int markerIndex = 0; markerIndex < railMarkers.Length; markerIndex++)
        {
            GetSegmentPoints(railMarkers, markerIndex, 
                out var startPoint, 
                out var endPoint, 
                out var startHandle, 
                out var endHandle);
            
            var lastPoint = startPoint;
            for (var i = 0; i < pointCountPerMarkerSegment; i++)
            {
                var t = tIntervalPerMarkerSegment * i;
                
                var point = CubicBezierOfSegment(startPoint, startHandle, endHandle, endPoint, t);
                
                totalDistance += math.distance(point, lastPoint);
                var distanceIndex = i + markerIndex * pointCountPerMarkerSegment;
                distArray[distanceIndex] = totalDistance;
                
                lastPoint = point;
            }

            distMarkerSegment[markerIndex] = totalDistance;
        }

        splineDataDistance = totalDistance;
        
        var lastDistancedPoint = railMarkers.First().position;
        int platformIndex = 0;
        for (var i = 0; i < pointCount; i++)
        {
            var percentage = i*increment;
            var (t, markerIndex) = Sample(distArray, distMarkerSegment, percentage, totalDistance, pointCountPerMarkerSegment);

            GetSegmentPoints(railMarkers, markerIndex, 
                out var startPoint, 
                out var endPoint, 
                out var startHandle, 
                out var endHandle);
            
            outPoints[i] = CubicBezierOfSegment(startPoint, startHandle, endHandle, endPoint, t);
            
            Debug.DrawLine(lastDistancedPoint, outPoints[i], Color.green);
            lastDistancedPoint = outPoints[i];
            
            if (railMarkers[markerIndex].railMarkerType == RailMarkerType.PLATFORM_END)
            {
                platformPositions[platformIndex++] = t;
            }
        }

        distArray.Dispose();
        distMarkerSegment.Dispose();
    }
    
    public RailMarkerData[] CreateActualRailMarkers(in RailMarker[] srcRailMarkers, bool createInstances)
    {
        var actualRailMarkers = new List<RailMarker>(srcRailMarkers);
        var markersData = actualRailMarkers.Select(m => new RailMarkerData(){railMarkerType = m.railMarkerType, position = m.transform.position}).ToList();
        var nbSrcPoints = srcRailMarkers.Length;
        var parent = srcRailMarkers[0].transform.parent;
        int curPointIndex = nbSrcPoints;
        for (int i = nbSrcPoints - 1; i >= 0; i--)
        {
            Vector3 offsetDir;
            var srcCurRailMarker = srcRailMarkers[i];
            var srcPrevRailMarker =  i < nbSrcPoints -1 ? srcRailMarkers[i + 1] : null;
            var srcNextRailMarker = i > 0 ? srcRailMarkers[i - 1] : null;
            
            if (i == 0)
            {
                var railDir = Vector3.Normalize(srcPrevRailMarker.transform.position - srcCurRailMarker.transform.position);
                offsetDir =  Vector3.Cross(Vector3.up, railDir).normalized;
            }
            else if (i  == nbSrcPoints - 1)
            {
                var railDir = Vector3.Normalize(srcNextRailMarker.transform.position - srcCurRailMarker.transform.position);
                offsetDir =  - Vector3.Cross(Vector3.up, railDir).normalized;
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
                if (Vector3.SignedAngle(offsetDir, railDirNext, Vector3.up) < 0)
                    offsetDir *= -1.0f;
            }
            
            //if (createInstances)
            //{
                //RailMarker newRailMarker = Instantiate(srcCurRailMarker, parent);
                var returnPos = srcCurRailMarker.transform.position + offsetDir * returnRailsOffset;
                //newRailMarker.transform.position = returnPos;
                //newRailMarker.pointIndex = curPointIndex++;
                RailMarkerType newRailMarkerType = RailMarkerType.ROUTE;
                if (srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_START)
                    newRailMarkerType = RailMarkerType.PLATFORM_END;
                if (srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_END)
                    newRailMarkerType = RailMarkerType.PLATFORM_START;
                //actualRailMarkers.Add((newRailMarker));
                markersData.Add(new RailMarkerData(){railMarkerType = newRailMarkerType, position = returnPos});
            //}
        }
        return markersData.ToArray();
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        using (var splineBlobBuilder = new BlobBuilder(Allocator.Temp))
        {
            ref var splineDataArray = ref splineBlobBuilder.ConstructRoot<SplineBlobAssetArray>();

            var splineArray = splineBlobBuilder.Allocate(ref splineDataArray.splineBlobAssets, transform.childCount);
            int lineId = 0;
            foreach (Transform child in transform)
            {
                var railMarkers = child.GetComponentsInChildren<RailMarker>();
                ref var newSplineBlobAsset = ref splineArray[lineId++];
                var splinePoints = splineBlobBuilder.Allocate(ref newSplineBlobAsset.points, pointCount);
                int nbPlatforms = railMarkers.Count(r => r.railMarkerType == RailMarkerType.PLATFORM_END) * 2;
                var splinePlatformPositions = splineBlobBuilder.Allocate(ref newSplineBlobAsset.platformPositions, nbPlatforms);
                var fullMarkersData = CreateActualRailMarkers(railMarkers, true);
                CalculatePoints(fullMarkersData, ref splinePoints, ref splinePlatformPositions, ref newSplineBlobAsset.length);
            }

            BlobAssetReference<SplineBlobAssetArray> blobAssetReference =
                splineBlobBuilder.CreateBlobAssetReference<SplineBlobAssetArray>(Allocator.Persistent);
            dstManager.AddComponentData(entity, new SplineDataReference
            {
                BlobAssetReference = blobAssetReference
            });
        }
    }

    static float3 CubicBezierOfSegment(Vector3 startPoint, Vector3 startHandle, Vector3 endHandle, Vector3 endPoint, float t)
    {
        var startToStartHandle = math.lerp(startPoint, startHandle, t);
        var startHandleToEndHandle = math.lerp(startHandle, endHandle, t);
        var endHandleToEnd = math.lerp(endHandle, endPoint, t);

        var startToStartHandleToEndHandle = math.lerp(startToStartHandle, startHandleToEndHandle, t);
        var startHandleToEndHandleToEnd = math.lerp(startHandleToEndHandle, endHandleToEnd, t);

        var point = math.lerp(startToStartHandleToEndHandle, startHandleToEndHandleToEnd, t);
        return point;
    }

    static void GetSegmentPoints(RailMarkerData[] railMarkers, int markerIndex, out Vector3 startPoint, out Vector3 endPoint, out Vector3 startHandle, out Vector3 endHandle)
    {
        startPoint = railMarkers[markerIndex].position;
        var backFromStartPos = railMarkers[markerIndex - 1 < 0 ? railMarkers.Length - 1 : markerIndex - 1].position;
        endPoint = railMarkers[(markerIndex + 1) % railMarkers.Length].position;
        var frontOfEndPoint = railMarkers[(markerIndex + 2) % railMarkers.Length].position;

        var startToEndDir = endPoint - startPoint;
        var backFromStartToStartDir = startPoint - backFromStartPos;
        var frontOfEndPointToEndDir = endPoint - frontOfEndPoint;
        
        startHandle = startPoint+(startToEndDir.normalized+backFromStartToStartDir.normalized).normalized*startToEndDir.magnitude*.5f;
        endHandle = endPoint+(frontOfEndPointToEndDir.normalized - startToEndDir.normalized).normalized*startToEndDir.magnitude*.5f;
    }
    
    [BurstCompile]
    public static (float t, int markerIndex) Sample(NativeArray<float> distances, NativeArray<float> distancesMarkerSegment, float percentage, in float totalLength, in int pointCountPerMarkerSegment)
    {
        percentage = math.frac(percentage);
        if(percentage <= 0)
            return (0,0);
        var smpDist = percentage * totalLength;
        for(var markerIndex = -1; markerIndex < distancesMarkerSegment.Length-1; markerIndex++ )
        {
            var currentDistanceAtMarkerLevel = markerIndex == -1 ? 0 : distancesMarkerSegment[markerIndex];
            if( smpDist >= currentDistanceAtMarkerLevel && smpDist < distancesMarkerSegment[markerIndex + 1] ) {
                var distanceIndex = (markerIndex+1) * pointCountPerMarkerSegment;
                for (var i = 0; i < pointCountPerMarkerSegment-1; i++)
                {
                    var lowerDistance = distances[distanceIndex+i];
                    var upperDistance = distances[distanceIndex+i+1];
                    if (smpDist >= lowerDistance && smpDist < upperDistance)
                    {
                        var ta = i / ((float) pointCountPerMarkerSegment - 1 );
                        var tb = ( i + 1 ) / ((float) pointCountPerMarkerSegment - 1 );
                        var dRange = upperDistance - lowerDistance;
                        var dVal = smpDist - lowerDistance;
                        var tMid = dVal / dRange;
                        return (math.lerp( ta, tb, tMid), markerIndex+1);
                    }
                }
            }
        }

        return (0,0);
    }
}
