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
    public float returnRailsOffset = 50.0f;
    private void Start()
    {
        foreach (Transform child in transform)
        {
            var railMarkers = child.GetComponentsInChildren<RailMarker>();
            CreateActualRailMarkersWithAngles(railMarkers, true);
        }
    }

    void Update()
    {
        BlobBuilder splineBlobBuilder = new BlobBuilder(Allocator.Temp);
        
        foreach (Transform child in transform)
        {
            var railMarkers = child.GetComponentsInChildren<RailMarker>();
            ref var newSplineBlobAsset =  ref splineBlobBuilder.ConstructRoot<BlobArray<float3>>();
            var splinePoints = splineBlobBuilder.Allocate(ref newSplineBlobAsset, railMarkers.Length + 1);
            CalculatePoints(railMarkers, ref splinePoints);

            var markersPos = CreateActualRailMarkersWithAngles(railMarkers, false);
            for (int i = 0; i < markersPos.Length - 1; i++)
            {
                Debug.DrawLine(markersPos[i], markersPos[i+1], i < markersPos.Length/2 ? Color.red : Color.green);
            }
            break;
        }
        
    }

    public struct DistanceAndDistanceIndex
    {
        public float Distance;
        public int DistanceIndex;
    }
    
    public const int pointCount = 1000;
    public const float increment = 1 / (float) pointCount;
    public void CalculatePoints(in RailMarker[] railMarkers, ref BlobBuilderArray<float3> outPoints)
    {
        var distArray = new NativeArray<float>(pointCount, Allocator.Temp);
        var distMarkerSegment = new NativeArray<DistanceAndDistanceIndex>(railMarkers.Length, Allocator.Temp);
        
        var pointCountPerMarkerSegment = pointCount/(railMarkers.Length);
        var tIntervalPerMarkerSegment = 1 / (float) pointCountPerMarkerSegment;
        var totalDistance = 0f;
        for (int markerIndex = 0; markerIndex < railMarkers.Length; markerIndex++)
        {
            var startPoint = railMarkers[markerIndex].transform.position;
            var backFromStartPos = railMarkers[markerIndex - 1 < 0 ? railMarkers.Length-1 : markerIndex-1].transform.position;
            var endPoint = railMarkers[(markerIndex+1) % railMarkers.Length].transform.position;
            var frontOfEndPoint = railMarkers[(markerIndex + 2) % railMarkers.Length].transform.position;
            
            var startHandle = startPoint+(startPoint-backFromStartPos).normalized*5;
            var endHandle = endPoint+(endPoint-frontOfEndPoint).normalized*5;

            // Debug.DrawLine(startPoint, startPoint+Vector3.up, Color.green);
            // Debug.DrawLine(startHandle, startHandle+Vector3.up);
            // Debug.DrawLine(endHandle, endHandle+Vector3.up);
            // Debug.DrawLine(endPoint, endPoint+Vector3.up, Color.green);
            //
            // Debug.DrawLine(startPoint, startHandle, Color.red);
            // Debug.DrawLine(startHandle, endHandle, Color.blue);
            // Debug.DrawLine(endHandle, endPoint, Color.cyan);
            
            var lastPoint = startPoint;
            var distanceIndex = 0;
            for (var i = 0; i < pointCountPerMarkerSegment; i++)
            {
                var t = tIntervalPerMarkerSegment * (i+1);
                
                var startToStartHandle = math.lerp(startPoint, startHandle, t);
                var startHandleToEndHandle = math.lerp(startHandle, endHandle, t);
                var endHandleToEnd = math.lerp(endHandle, endPoint, t);

                var startToStartHandleToEndHandle = math.lerp(startToStartHandle, startHandleToEndHandle, t);
                var startHandleToEndHandleToEnd = math.lerp(startHandleToEndHandle, endHandleToEnd, t);
                
                var point = math.lerp(startToStartHandleToEndHandle, startHandleToEndHandleToEnd, t);
                
                Debug.DrawLine(lastPoint, point, Color.yellow);
 
                totalDistance += math.distance(point, lastPoint);
                distanceIndex = i + markerIndex * pointCountPerMarkerSegment;
                distArray[distanceIndex] = totalDistance;
                
                lastPoint = point;
            }

            distMarkerSegment[markerIndex] = new DistanceAndDistanceIndex{Distance = totalDistance, DistanceIndex = distanceIndex};
        }
        
        var lastDistancedPoint = Vector3.zero;
        
        for (float percentage = 0; percentage <= 1; percentage += increment)
        {
            var (t, markerIndex) = NativeFloatArrayExtensions.Sample(distArray, distMarkerSegment, percentage, totalDistance, pointCountPerMarkerSegment);
           
            var startPoint = railMarkers[markerIndex].transform.position;
            var backFromStartPos = railMarkers[markerIndex - 1 < 0 ? railMarkers.Length-1 : markerIndex-1].transform.position;
            var endPoint = railMarkers[(markerIndex+1) % railMarkers.Length].transform.position;
            var frontOfEndPoint = railMarkers[(markerIndex + 2) % railMarkers.Length].transform.position;
            
            var startHandle = startPoint+(startPoint-backFromStartPos).normalized*5;
            var endHandle = endPoint+(endPoint-frontOfEndPoint).normalized*5;

            var startToStartHandle = math.lerp(startPoint, startHandle, t); 
            var startHandleToEndHandle = math.lerp(startHandle, endHandle, t);
            var endHandleToEnd = math.lerp(endHandle, endPoint, t);

            var startToStartHandleToEndHandle = math.lerp(startToStartHandle, startHandleToEndHandle, t);
            var startHandleToEndHandleToEnd = math.lerp(startHandleToEndHandle, endHandleToEnd, t);
                
            var point = math.lerp(startToStartHandleToEndHandle, startHandleToEndHandleToEnd, t);
                
            Debug.DrawLine(lastDistancedPoint, point, Color.magenta);
            Debug.DrawLine(point, point+math.up(), Color.blue);
            
            lastDistancedPoint = point;
        }

        distArray.Dispose();
        distMarkerSegment.Dispose();
    }

    public RailMarker[] CreateActualRailMarkers(in RailMarker[] srcRailMarkers)
    {
        var actualRailMarkers = new List<RailMarker>(srcRailMarkers);
        var nbSrcPoints = srcRailMarkers.Length;
        var parent = srcRailMarkers[0].transform.parent;
        int curPointIndex = nbSrcPoints;
        for (int i = nbSrcPoints - 1; i >= 0; i--)
        {
            var srcCurRailMarker = srcRailMarkers[i];
            var srcNextRailMarker = i > 0 ? srcRailMarkers[i - 1] : srcRailMarkers[i + 1];
            float flipDir = i > 0 ? 1.0f : -1.0f;
            var railDir = flipDir * Vector3.Normalize(srcNextRailMarker.transform.position - srcCurRailMarker.transform.position);
            var orthoDir = Vector3.Cross(Vector3.up, railDir).normalized;
            RailMarker newRailMarker = Instantiate(srcCurRailMarker,parent);
            newRailMarker.transform.position = srcCurRailMarker.transform.position + orthoDir * returnRailsOffset;
            newRailMarker.pointIndex = curPointIndex++;
            if (srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_START)
                newRailMarker.railMarkerType = RailMarkerType.PLATFORM_END;
            if (srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_END)
                newRailMarker.railMarkerType = RailMarkerType.PLATFORM_START;
            actualRailMarkers.Add((newRailMarker));
        }
        return actualRailMarkers.ToArray();
    }
    
    public Vector3[] CreateActualRailMarkersWithAngles(in RailMarker[] srcRailMarkers, bool createInstances)
    {
        var actualRailMarkers = new List<RailMarker>(srcRailMarkers);
        var markersPos = actualRailMarkers.Select(m => m.transform.position).ToList();
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
                var railDirNext = Vector3.Normalize(srcNextRailMarker.transform.position - srcCurRailMarker.transform.position);
                var railDirPrev = Vector3.Normalize(srcPrevRailMarker.transform.position - srcCurRailMarker.transform.position);
                railDirNext.y = 0;
                railDirPrev.y = 0;
                offsetDir = Vector3.Slerp(railDirPrev.normalized, railDirNext.normalized, 0.5f);
                offsetDir = Vector3.Normalize(offsetDir);
                if (Vector3.SignedAngle(offsetDir, railDirNext, Vector3.up) < 0)
                    offsetDir *= -1.0f;
            }

            if (createInstances)
            {
                RailMarker newRailMarker = Instantiate(srcCurRailMarker, parent);
                var returnPos = srcCurRailMarker.transform.position + offsetDir * returnRailsOffset;
                newRailMarker.transform.position = returnPos;
                newRailMarker.pointIndex = curPointIndex++;
                if (srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_START)
                    newRailMarker.railMarkerType = RailMarkerType.PLATFORM_END;
                if (srcCurRailMarker.railMarkerType == RailMarkerType.PLATFORM_END)
                    newRailMarker.railMarkerType = RailMarkerType.PLATFORM_START;
                actualRailMarkers.Add((newRailMarker));
                markersPos.Add(returnPos);
            }
        }
        return markersPos.ToArray();
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
                var locations = railMarkers.Select(r => r.transform.position).ToList();
                ref var newSplineBlobAsset = ref splineArray[lineId++];
                var splinePoints = splineBlobBuilder.Allocate(ref newSplineBlobAsset.points, locations.Count() + 1);
                var splinePlatformPositions = splineBlobBuilder.Allocate(ref newSplineBlobAsset.platformPositions, 1);

                var nbPoints = locations.Count();
                var totalLength = 0.0f;
                for (int i = 0; i < nbPoints; i++) 
                {
                    splinePoints[i] = locations[i];
                    if (i < nbPoints - 1)
                        totalLength += Vector3.Magnitude(locations[i + 1] - locations[i]);
                }

                splinePoints[nbPoints] = locations[0];
                splinePlatformPositions[0] = 0.5f;
                newSplineBlobAsset.length = totalLength;
            }

            BlobAssetReference<SplineBlobAssetArray> blobAssetReference =
                splineBlobBuilder.CreateBlobAssetReference<SplineBlobAssetArray>(Allocator.Persistent);
            dstManager.AddComponentData(entity, new SplineDataReference
            {
                BlobAssetReference = blobAssetReference
            });
        }
    }
}

public static class NativeFloatArrayExtensions
{
    [BurstCompile]
    public static (float t, int markerIndex) Sample(NativeArray<float> distances, NativeArray<SplineDataAuthoring.DistanceAndDistanceIndex> distancesMarkerSegment, float percentage, in float totalLength, in int pointCountPerMarkerSegment)
    {
        if( percentage <= 0 )
            return (0,0);
        if( percentage >= 1 )
            return (1, distancesMarkerSegment.Length-1);
        var smpDist = percentage * totalLength;
        for(var markerIndex = 0; markerIndex < distancesMarkerSegment.Length-1; markerIndex++ )
        {
            var currentDistanceAtMarkerLevel = distancesMarkerSegment[markerIndex];
            if( smpDist >= currentDistanceAtMarkerLevel.Distance && smpDist < distancesMarkerSegment[markerIndex + 1].Distance ) {
                for (int i = 0; i < pointCountPerMarkerSegment; i++)
                {
                    var lowerDistance = distances[currentDistanceAtMarkerLevel.DistanceIndex+i];
                    var upperDistance = distances[currentDistanceAtMarkerLevel.DistanceIndex+i+1];
                    if (smpDist >= lowerDistance && smpDist < upperDistance)
                    {
                        var ta = i / ((float) pointCountPerMarkerSegment - 1 );
                        var tb = ( i + 1 ) / ((float) pointCountPerMarkerSegment - 1 );
                        var dRange = upperDistance - lowerDistance;
                        var dVal = smpDist - lowerDistance;
                        var tMid = dVal / dRange;
                        return (math.lerp( ta, tb, tMid), markerIndex);
                    }
                }
            }
        }

        return (0,0);
    }
}
