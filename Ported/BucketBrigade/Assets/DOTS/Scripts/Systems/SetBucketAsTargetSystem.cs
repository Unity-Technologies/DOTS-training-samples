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
        var bucketEntities = BucketQuery.ToEntityArray(Allocator.TempJob);


        // TODO: if there are no flames don't do anything
        Entities
            .WithAll<BucketFetcher>()
            .WithNone<HoldingBucket>()
            .WithReadOnly(bucketTranslations)
            .WithDisposeOnCompletion(bucketTranslations)
            .WithReadOnly(bucketEntities)
            .WithDisposeOnCompletion(bucketEntities)
            .ForEach((Entity e, int entityInQueryIndex, ref TargetDestination targetDestination, in Translation translation) =>
            {
                if (bucketTranslations.Length == 0 || !targetDestination.IsAtDestination(translation))
                    return;

                var closestIndex = 0;
                var closest = bucketTranslations[0].Value.xz;
                var bestDistance = float.MaxValue;

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

                if (bestDistance < 0.1f)
                {
                    var bucket = bucketEntities[closestIndex];
                    ecb.AddComponent(entityInQueryIndex, e, new HoldingBucket { HeldBucket = bucket });
                    ecb.AddComponent<BeingHeld>(entityInQueryIndex, bucket);
                }
                else
                {
                    targetDestination = closest;
                }
            }).ScheduleParallel();


        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
