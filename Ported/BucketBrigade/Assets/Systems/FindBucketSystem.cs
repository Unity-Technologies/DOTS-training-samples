
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using System.Diagnostics;
using Unity.Rendering;

public class FindBucketSystem : SystemBase
{
    public EntityQuery buckets;

    protected override void OnCreate()
    {
        base.OnCreate();
        buckets = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BucketForScooper>(), ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        var bucketsEntitiesOriginal = buckets.ToEntityArray(Allocator.Temp);
        var bucketsEntitiesCopy = bucketsEntitiesOriginal;

        var bucketsTranslationsOriginal = buckets.ToComponentDataArray<Translation>(Allocator.Temp);
        var bucketsTranslationsCopy = bucketsTranslationsOriginal;

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .ForEach((Entity entity, in FindBucket bot, in FillerBot chainstart, in Translation translation) =>
            {
                ecb.RemoveComponent<FindBucket>(entity);

                if (bucketsEntitiesCopy.Length > 0)
                {
                    int closestBucketIndex = FireSim.GetClosestIndex(translation.Value, bucketsEntitiesCopy, bucketsTranslationsCopy);

                    Entity closestBucket = bucketsEntitiesCopy[closestBucketIndex];
                    bucketsEntitiesCopy[closestBucketIndex] = bucketsEntitiesCopy[bucketsEntitiesCopy.Length - 1];
                    bucketsTranslationsCopy[closestBucketIndex] = bucketsTranslationsCopy[bucketsEntitiesCopy.Length - 1];

                    bucketsEntitiesCopy = bucketsEntitiesCopy.GetSubArray(0, bucketsEntitiesCopy.Length - 1);
                    bucketsTranslationsCopy = bucketsTranslationsCopy.GetSubArray(0, bucketsTranslationsCopy.Length - 1);

                    ecb.RemoveComponent<BucketForScooper>(closestBucket);
                    ecb.AddComponent(entity, new MoveTowardBucket() { Target = closestBucket });
                }
            }).Run();

        ecb.Playback(World.EntityManager);
        ecb.Dispose();

        bucketsEntitiesOriginal.Dispose();
        bucketsTranslationsOriginal.Dispose();
    }
}