using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Analytics;

public class ScooperBotSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var bucketQuery = EntityManager.CreateEntityQuery(typeof(BucketActiveComponent), typeof(Translation));
        var bucketEntities = bucketQuery.ToEntityArray(Allocator.TempJob);
        var bucketActive = bucketQuery.ToComponentDataArray<BucketActiveComponent>(Allocator.TempJob);
        var bucketTranslation = bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        //var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        //var ecb = ecbs.CreateCommandBuffer();

        Entities
            .WithDisposeOnCompletion(bucketEntities)
            .WithDisposeOnCompletion(bucketActive)
            .WithDisposeOnCompletion(bucketTranslation)
            .ForEach((in BotsChainComponent chain) =>
        {
            var scooper = chain.scooper;
            var carriedBucket = GetComponent<CarriedBucket>(scooper);
            if (carriedBucket.bucket != Entity.Null)
                return;

            var currentTargetBucket = GetComponent<TargetBucket>(scooper);
            var scooperPos = GetComponent<Translation>(scooper).Value.xz;
            if (currentTargetBucket.bucket != Entity.Null)
            {
                var targetBucket = currentTargetBucket.bucket;
                if (GetComponent<BucketActiveComponent>(targetBucket).active)
                {
                    // Clear our own target bucket: it got stolen by another bot.
                    Debug.Log("Clearing target bucket, someone stole it.");
                    SetComponent<TargetBucket>(scooper, new TargetBucket());
                }
                else
                {
                    // Acquire the bucket. Would be nice to put in a separate function, but couldn't find a way.
                    var bucketPos = GetComponent<Translation>(targetBucket).Value.xz;

                    var dist = math.length(bucketPos - scooperPos);

                    var bucketProximityThreshold = 0.01F;
                    if (dist > bucketProximityThreshold)
                        return;

                    SetComponent(scooper, new CarriedBucket() {bucket = targetBucket});
                    SetComponent(currentTargetBucket.bucket, new BucketActiveComponent() { active = true });
                    //ecb.AddComponent(targetBucket, new Parent() {Value = scooper});
                    //ecb.AddComponent(targetBucket, new LocalToParent() { Value = float4x4.identity});
                    var targetWaterPosition = float2.zero;
                    SetComponent(scooper, new TargetLocationComponent(){location = targetWaterPosition});
                    Debug.Log("Carrying new bucket");
                    return;
                }
            }
            
            var bestBucketPosition = float2.zero;

            float bestBucketDistance = float.MaxValue;
            var bestBucket = Entity.Null;
            for (var i = 0; i < bucketEntities.Length; ++i)
            {
                if (bucketActive[i].active)
                    continue;

                var bucketPosition = bucketTranslation[i].Value.xz;
                var distanceToBucket = math.distancesq(bucketPosition, scooperPos);

                if (distanceToBucket > bestBucketDistance)
                {
                    Debug.Log("This bucket is too far.");
                    continue;
                }

                bestBucketDistance = distanceToBucket;
                bestBucket = bucketEntities[i];
                bestBucketPosition = bucketPosition;
            }

            if (bestBucketDistance < float.MaxValue)
            {
                Debug.Log("Targeting new bucket.");
                SetComponent(scooper, new TargetBucket(){bucket = bestBucket});
                SetComponent(scooper, new TargetLocationComponent(){location = bestBucketPosition});
            }
        }).Schedule();
        
        //ecbs.AddJobHandleForProducer(Dependency);
    }
}
