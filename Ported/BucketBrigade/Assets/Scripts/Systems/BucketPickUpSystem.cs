using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
// using UnityEngine;

public partial class BucketPickUpSystem : SystemBase
{
    private const float pickUpRadius = 1.5f;

    protected override void OnUpdate()
    {
        // This would be faster if we could query only the buckets that aren't held (ie. make structural changes)
        var bucketQuery = GetEntityQuery(typeof(Bucket), typeof(Translation));
        var bucketTranslations = bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketComponents = bucketQuery.ToComponentDataArray<Bucket>(Allocator.TempJob);
        var bucketEntities = bucketQuery.ToEntityArray(Allocator.TempJob);

        var waterPatchQuery = GetEntityQuery(typeof(Translation), typeof(WaterPatch));
        var waterTranslations = waterPatchQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        var sys = this.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        // var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecb = sys.CreateCommandBuffer();

        bool PickUpHelper(ref HeldBucket heldBucket, in TargetPosition targetPos, in Translation pos, in bool bucketNeedsWater)
        {
            // for each worker that doesn't have a bucket and if not moving somewhere
            if (math.all(pos.Value == targetPos.Value) && heldBucket.Bucket == Entity.Null)
            {
                for (int i = 0; i < bucketComponents.Length; i++)
                {
                    var bucketComp = bucketComponents[i];
                    if (!bucketComp.isHeld && bucketComp.HasWater == bucketNeedsWater)
                    {
                        // check all non-held buckets to see if a bucket is in range
                        var bucketPos = bucketTranslations[i].Value;
                        float dist2 = (bucketPos.x - pos.Value.x) * (bucketPos.x - pos.Value.x) + (bucketPos.z - pos.Value.z) * (bucketPos.z - pos.Value.z);
                        if (dist2 < pickUpRadius)
                        {
                            heldBucket.Bucket = bucketEntities[i];
                            ecb.SetComponent<Translation>(bucketEntities[i], new Translation{Value = new float3(pos.Value.x, 2.25f, pos.Value.z)});
                            bucketComp.isHeld = true;
                            bucketComponents[i] = bucketComp;
                            ecb.SetComponent<Bucket>(bucketEntities[i], bucketComp);
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        /*** OmniWorker ***/

        Entities
            // TODO : This might be faster if we could query only the workers that don't have buckets (ie. make structural changes by adding/removing HeldBucket)
            .WithDisposeOnCompletion(waterTranslations)
            .WithAny<OmniWorker>()
            .ForEach((ref HeldBucket heldBucket, ref TargetPosition targetPos, in Translation pos) =>
            {
                if (PickUpHelper(ref heldBucket, targetPos, pos, false))
                {
                   // Target closest water patch
                    float closestPosition = float.MaxValue;
                   for (int i = 0; i < waterTranslations.Length; i++)
                   {
                       var currentDistance =
                            math.pow(waterTranslations[i].Value.z - pos.Value.z, 2) +
                            math.pow(waterTranslations[i].Value.x - pos.Value.x, 2);

                        if (closestPosition > currentDistance)
                        {
                            closestPosition = currentDistance;
                            targetPos.Value = new float3(waterTranslations[i].Value.x, 1, waterTranslations[i].Value.z);
                        }
                   }
                }
            }).Schedule();


        /*** BucketFetcher ***/

        Entities
            .WithAny<BucketFetcher>()
            .ForEach((ref HeldBucket heldBucket, ref TargetPosition targetPos, in TargetOriginalPosition origPos, in Translation pos) =>
            {
                if (PickUpHelper(ref heldBucket, targetPos, pos, false))
                    // Target team water patch (was set as orig pos)
                    targetPos.Value = origPos.Value;

            }).Schedule();


        /*** EmptyBucketWorker ***/

        Entities
            .ForEach((Entity e, ref HeldBucket heldBucket, ref TargetPosition targetPos, in Translation pos, in EmptyBucketWorker worker) =>
            {
                if (PickUpHelper(ref heldBucket, targetPos, pos, false))
                    if (worker.nextWorker != Entity.Null)
                        // Target next worker in line
                        targetPos.Value = GetComponent<TargetOriginalPosition>(worker.nextWorker).Value;
                    else
                        targetPos.Value = GetComponent<LastWorker>(e).targetPosition;
            }).Schedule();


        /*** FullBucketWorker ***/

        Entities
            .WithDisposeOnCompletion(bucketTranslations)
            .WithDisposeOnCompletion(bucketComponents)
            .WithDisposeOnCompletion(bucketEntities)
            .ForEach((Entity e, ref HeldBucket heldBucket, ref TargetPosition targetPos, in Translation pos, in FullBucketWorker worker) =>
            {
                if (PickUpHelper(ref heldBucket, targetPos, pos, true))
                    if (worker.nextWorker != Entity.Null)
                        // Target next worker in line
                        targetPos.Value = GetComponent<TargetOriginalPosition>(worker.nextWorker).Value;
                    else
                        targetPos.Value = GetComponent<LastWorker>(e).targetPosition;
            }).Schedule();

        sys.AddJobHandleForProducer(this.Dependency);
        // this.Dependency.Complete();
        // ecb.Playback(EntityManager);
        // ecb.Dispose();
    }
}
