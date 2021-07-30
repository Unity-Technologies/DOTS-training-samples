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
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfigComponent>();
        Enabled = false;
    }

    protected override void OnUpdate()
    {
        var bucketQuery = EntityManager.CreateEntityQuery(typeof(BucketActiveComponent), typeof(Translation), typeof(BucketFullComponent));
        var bucketEntities = bucketQuery.ToEntityArray(Allocator.TempJob);
        var bucketActive = bucketQuery.ToComponentDataArray<BucketActiveComponent>(Allocator.TempJob);
        var bucketTranslation = bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketFullArray = bucketQuery.ToComponentDataArray<BucketFullComponent>(Allocator.TempJob);
        
        var config = GetSingleton<GameConfigComponent>();
        var distanceThreshold = config.TargetProximityThreshold;
        //var ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        //var ecb = ecbs.CreateCommandBuffer();

        Entities
            .WithDisposeOnCompletion(bucketEntities)
            .WithDisposeOnCompletion(bucketActive)
            .WithDisposeOnCompletion(bucketTranslation)
            .WithDisposeOnCompletion(bucketFullArray)
            .ForEach((in BotsChainComponent chain) =>
        {
            var scooper = chain.scooper;
            var carriedBucket = GetComponent<CarriedBucket>(scooper);
            var scooperPos = GetComponent<Translation>(scooper).Value.xz;
            
            if (carriedBucket.bucket != Entity.Null)
            {
                var water = GetComponent<TargetWater>(scooper);
                if (water.water == Entity.Null)
                    return;
                
                var targetWaterPosition = GetComponent<Translation>(water.water).Value.xz;
                
                
                //check current position vs water position
                var dist = math.length(targetWaterPosition - scooperPos);

                if (dist > distanceThreshold)
                    return;
                
                //check bucket state
                var bucketfull = GetComponent<BucketFullComponent>(carriedBucket.bucket).full;
                if (bucketfull)
                {
                    //drop the bucket
                    var bucketPos = new float3(scooperPos.x,0f, scooperPos.y);
                    SetComponent(carriedBucket.bucket, new Translation(){Value = bucketPos});
                    SetComponent(carriedBucket.bucket, new BucketActiveComponent(){active = false});
                    SetComponent(scooper, new TargetBucket());
                    SetComponent(scooper, new CarriedBucket());
                }
                else
                {
                    //fill the bucket
                    //check first
                    var fillingwater = GetComponent<BucketStartFill>(carriedBucket.bucket).Water;
                    if (fillingwater != Entity.Null)
                        return;
                    
                    SetComponent(carriedBucket.bucket, new BucketStartFill(){Water = water.water});
                }

                
                
                
                return;
            }

           

            var currentTargetBucket = GetComponent<TargetBucket>(scooper);
            
            if (currentTargetBucket.bucket != Entity.Null)
            {
                var targetBucket = currentTargetBucket.bucket;
                if (GetComponent<BucketActiveComponent>(targetBucket).active)
                {
                    // Clear our own target bucket: it got stolen by another bot.
                    SetComponent<TargetBucket>(scooper, new TargetBucket());
                }
                else
                {
                    // Acquire the bucket. Would be nice to put in a separate function, but couldn't find a way.
                    var bucketPos = GetComponent<Translation>(targetBucket).Value.xz;

                    var dist = math.length(bucketPos - scooperPos);

                    if (dist > distanceThreshold)
                        return;

                    SetComponent(scooper, new CarriedBucket() {bucket = targetBucket});
                    SetComponent(currentTargetBucket.bucket, new BucketActiveComponent() { active = true });
                    var water = GetComponent<TargetWater>(scooper);
                    var targetWaterPosition = GetComponent<Translation>(water.water).Value.xz;
                    SetComponent(scooper, new TargetLocationComponent(){location = targetWaterPosition});
                    return;
                }
            }

            
            
            var bestBucketPosition = float2.zero;

            float bestBucketDistance = float.MaxValue;
            var bestBucket = Entity.Null;
            for (var i = 0; i < bucketEntities.Length; ++i)
            {
                if (bucketActive[i].active || bucketFullArray[i].full)
                    continue;
                

                var bucketPosition = bucketTranslation[i].Value.xz;
                var distanceToBucket = math.distancesq(bucketPosition, scooperPos);

                if (distanceToBucket > bestBucketDistance)
                {
                    continue;
                }

                bestBucketDistance = distanceToBucket;
                bestBucket = bucketEntities[i];
                bestBucketPosition = bucketPosition;
            }

            if (bestBucketDistance < float.MaxValue)
            {
                SetComponent(scooper, new TargetBucket(){bucket = bestBucket});
                SetComponent(scooper, new TargetLocationComponent(){location = bestBucketPosition});
            }
        }).Schedule();
        
        //ecbs.AddJobHandleForProducer(Dependency);
    }
}
