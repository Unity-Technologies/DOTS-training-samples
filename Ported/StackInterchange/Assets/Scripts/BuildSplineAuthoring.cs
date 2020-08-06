using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public struct BuildSpline : IComponentData
{
}

public class BuildSplineAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject m_Reference;
    public float m_MergeRadius = 1.0f;
    public uint m_GenerateCopyCount = 1;

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

        var allTransformList = m_Reference.GetComponentsInChildren<PathResult>().Select(o =>
        {
            return o.m_Path.Select(path => path.position).ToArray();
        }).ToArray();

        var sqrRadius = m_MergeRadius * m_MergeRadius;

        uint squareCount = (uint)Math.Round(Mathf.Sqrt(m_GenerateCopyCount));
        if (squareCount == 0)
            throw new InvalidOperationException("Convert fails");

        for (uint currentCopy = 0; currentCopy < m_GenerateCopyCount; ++currentCopy)
        {
            int x = (int)(currentCopy % squareCount);
            int y = (int)(currentCopy / squareCount);
            x -= (int)squareCount / 2;
            y -= (int)squareCount / 2;

            var scale = 300.0f;
            var offset = new Vector3(x, 0, y) * scale;

            foreach (var path in allTransformList)
            {
                if (path.Length < 2)
                    continue;

                var currentSpline = new List<int>();
                for (int i = 0; i < path.Length - 1; ++i)
                {
                    var currentSegment = (-1, -1);
                    var start = path[i + 0] + offset;
                    var end = path[i + 1] + offset;
                    if ((start - end).sqrMagnitude < 0.01f)
                        continue;

                    currentSegment.Item1 = GetOrAddPointIndex(path[i + 0] + offset, waypointListCache, sqrRadius);
                    currentSegment.Item2 = GetOrAddPointIndex(path[i + 1] + offset, waypointListCache, sqrRadius);

                    int segmentIndex = segmentList.FindIndex(s => s.Item1 == currentSegment.Item1 && s.Item2 == currentSegment.Item2);
                    if (segmentIndex == -1)
                    {
                        segmentList.Add(currentSegment);
                        segmentIndex = segmentList.Count - 1;
                    }
                    currentSpline.Add(segmentIndex);
                }

                //Compute category based on destination
                var value = path.Last();
                var directionFromCenter = value.normalized;

                var directionArray = new[]
                {
                    Vector3.forward,
                    Vector3.back,
                    Vector3.right,
                    Vector3.left
                };

                var max = float.MinValue;
                var direction = -1;

                for (int i = 0; i < directionArray.Length; ++i)
                {
                    var dot = Vector3.Dot(directionFromCenter, directionArray[i]);
                    if (dot > max)
                    {
                        max = dot;
                        direction = i;
                    }
                }

                var splineEntity = conversionSystem.CreateAdditionalEntity(this);

                var splineData = new Spline();
                using (var builder = new BlobBuilder(Allocator.Temp))
                {
                    ref var root = ref builder.ConstructRoot<SplineHandle>();
                    var splineHandle = builder.Allocate(ref root.Segments, currentSpline.Count);

                    for (int i = 0; i < splineHandle.Length; ++i)
                        splineHandle[i] = currentSpline[i];

                    splineData.Value = builder.CreateBlobAssetReference<SplineHandle>(Allocator.Persistent);
                }

                var splineCategory = new SplineCategory()
                {
                    category = direction
                };

                dstManager.AddComponentData(splineEntity, splineData);
                dstManager.AddComponentData(splineEntity, splineCategory);
            }
        }

        //Building the segment collection
        var segmentCollectionData = new SegmentCollection();
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

            segmentCollectionData.Value = builder.CreateBlobAssetReference<SegmentHandle>(Allocator.Persistent);
        }
        var segmentCollectionEntity = conversionSystem.CreateAdditionalEntity(this);
        dstManager.AddComponentData(segmentCollectionEntity, segmentCollectionData);

        //Keep a reference on created data maybe ? is it needed ?
        dstManager.AddComponentData(entity, new BuildSpline());

    }
}
