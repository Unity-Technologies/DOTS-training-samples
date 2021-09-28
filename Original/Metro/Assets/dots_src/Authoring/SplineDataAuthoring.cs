using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
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
    
    public const int pointCount = 100;
    public const float increment = 1 / (float) pointCount;
    public void CalculatePoints(in RailMarker[] railMarkers, ref BlobBuilderArray<float3> outPoints)
    {
        var startPoint = railMarkers[0].transform.position;
        var endHandle = railMarkers[1].transform.position+(railMarkers[1].transform.position-railMarkers[2].transform.position);
        var endPoint = railMarkers[1].transform.position;
        
        Debug.DrawLine(startPoint, startPoint+Vector3.up, Color.green);
        Debug.DrawLine(endHandle, endHandle+Vector3.up);
        Debug.DrawLine(endPoint, endPoint+Vector3.up, Color.green);

        Debug.DrawLine(startPoint, endHandle, Color.black);
        Debug.DrawLine(endHandle, endPoint, Color.black);

        var lastPoint = startPoint;
        var distArray = new NativeArray<float>(pointCount, Allocator.Persistent);
        var totalDistance = 0f;
        for (int i = 0; i < pointCount; i++)
        {
            var t = increment * (i+1);
            var startToEndHandle = math.lerp(startPoint, endHandle, t);
            var endHandleToEnd = math.lerp(endHandle, endPoint, t);
            var point = math.lerp(startToEndHandle, endHandleToEnd, t);
            Debug.DrawLine(lastPoint, point, Color.yellow);
            totalDistance += math.distance(point, lastPoint);

            distArray[i] = totalDistance;
            lastPoint = point;
        }

        lastPoint = startPoint;
        for (float percentage = 0; percentage < 1; percentage += increment)
        {
            var t = distArray.Sample(percentage, totalDistance);
            
            var startToEndHandle = math.lerp(startPoint, endHandle, t);
            var endHandleToEnd = math.lerp(endHandle, endPoint, t);
            var point = math.lerp(startToEndHandle, endHandleToEnd, t);
            
            Debug.DrawLine(lastPoint, point, Color.magenta);
            Debug.DrawLine(point, point+math.up(), Color.red);
            lastPoint = point;
        }

        distArray.Dispose();
        
        for (int i = 1; i < railMarkers.Length; i++)
        {
            
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
            
            break;
        }
        
        
        var blobAssetReference = splineBlobBuilder.CreateBlobAssetReference<SplineBlobAsset>(Allocator.Persistent);
        dstManager.AddComponentData(entity, new SplineDataReference
        {
            BlobAssetReference = blobAssetReference
        });
    }
}

public static class NativeFloatArrayExtensions
{
    [BurstCompile]
    public static float Sample(this NativeArray<float> distance, float t, in float totalLength)
    {
        if( t <= 0 )
            return 0;
        if( t >= 1 )
            return 1;
        var smpDist = t * totalLength;
        for( var i = 0; i < SplineDataAuthoring.pointCount - 1; i++ ) {
            if( smpDist >= distance[i] && smpDist < distance[i + 1] ) {
                var ta = i / ((float) SplineDataAuthoring.pointCount - 1 );
                var tb = ( i + 1 ) / ((float) SplineDataAuthoring.pointCount - 1 );
                var dRange = distance[i + 1] - distance[i];
                var dVal = smpDist - distance[i];
                var tMid = dVal / dRange;
                return math.lerp( ta, tb, tMid );
            }
        }

        return 0;
    }
}
