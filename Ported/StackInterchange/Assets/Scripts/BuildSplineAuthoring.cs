using SWS;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public struct BuildSpline : IComponentData
{
    public BlobAssetReference<SegmentHandle> SegmentCollection;
    public BlobAssetReference<SplineHandle> AllSplines;
    public BlobAssetReference<SplineHandle> SplineCollection; //Use the same view as spline but represent count
}

public class BuildSplineAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject m_Reference;
    public float m_MergeRadius = 1.0f;

    private static int GetOrAddPointIndex(float3 value, List<float3> list, float radius)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            var sqrLength = math.lengthsq(list[i] - value);
            if (sqrLength < radius)
            {
                list[i] = 0.5f * (list[i] + value);
                return i;
            }
        }

        list.Add(value);
        return list.Count - 1;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var waypointListCache = new List<float3>();
        var segmentList = new List<(int, int)>();
        var allSpline = new List<int>();
        var splineCollection = new List<int>();

        //Using Prefab of Spline
        var allTransformList = m_Reference.GetComponentsInChildren<PathManager>().Select(o =>
        {
            return o.waypoints.Select(pathManager => pathManager.position).ToArray();
        });

        //Using Custom compute
        allTransformList = allTransformList.Concat(m_Reference.GetComponentsInChildren<PathResult>().Select(o =>
        {
            return o.m_Path.Select(path => path.position).ToArray();
        }));

        var sqrRadius = m_MergeRadius * m_MergeRadius;
        foreach (var path in allTransformList)
        {
            if (path.Length < 2)
                continue;

            var currentSpline = new List<int>();

            for (int i = 0; i < path.Length - 1; ++i)
            {
                var currentSegment = (-1, -1);
                currentSegment.Item1 = GetOrAddPointIndex(path[i+0], waypointListCache, sqrRadius);
                currentSegment.Item2 = GetOrAddPointIndex(path[i+1], waypointListCache, sqrRadius);

                int segmentIndex = segmentList.FindIndex(s => s.Item1 == currentSegment.Item1 && s.Item2 == currentSegment.Item2);
                if (segmentIndex == -1)
                {
                    segmentList.Add(currentSegment);
                    segmentIndex = segmentList.Count - 1;
                }
                currentSpline.Add(segmentIndex);
            }

            allSpline.AddRange(currentSpline);
            splineCollection.Add(currentSpline.Count());
        }

        //Building the segment collection
        var buildSplineData = new BuildSpline();

        //Segment Collection
        using (var builder = new BlobBuilder(Allocator.Temp))
        {
            var arrayOfSegment = segmentList.Select(o => new SegmentData()
            {
                Start = waypointListCache[o.Item1],
                End = waypointListCache[o.Item2]
            }).ToArray();


            ref var root = ref builder.ConstructRoot<SegmentHandle>();
            var segmentArray = builder.Allocate(ref root.Segments, arrayOfSegment.Length);

            for (int i = 0; i < segmentArray.Length; ++i)
                segmentArray[i] = arrayOfSegment[i];

            var segmentCollection = dstManager.CreateEntity();
            buildSplineData.SegmentCollection = builder.CreateBlobAssetReference<SegmentHandle>(Allocator.Persistent);
        }

        //All Spline
        using (var builder = new BlobBuilder(Allocator.Temp))
        {
            ref var root = ref builder.ConstructRoot<SplineHandle>();
            var splineHandle = builder.Allocate(ref root.Segments, allSpline.Count);

            for (int i = 0; i < splineHandle.Length; ++i)
                splineHandle[i] = allSpline[i];

            var segmentCollection = dstManager.CreateEntity();
            buildSplineData.AllSplines = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent);
        }

        //Spline Collection
        using (var builder = new BlobBuilder(Allocator.Temp))
        {
            ref var root = ref builder.ConstructRoot<SplineHandle>();
            var splineHandle = builder.Allocate(ref root.Segments, splineCollection.Count);

            for (int i = 0; i < splineHandle.Length; ++i)
                splineHandle[i] = splineCollection[i];

            var segmentCollection = dstManager.CreateEntity();
            buildSplineData.SplineCollection = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent);
        }

        dstManager.AddComponentData(entity, buildSplineData);

    }
}
