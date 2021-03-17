using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class PickupBucketSystem : SystemBase
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
            .WithAll<BucketFetcher, TargetPosition>()
            .WithNone<CarryingBucket>()
            .ForEach((Entity entity, ref BucketID bucketId) =>
            {
                if (bucketId.Value != Entity.Null)
                {
                    var targetBucketPos = GetComponent<Translation>(bucketId.Value);
                    var pos = GetComponent<Translation>(entity);
                    if (math.distance(targetBucketPos.Value.x, pos.Value.x) < 0.001f &&
                        math.distance(targetBucketPos.Value.z, pos.Value.z) < 0.001f)
                    {
                        ecb.AddComponent(entity, new CarryingBucket());
                        //ecb.SetComponent(entity,new TargetPosition());// change to water source
                    }
                }
            }).Run();
    }
}