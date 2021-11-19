using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class BucketDropSystem : SystemBase
{
    public static void DropHelper(ref HeldBucket heldBucket, in int sortkey, in Bucket bucketComp, in Translation bucketTranslation, in EntityCommandBuffer.ParallelWriter ecb)
    {
        var translation = bucketTranslation.Value;
        bool bucketWater = bucketComp.HasWater;

        // For debugging, feel free to set the x and z value offset as well so the bucket becomes visible when dropped
        translation.y = 0.25f;
        ecb.SetComponent<Translation>(sortkey, heldBucket.Bucket, new Translation{Value = translation});

        ecb.SetComponent<Bucket>(sortkey, heldBucket.Bucket, new Bucket{HasWater = bucketWater, isHeld = false});
        heldBucket.Bucket = Entity.Null;
    }

    protected override void OnUpdate()
    {
        // var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var sys = this.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var parallelEcb = sys.CreateCommandBuffer().AsParallelWriter();


        /*** OmniWorker ***/
        // Should call DropHelper after EmptyBucket


        /*** BucketFetcher ***/
        // Should call DropHelper after FillBucket


        /*** EmptyBucketWorker and FullBucketWorker ***/
        Entities
            .WithAny<EmptyBucketWorker, FullBucketWorker>()
            .ForEach((int entityInQueryIndex, Entity e, ref HeldBucket heldBucket, ref TargetPosition targetPos, in TargetOriginalPosition origPos, in Translation pos) =>
            {
                // for each worker that does have a bucket and if not moving somewhere
                if (math.all(pos.Value == targetPos.Value) && heldBucket.Bucket != Entity.Null)
                {
                    var bucketComp = GetComponent<Bucket>(heldBucket.Bucket);
                    // TODO: better way to check this?
                    if (!bucketComp.HasWater || !HasComponent<LastWorker>(e))
                    {
                        DropHelper(ref heldBucket, entityInQueryIndex, bucketComp, GetComponent<Translation>(heldBucket.Bucket), parallelEcb);

                        // Target orig line position
                        targetPos.Value = origPos.Value;
                    }
                }
            }).ScheduleParallel();

        sys.AddJobHandleForProducer(this.Dependency);
        // this.Dependency.Complete();
        // ecb.Playback(EntityManager);
        // ecb.Dispose();
    }
}
