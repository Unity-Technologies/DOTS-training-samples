using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class FillBucketSystem : SystemBase
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
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .WithAll<BucketFetcher, CarryingBucket, TargetPosition>()
            .ForEach((Entity entity, ref BucketID bucketId) =>
            {
                var targetWaterPos = GetComponent<Translation>(bucketId.Value); //todo change to water source
                var pos = GetComponent<Translation>(entity);
                if (math.distance(targetWaterPos.Value.x,pos.Value.x) < 0.001f && math.distance(targetWaterPos.Value.z, pos.Value.z) < 0.001f)
                {
                    
                }
            }).Run();
    }
}