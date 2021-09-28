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

        const int pointCount = 5;
        const float increment = 1 / (float) pointCount;
        
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

        var prevT = 0f;
        lastPoint = startPoint;
        for (int i = 0; i < pointCount; i++)
        {
            var t = distArray.Sample(prevT)/totalDistance;
            
            var startToEndHandle = math.lerp(startPoint, endHandle, t);
            var endHandleToEnd = math.lerp(endHandle, endPoint, t);
            var point = math.lerp(startToEndHandle, endHandleToEnd, t);
            
            Debug.DrawLine(lastPoint, point, Color.magenta);
            Debug.DrawLine(point, point+math.up(), Color.red);
            lastPoint = point;
            prevT = t;
        }
        
        Debug.Log(totalDistance);

        distArray.Dispose();
        
        for (int i = 1; i < railMarkers.Length; i++)
        {
            
        }
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

public static class NativeFloatArrayExtensions
{
    [BurstCompile]
    public static float Sample(this NativeArray<float> points, float t)
    {
        var iNormalized = t * (points.Length - 1);
        var iLowerBound = (int) math.floor(iNormalized);
        var iUpperBound = iLowerBound + 1;

        return math.lerp(points[iLowerBound], points[iUpperBound], math.frac(iNormalized));
    }
}
