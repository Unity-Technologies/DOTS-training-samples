using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PickLinePositionsForTeamSystem))]
public partial class SetLakeAsTargetSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    private EntityQuery BucketQuery;

    protected override void OnCreate()
    {
        BucketQuery = GetEntityQuery(ComponentType.ReadOnly<Bucket>(), ComponentType.ReadOnly<BeingHeld>());
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var buckets = BucketQuery.ToComponentDataArray<Bucket>(Allocator.TempJob);
        var bucketEntities = BucketQuery.ToEntityArray(Allocator.TempJob);

        Entities
            .WithReadOnly(buckets)
            .WithDisposeOnCompletion(buckets)
            .WithReadOnly(bucketEntities)
            .WithDisposeOnCompletion(bucketEntities)
            .WithNone<HoldsBucketBeingFilled>()
            .ForEach((Entity e, int entityInQueryIndex, ref TargetDestination targetDestination, in Translation translation, in BucketFetcher bucketFetcher,
                in HoldingBucket holdingBucket) =>
            {
                if (!targetDestination.IsAtDestination(translation))
                    return;

                int index = -1;
                for (int i = 0; i < bucketEntities.Length; ++i)
                {
                    if (bucketEntities[i] == holdingBucket.HeldBucket)
                    {
                        index = i;
                        break;
                    }
                }
                
                var distance = math.distance(bucketFetcher.LakePosition, translation.Value);

                if (distance < 0.1f )
                {
                    // Do nothing, lake will be updated by team system in the next frame and we want to go there
                    if (HasComponent<EmptyLake>(bucketFetcher.Lake))
                        return;

                    ecb.AddComponent<HoldsBucketBeingFilled>(entityInQueryIndex, e);
                    ecb.AppendToBuffer(entityInQueryIndex, bucketFetcher.Lake,
                        new BucketFillAction
                        {
                            Bucket = holdingBucket.HeldBucket, FireFighter = e,
                            BucketVolume = buckets[index].Volume,
                            Position = translation.Value
                        });
                }
                else
                {
                    targetDestination = bucketFetcher.LakePosition.xz;
                }
            }).ScheduleParallel();


        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}