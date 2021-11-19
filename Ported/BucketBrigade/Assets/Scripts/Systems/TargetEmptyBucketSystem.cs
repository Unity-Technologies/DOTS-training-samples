using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TargetEmptyBucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var bucketQuery = GetEntityQuery(typeof(Translation), typeof(Bucket));
        var bucketTranslations = bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketComponentArray = bucketQuery.ToComponentDataArray<Bucket>(Allocator.TempJob);
        //var bucketEntities = bucketQuery.ToEntityArray(Allocator.TempJob);

        Entities
            .WithNativeDisableContainerSafetyRestriction(bucketTranslations)
            .WithNativeDisableContainerSafetyRestriction(bucketComponentArray)
            .WithDisposeOnCompletion(bucketTranslations)
            .WithDisposeOnCompletion(bucketComponentArray)
            .WithAny<OmniWorker, BucketFetcher>()
            .ForEach((ref TargetPosition targetPosition, in HeldBucket heldBucket, in Translation currentWorkerPosition) =>
            {
                //ecb.SetComponent(bucketEntities[0], new Translation(){Value = float3.zero});
                if (heldBucket.Bucket == Entity.Null)
                {
                    targetPosition.Value = currentWorkerPosition.Value; 
                    
                    float closestBucketPosition = float.MaxValue;

                    for (int i = 0; i < bucketTranslations.Length; i++)
                    {
                        if (!bucketComponentArray[i].HasWater && !bucketComponentArray[i].isHeld)
                        {
                            // Don't need sqrt if just comparing distances to find closest
                            var currentDistance = //math.sqrt(
                                math.pow(bucketTranslations[i].Value.z - currentWorkerPosition.Value.z, 2) +
                                math.pow(bucketTranslations[i].Value.x - currentWorkerPosition.Value.x, 2)/*)*/;

                            if (closestBucketPosition > currentDistance)
                            {
                                closestBucketPosition = currentDistance;
                                targetPosition.Value = new float3(bucketTranslations[i].Value.x, 1, bucketTranslations[i].Value.z);
                                
                            }
                        }
                    }
                }
            }).ScheduleParallel();
    }
}