using System;
using System.Collections.Generic;
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
