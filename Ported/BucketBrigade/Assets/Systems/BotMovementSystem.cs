using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BotMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .ForEach((Entity entity, ref Translation translation, in GotoPickupLocation bot, in PasserBot chain) =>
            {
                var distanceLeft = chain.PickupPosition - translation.Value;
                var length = math.length(distanceLeft);

                if (length <= MoveTowardEntitySystem.Threshold)
                {
                    translation.Value = chain.PickupPosition;
                    ecb.RemoveComponent<GotoPickupLocation>(entity);
                }
                else
                {
                    var direction = distanceLeft / length;
                    var newPosition = translation.Value + MoveTowardEntitySystem.Speed * deltaTime * direction;
                    translation.Value = newPosition;
                }
            }).Run();

        var query = GetEntityQuery(ComponentType.ReadOnly<BucketReadyFor>(), ComponentType.ReadOnly<Translation>());
        var entities = query.ToEntityArray(Allocator.TempJob);
        var translations = query.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities
            .WithNone<GotoPickupLocation>()
            //.WithNone<ThrowerBot>()
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
