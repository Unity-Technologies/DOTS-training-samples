using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PassingBucketSystem : SystemBase
{
    public static EntityQuery buckets;

    protected override void OnCreate()
    {
        base.OnCreate();
        buckets = GetEntityQuery(ComponentType.ReadOnly<EmptyBucket>(), ComponentType.ReadOnly<Translation>());
    }       

    protected override void OnUpdate()
    {
    }
}

