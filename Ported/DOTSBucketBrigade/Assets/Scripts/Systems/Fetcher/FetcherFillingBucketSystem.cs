using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FetcherFillingBucketSystem : SystemBase
{
    private EntityQuery m_BucketQuery;
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();

        var elapsedTime = (float) Time.ElapsedTime;
        var bucketFillRate = FireSimConfig.bucketFillRate;
        var bucketWaterLevels = m_BucketQuery.ToComponentDataArray<WaterLevel>(Allocator.TempJob);
        var bucketOwners = m_BucketQuery.ToComponentDataArray<BucketOwner>(Allocator.TempJob);
        var bucketEntities = m_BucketQuery.ToEntityArray(Allocator.TempJob);

        Entities
            .WithAll<Fetcher>()
            .WithAll<FetcherFillingBucket>()
            .WithNone<BucketFillStartTime>()
            .ForEach((Entity entity)=>
            {
                ecb.AddComponent<BucketFillStartTime>(entity, new BucketFillStartTime {Value = (float) elapsedTime});
            })
            .Schedule();

        // Loop through all fetchers that are in FetcherFillingBucket state and
        // update the water level.
        // If the bucket is full:
        //    * add FetcherFindBucket component
        //    * remove the FetcherFillingBucket component
        //    * and remove assigned bucket component.
        // Update to the bucket (in separate ForEach):
        //    * BucketOwner --> switch to be to team (i.e. keep the TeamIndex but isFetcher = false)
        Entities
            .WithAll<Fetcher>()
            .WithAll<FetcherFillingBucket>()
            .WithAll<BucketFillStartTime>()
            .WithReadOnly(bucketEntities)
            .WithDisposeOnCompletion(bucketEntities)
            .ForEach((Entity entity, int entityInQueryIndex, ref BucketFillStartTime bucketFillStartTime, in AssignedBucket assignedBucket) =>
            {
                // Find the index of the bucket assigned to the fetcher
                int bucketIndex = -1;
                for (var i=0; i<bucketEntities.Length; i++)
                {
                    if (bucketEntities[i] == assignedBucket.Value)
                    {
                        bucketIndex = i;
                    }
                }

                if (bucketIndex == -1)
                {
                    return;
                }

                var timeDiff = elapsedTime - bucketFillStartTime.Value;
                var bucketWaterLevel = bucketWaterLevels[bucketIndex];
                var bucketOwner = bucketOwners[bucketIndex];

                // Add to the water level
                bucketWaterLevel.Value += (float) (bucketFillRate * timeDiff);
                if (bucketWaterLevel.Value >= 1.0f)
                {
                    ecb.AddComponent<FetcherFindBucket>(entity, new FetcherFindBucket {});
                    ecb.RemoveComponent<FetcherFillingBucket>(entity);
                    ecb.RemoveComponent<BucketFillStartTime>(entity);
                    bucketOwner.Value *= -1;
                }

                bucketWaterLevels[bucketIndex] = bucketWaterLevel;
                bucketOwners[bucketIndex] = bucketOwner;
            })
            .Schedule();

        Entities
            .WithAll<Bucket>()
            .WithStoreEntityQueryInField(ref m_BucketQuery)
            .WithReadOnly(bucketWaterLevels)
            .WithReadOnly(bucketOwners)
            .WithDisposeOnCompletion(bucketWaterLevels)
            .WithDisposeOnCompletion(bucketOwners)
            .ForEach((Entity entity, int entityInQueryIndex,
                ref WaterLevel waterLevel, ref BucketOwner bucketOwner) =>
            {
                waterLevel.Value = bucketWaterLevels[entityInQueryIndex].Value;
                bucketOwner.Value = bucketOwners[entityInQueryIndex].Value;
            })
            .Schedule();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
