using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class FetchBucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .WithAll<BucketFetcher,CarryingBucket>()
            .WithNone<TargetPosition>()
            .ForEach((Entity entity, ref BucketID bucketId) =>
            {
                if (bucketId.Value != Entity.Null)
                {
                    var targetBucketPos = GetComponent<Translation>(bucketId.Value);
                    ecb.AddComponent(entity, new TargetPosition(){ Value = targetBucketPos.Value});
                }
            }).Run();
    }
}
