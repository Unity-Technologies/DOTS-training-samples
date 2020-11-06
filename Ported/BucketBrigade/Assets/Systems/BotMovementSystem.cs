using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BotMovementSystem : SystemBase
{
    public EntityQuery readyBuckets;

    protected override void OnCreate()
    {
        base.OnCreate();
        readyBuckets = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BucketReadyFor>(), ComponentType.ReadOnly<Translation>());
    }
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var entities = readyBuckets.ToEntityArray(Allocator.TempJob);
        var translations = readyBuckets.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities
            .WithNone<GotoDropoffLocation>()
            .WithNone<HoldingBucket>()
            .WithNone<GotoPickupLocation>()
            .WithReadOnly(entities)
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref Translation translation, in Bot bot) =>
            {
                var index = FireSim.GetClosestIndex(translation.Value, entities, translations);
                if (index != -1 && math.distance(translation.Value, translations[index].Value) < 2.0f &&
                    GetComponent<BucketReadyFor>(entities[index]).Index == bot.Index)
                {
                    ecb.RemoveComponent<BucketReadyFor>(entities[index]);

                    ecb.AddComponent<GotoDropoffLocation>(entity);
                    ecb.AddComponent(entity, new HoldingBucket() { Target = entities[index] });

                    if (HasComponent<FillerBot>(entity))
                    {
                        ecb.AddComponent<FullBucket>(entities[index]);
                        ecb.RemoveComponent<EmptyBucket>(entities[index]);
                    }
                    else if (HasComponent<ThrowerBot>(entity))
                    {
                        ecb.AddComponent<EmptyBucket>(entities[index]);
                        ecb.RemoveComponent<FullBucket>(entities[index]);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        entities.Dispose();
        translations.Dispose();
    }
}
