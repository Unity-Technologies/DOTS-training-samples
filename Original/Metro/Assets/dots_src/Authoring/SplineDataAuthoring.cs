using System;
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
    void Update()
    {
        BlobBuilder splineBlobBuilder = new BlobBuilder(Allocator.Temp);
        
        foreach (Transform child in transform)
        {
            var railMarkers = child.GetComponentsInChildren<RailMarker>();
            ref var newSplineBlobAsset =  ref splineBlobBuilder.ConstructRoot<BlobArray<float3>>();
            var splinePoints = splineBlobBuilder.Allocate(ref newSplineBlobAsset, railMarkers.Length + 1);
            CalculatePoints(railMarkers, ref splinePoints);
        }
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        BlobBuilder splineBlobBuilder = new BlobBuilder(Allocator.Temp);

        foreach (Transform child in transform)
        {
            var railMarkers = child.GetComponentsInChildren<RailMarker>();
            ref var newSplineBlobAsset =  ref splineBlobBuilder.ConstructRoot<SplineBlobAsset>();
            var splinePoints = splineBlobBuilder.Allocate(ref newSplineBlobAsset.points, railMarkers.Length + 1);
            var splinePlatformPositions = splineBlobBuilder.Allocate(ref newSplineBlobAsset.platformPositions, 1);
            CalculatePoints(railMarkers, ref splinePoints);
        }
        
        
        var blobAssetReference = splineBlobBuilder.CreateBlobAssetReference<SplineBlobAsset>(Allocator.Persistent);
        dstManager.AddComponentData(entity, new SplineDataReference
        {
            BlobAssetReference = blobAssetReference
        });
    }
    
    public const int pointCount = 1000;
    public const float increment = 1 / (float) pointCount;
    public void CalculatePoints(in RailMarker[] railMarkers, ref BlobBuilderArray<float3> outPoints)
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
        
        var lastDistancedPoint = railMarkers.First().transform.position;
        
        for (float percentage = 0; percentage <= 1; percentage += increment)
        {
            var (t, markerIndex) = Sample(distArray, distMarkerSegment, percentage, totalDistance, pointCountPerMarkerSegment);
           
            GetSegmentPoints(railMarkers, markerIndex, 
                out var startPoint, 
                out var endPoint, 
                out var startHandle, 
                out var endHandle);

            var point = CubicBezierOfSegment(startPoint, startHandle, endHandle, endPoint, t);
            
            Debug.DrawLine(lastDistancedPoint, point, Color.HSVToRGB(railMarkers[markerIndex].metroLineID/10f,1,1));
            
            lastDistancedPoint = point;
        }

        distArray.Dispose();
        distMarkerSegment.Dispose();
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

    static void GetSegmentPoints(RailMarker[] railMarkers, int markerIndex, out Vector3 startPoint, out Vector3 endPoint, out Vector3 startHandle, out Vector3 endHandle)
    {
        startPoint = railMarkers[markerIndex].transform.position;
        var backFromStartPos = railMarkers[markerIndex - 1 < 0 ? railMarkers.Length - 1 : markerIndex - 1].transform.position;
        endPoint = railMarkers[(markerIndex + 1) % railMarkers.Length].transform.position;
        var frontOfEndPoint = railMarkers[(markerIndex + 2) % railMarkers.Length].transform.position;

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
