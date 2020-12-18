using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FetcherFindBucketSystem : SystemBase
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

        // Assign a bucket to each fetcher
        Entities
            .WithAll<Fetcher>()
            .ForEach((Entity entity, ref Position position, ref Translation translation) =>
            {
                translation.Value = new float3(position.coord.x, 0.9f, position.coord.y);
            })
            .Schedule();

        var bucketTranslations = m_BucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketOwners = m_BucketQuery.ToComponentDataArray<BucketOwner>(Allocator.TempJob);
        var bucketEntities = m_BucketQuery.ToEntityArray(Allocator.TempJob);

        // For each bucket, fetcher entity that it is assigned to
        var assignedFetcherEntities = new NativeArray<Entity>(BucketConfig.nBuckets, Allocator.TempJob);

        for (var i=0; i<assignedFetcherEntities.Length; i++)
        {
            assignedFetcherEntities[i] = Entity.Null;
        }

        ComponentType tagComponentToAddOnArrival = ComponentType.ReadWrite<FetcherFindWaterSource>();
        var startTime = Time.ElapsedTime;

        // Assign buckets to the fetchers
        Entities
            .WithAll<Fetcher, FetcherFindBucket>()
            .WithNone<AssignedBucket>()
            .WithReadOnly(bucketTranslations)
            .WithDisposeOnCompletion(bucketTranslations)
            .WithDisposeOnCompletion(bucketEntities)
            .WithDisposeOnCompletion(assignedFetcherEntities)
            .ForEach((Entity entity, in Translation translation, in TeamIndex teamIndex) =>
            {
                float minDistance = float.MaxValue;
                int minDistanceIndex = -1;
                for (var i=0; i<bucketTranslations.Length; i++)
                {
                    if (assignedFetcherEntities[i] == Entity.Null)
                    {
                        var newMinDistance = GetSquaredDistance(bucketTranslations[i], translation);
                        if (newMinDistance < minDistance)
                        {
                            minDistance = newMinDistance;
                            minDistanceIndex = i;
                        }
                    }
                }

                if (minDistanceIndex != -1)
                {
                    if (assignedFetcherEntities[minDistanceIndex] == Entity.Null)
                    {
                        ecb.AddComponent<AssignedBucket>(entity, new AssignedBucket {Value = bucketEntities[minDistanceIndex]});
                        ecb.RemoveComponent<FetcherFindBucket>(entity);
                        ecb.AddComponent<MovingBot>(entity, new MovingBot
                        {
                            StartPosition = translation.Value,
                            TargetPosition = bucketTranslations[minDistanceIndex].Value,
                            StartTime = startTime,
                            TagComponentToAddOnArrival = tagComponentToAddOnArrival
                        });
                        bucketOwners[minDistanceIndex].SetBucketOwner(teamIndex.Value, true);
                        assignedFetcherEntities[minDistanceIndex] = entity;
                        ecb.SetComponent<Bucket>(bucketEntities[minDistanceIndex], new Bucket {LinearT = 0.0f});
                    }
                }

                float GetSquaredDistance(Translation position1, Translation position2)
                {
                    return (position2.Value.x - position1.Value.x) * (position2.Value.x - position1.Value.x) +
                           (position2.Value.y - position1.Value.y) * (position2.Value.y - position1.Value.y) +
                           (position2.Value.z - position1.Value.z) * (position2.Value.z - position1.Value.z);
                }
            })
            .Schedule();

        Entities
            .WithAll<Bucket>()
            .WithStoreEntityQueryInField(ref m_BucketQuery)
            .WithReadOnly(bucketOwners)
            .WithDisposeOnCompletion(bucketOwners)
            .ForEach((Entity entity, int entityInQueryIndex, ref BucketOwner bucketOwner, in Translation translation) =>
            {
                bucketOwner.Value = bucketOwners[entityInQueryIndex].Value;
            })
            .Schedule();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
