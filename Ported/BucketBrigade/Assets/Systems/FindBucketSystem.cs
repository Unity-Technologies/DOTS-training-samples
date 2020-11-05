
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using System.Diagnostics;

public class FindBucketSystem : SystemBase
{
    public static EntityQuery buckets;

    protected override void OnCreate()
    {
        base.OnCreate();
        buckets = GetEntityQuery(ComponentType.ReadOnly<EmptyBucket>(), ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var bucketsEntities = buckets.ToEntityArray(Allocator.Temp);
        var bucketsTranslations = buckets.ToComponentDataArray<Translation>(Allocator.Temp);

        Entities.ForEach((Entity entity, in FindBucket bot, in Translation translation) =>
        {
            ecb.RemoveComponent<FindBucket>(entity);

            Entity closestBucket = FireSim.GetClosestEntity(translation.Value, bucketsEntities, bucketsTranslations);

            UnityEngine.Debug.Log("hrllo");
            ecb.AddComponent(entity, new MoveTowardBucket() { Target = closestBucket });
        }).Run();

        ecb.Playback(World.EntityManager);
        ecb.Dispose();

        bucketsEntities.Dispose();
        bucketsTranslations.Dispose();
    }
}