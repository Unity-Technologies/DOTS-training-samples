using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FillBucketSystem : SystemBase
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
    // return true if the bucket is full
    public static bool RefillHelper(in Entity bucketID, in int sortkey, in TargetPosition targetPos, in int gridSize, in EntityCommandBuffer.ParallelWriter ecb, float newScale)
    {
        // if scaling = 0.5, it's empty. If 1, it's full
        newScale += 0.002f;
        bool isFull = newScale >= 1.0f;
        ecb.SetComponent<Bucket>(sortkey, bucketID, new Bucket{HasWater = isFull, isHeld = true});

        float ratio = (newScale - 0.5f) / 0.5f;

        // TODO : those colors should be CONST
        float4 fullColor = new float4(0.65f, 0.85f, 0.9f, 1.0f);
        float4 emptycolor = new float4(0.2f, 0, 0.6f, 1.0f);

        var newColor = (fullColor - emptycolor) * ratio + emptycolor;


        ecb.SetComponent<NonUniformScale>(sortkey, bucketID, new NonUniformScale{Value = new float3(newScale,newScale, newScale)});
        ecb.SetComponent<URPMaterialPropertyBaseColor>(sortkey, bucketID, new URPMaterialPropertyBaseColor{Value = newColor});
        return isFull;
    }

    protected override void OnUpdate()
    {
        // var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var sys = this.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var parallelEcb = sys.CreateCommandBuffer().AsParallelWriter();
        int gridSize = GetComponent<Spawner>(GetSingletonEntity<Spawner>()).GridSize;
        var heatSingleton = EntityManager.GetBuffer<Heat>(GetSingletonEntity<Heat>());

        /*** OmniWorker ***/
        // Refill buckets
        Entities
            .WithNativeDisableContainerSafetyRestriction(heatSingleton)
            .WithAny<OmniWorker>()
            .ForEach((int entityInQueryIndex, ref HeldBucket heldBucket, ref TargetPosition targetPos, in Translation pos) =>
            {
                // for each worker that does have a bucket with no water and if not moving somewhere
                if (math.all(pos.Value == targetPos.Value) && heldBucket.Bucket != Entity.Null)
                {
                    var bucketComp = GetComponent<Bucket>(heldBucket.Bucket);
                    if (!bucketComp.HasWater)
                    {
                        // Fill the bucket until full
                        var scaleX = GetComponent<NonUniformScale>(heldBucket.Bucket).Value.x;

                        if (RefillHelper(heldBucket.Bucket, entityInQueryIndex, targetPos, gridSize, parallelEcb, scaleX))
                        {
                            float closestFirePosition = float.MaxValue;

                            // look at every cell, and if one of them is on fire, look at the distance
                            for (int i = 0; i < heatSingleton.Length; i++)
                            {
                        
                                if (heatSingleton[i].Value > 0.0f)
                                {
                                    var xPosition = i % gridSize;
                                    var zPosition = i / gridSize;

                                    var currentDistance =
                                        math.pow(zPosition - pos.Value.z, 2) +
                                        math.pow(xPosition - pos.Value.x, 2);

                                    if (closestFirePosition > currentDistance)
                                    {
                                        closestFirePosition = currentDistance;
                                        targetPos.Value = new float3(xPosition, 1, zPosition);
                                    }
                                }
                            }
                        }
                    }
                }
            }).ScheduleParallel();

        /*** BucketFetcher ***/
        // Refill buckets
        Entities
            .WithAny<BucketFetcher>()
            .ForEach((int entityInQueryIndex, ref HeldBucket heldBucket, in TargetPosition targetPos, in Translation pos) =>
            {
                // for each worker that does have a bucket with no water and is not moving somewhere
                if (math.all(pos.Value == targetPos.Value) && heldBucket.Bucket != Entity.Null)
                {
                    var bucketComp = GetComponent<Bucket>(heldBucket.Bucket);
                    if (!bucketComp.HasWater)
                    {
                        var scaleX = GetComponent<NonUniformScale>(heldBucket.Bucket).Value.x;
                        
                        if (RefillHelper( heldBucket.Bucket, entityInQueryIndex, targetPos, gridSize, parallelEcb, scaleX))
                        {
                            bucketComp.HasWater = true;
                            BucketDropSystem.DropHelper(ref heldBucket, entityInQueryIndex, bucketComp, GetComponent<Translation>(heldBucket.Bucket), parallelEcb);
                        }

                    }
                }
            }).ScheduleParallel(); 
        

        /*** EmptyBucketWorker ***/
        // Does not refill buckets

        /*** FullBucketWorker ***/
        // Does not refill buckets

        sys.AddJobHandleForProducer(this.Dependency);
        // this.Dependency.Complete();
        // ecb.Playback(EntityManager);
        // ecb.Dispose();
    }
}