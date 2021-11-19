using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

public partial class BucketEmptySystem : SystemBase
{
    protected override void OnCreate()
    {
        // Wait for the specified instanciations
        RequireSingletonForUpdate<Heat>();
    }

    // TODO: We are random accessing the bucket that each worker has. But we can't foreach over the buckets instead unless the
    //       buckets store a reference to the worker carrying it (to find out if it should be emptied).
    //
    //      OR we could foreach bucket, loop through all the workers to find the worker that has that bucket. Is that more performant than this?
    public static void EmptyHelper(ref DynamicBuffer<Heat> heatMap, in int sortkey, in Entity bucketID, in TargetPosition targetPos, in int gridSize, in EntityCommandBuffer.ParallelWriter ecb)
    {
        // set bucket to not have water
        ecb.SetComponent<Bucket>(sortkey, bucketID, new Bucket{HasWater = false, isHeld = true});
        ecb.SetComponent<NonUniformScale>(sortkey, bucketID, new NonUniformScale{Value = new float3(0.5f,0.5f, 0.5f)});
        ecb.SetComponent<URPMaterialPropertyBaseColor>(sortkey, bucketID, new URPMaterialPropertyBaseColor{Value = new float4(0.2f, 0, 0.6f, 1.0f)});

        // reduce fire heat

        // for now, assume that targetPosition is correctly the same as heatmap's grid indices
        int x = (int)targetPos.Value.x;
        int y = (int)targetPos.Value.z;
        for (int i = 0; i < 5; i++)
        {
            int xIndex = x + i - 2;
            if (xIndex >= 0 && xIndex < gridSize)
            {
                for (int j = 0; j < 5; j++)
                {
                    int yIndex = y + j - 2;
                    if (yIndex >= 0 && yIndex < gridSize)
                    {
                        int finalIndex = xIndex + gridSize * yIndex;
                        if (i == 0 || i == 4 || j == 0 || j == 4)
                            heatMap[finalIndex] = new Heat{Value = math.max(heatMap[finalIndex].Value - 0.5f, 0.0f)};
                        else
                            heatMap[finalIndex] = Heat.NonBurningHeat;
                    }
                }
            }
        }
    }

    protected override void OnUpdate()
    {
        // var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var sys = this.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var parallelEcb = sys.CreateCommandBuffer().AsParallelWriter();
        var heatSingleton = EntityManager.GetBuffer<Heat>(GetSingletonEntity<Heat>());
        int gridSize = GetComponent<Spawner>(GetSingletonEntity<Spawner>()).GridSize;

        /*** OmniWorker ***/
        Entities
            .WithAny<OmniWorker>()
            .ForEach((int entityInQueryIndex, ref HeldBucket heldBucket, in TargetPosition targetPos, in Translation pos) =>
            {
                // for each worker that does have a bucket full of water and if not moving somewhere
                if (math.all(pos.Value == targetPos.Value) && heldBucket.Bucket != Entity.Null)
                {
                    var bucketComp = GetComponent<Bucket>(heldBucket.Bucket);
                    if (bucketComp.HasWater)
                    {
                        EmptyHelper(ref heatSingleton, entityInQueryIndex, heldBucket.Bucket, targetPos, gridSize, parallelEcb);
                        bucketComp.HasWater = false;
                        BucketDropSystem.DropHelper(ref heldBucket, entityInQueryIndex, bucketComp, GetComponent<Translation>(heldBucket.Bucket), parallelEcb);
                    }
                }
            }).Schedule();  // TODO: ScheduleParallel?

        /*** BucketFetcher ***/
        // Does not empty buckets


        /*** EmptyBucketWorker ***/
        // Does not empty buckets


        /*** FullBucketWorker ***/
        // Only the end of the line worker empties the bucket.
        Entities
            .WithAll<FullBucketWorker, LastWorker>()
            .ForEach((int entityInQueryIndex, ref HeldBucket heldBucket, in TargetPosition targetPos, in Translation pos) =>
            {
                // does have a bucket full of water and if not moving somewhere
                if (math.all(pos.Value == targetPos.Value) && heldBucket.Bucket != Entity.Null)
                {
                    var bucketComp = GetComponent<Bucket>(heldBucket.Bucket);
                    if (bucketComp.HasWater)
                    {
                        EmptyHelper(ref heatSingleton, entityInQueryIndex, heldBucket.Bucket, targetPos, gridSize, parallelEcb);
                        bucketComp.HasWater = false;
                        BucketDropSystem.DropHelper(ref heldBucket, entityInQueryIndex, bucketComp, GetComponent<Translation>(heldBucket.Bucket), parallelEcb);
                    }
                }
            }).Schedule();  // TODO: ScheduleParallel?

        sys.AddJobHandleForProducer(this.Dependency);
        // this.Dependency.Complete();
        // ecb.Playback(EntityManager);
        // ecb.Dispose();
    }
}
