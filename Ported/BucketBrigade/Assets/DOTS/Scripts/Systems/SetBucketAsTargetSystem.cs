using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SetBucketAsTargetSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    private EntityQuery BucketQuery;

    protected override void OnCreate()
    {
        BucketQuery = GetEntityQuery(ComponentType.ReadOnly<Bucket>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<EmptyBucket>(), ComponentType.Exclude<BeingHeld>());
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<GameConstants>();
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        //var chunks = LakeQuery.CreateArchetypeChunkArray(Allocator.TempJob);

        var bucketTranslations = BucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        if (bucketTranslations.Length == 0)
            return;

        var bucketEntities = BucketQuery.ToEntityArray(Allocator.TempJob);


        // TODO: if there are no flames don't do anything
        Entities
            .WithAll<BucketFetcher>()
            .WithNone<HoldingBucket, TargetDestination>()
            .WithReadOnly(bucketTranslations)
            .WithDisposeOnCompletion(bucketTranslations)
            .WithReadOnly(bucketEntities)
            .WithDisposeOnCompletion(bucketEntities)
            .ForEach((Entity e, int entityInQueryIndex, in Translation translation) =>
            {
                // HACK: We assume that a flame exists here...
                var closestIndex = -1;
                var closest = new float2(10000000, 100000); // This is bad HACK
                var bestDistance = float.MaxValue;
                // HACK: We are mixing types, this is awful.

                for (int i = 0; i < bucketTranslations.Length; i++)
                {
                    var dist = math.distance(bucketTranslations[i].Value, translation.Value);

                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        closest = bucketTranslations[i].Value.xz;
                        closestIndex = i;
                    }
                }

                // TODO: If distance is close enough to fill bucket, set a "filling bucket" tag instead
                if (bestDistance < 0.1f)
                {
                    var bucket = bucketEntities[closestIndex];
                    ecb.AddComponent<HoldsEmptyBucket>(entityInQueryIndex, e);
                    ecb.AddComponent(entityInQueryIndex, e, new HoldingBucket { HeldBucket = bucket });
                    ecb.AddComponent<BeingHeld>(entityInQueryIndex, bucket);
                }
                else
                {
                    ecb.AddComponent(entityInQueryIndex, e, new TargetDestination { Value = closest });
                }
            }).ScheduleParallel();


        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
