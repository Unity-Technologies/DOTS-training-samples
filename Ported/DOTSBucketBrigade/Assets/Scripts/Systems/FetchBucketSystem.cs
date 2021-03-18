using System;
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
        Entities
            .WithAll<BucketFetcher>()
            .WithNone<CarryingBucket>()
            .ForEach((Entity entity, ref TargetPosition targetPos, in BucketID bucketId) =>
            {
                if (bucketId.Value != Entity.Null)
                {
                    var targetBucketPos = GetComponent<Translation>(bucketId.Value);
                    if (!targetPos.Value.Equals(targetBucketPos.Value))
                    {
                        targetPos.Value = targetBucketPos.Value;
                    }
                }
            }).Run();
    }
}
