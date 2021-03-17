using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class FindEmptyBucketSystem : SystemBase
{
    private EntityQuery bucketQuery;
    
    protected override void OnCreate()
    {
        bucketQuery = GetEntityQuery(
            typeof(Bucket),
            typeof(Volume),
            typeof(Translation));
    }

    protected override void OnUpdate()
    {
        var bucketPositions = bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketVolumes = bucketQuery.ToComponentDataArray<Volume>(Allocator.TempJob);
        var bucketIDs = bucketQuery.ToEntityArray(Allocator.TempJob);

        Entities
            .WithDisposeOnCompletion(bucketPositions)
            .WithDisposeOnCompletion(bucketVolumes)
            .WithDisposeOnCompletion(bucketIDs)
            .WithAll<BucketFetcher>()
            .ForEach((Entity entity, ref Translation pos, ref Speed speed) =>
            {
                var bucketId = GetComponent<BucketID>(entity);
                
                // get new bucket target
                if(bucketId.Value == Entity.Null)
                {
                    int minIndex = bucketPositions.Length;
                    var minDist = float.MaxValue;
                    for (int i = 0; i < bucketPositions.Length; i++)
                    {
                        if (bucketVolumes[i].Value == 0.0f && bucketPositions[i].Value.y == 0.0f)
                        {
                            var currentDist = Unity.Mathematics.math.distance(bucketPositions[i].Value, pos.Value);
                            if (currentDist < minDist)
                            {
                                minDist = currentDist;
                                minIndex = i;
                            }
                        }
                    }

                    if(minIndex != bucketPositions.Length)
                        SetComponent(entity,new BucketID(){ Value = bucketIDs[minIndex] });
                }
            }).Run();
    }
}