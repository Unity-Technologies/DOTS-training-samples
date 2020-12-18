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
            .ForEach((Entity entity, ref Position position, ref LocalToWorld localToWorld) =>
            {
                localToWorld.Value.c3 = new float4(position.coord.x, 0.9f, position.coord.y, localToWorld.Value.c3.w);
            })
            .Schedule();

        var bucketLocalToWorlds = m_BucketQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
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
            .WithReadOnly(bucketLocalToWorlds)
            .WithDisposeOnCompletion(bucketLocalToWorlds)
            .WithDisposeOnCompletion(bucketEntities)
            .WithDisposeOnCompletion(assignedFetcherEntities)
            .ForEach((Entity entity, in LocalToWorld localToWorld, in TeamIndex teamIndex) =>
            {
                float minDistance = float.MaxValue;
                int minDistanceIndex = -1;
                for (var i=0; i<bucketLocalToWorlds.Length; i++)
                {
                    if (assignedFetcherEntities[i] == Entity.Null)
                    {
                        var newMinDistance = GetSquaredDistance(bucketLocalToWorlds[i].Value.c3.xyz, localToWorld.Value.c3.xyz);
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
                            StartPosition = localToWorld.Value.c3.xyz,
                            TargetPosition = bucketLocalToWorlds[minDistanceIndex].Value.c3.xyz,
                            StartTime = startTime,
                            HasTagComponentToAddOnArrival = true,
                            TagComponentToAddOnArrival = tagComponentToAddOnArrival
                        });
                        bucketOwners[minDistanceIndex].SetBucketOwner(teamIndex.Value, true);
                        assignedFetcherEntities[minDistanceIndex] = entity;
                        ecb.SetComponent<Bucket>(bucketEntities[minDistanceIndex], new Bucket {LinearT = 0.0f});
                    }
                }

                float GetSquaredDistance(float3 position1, float3 position2)
                {
                    return (position2.x - position1.x) * (position2.x - position1.x) +
                           (position2.y - position1.y) * (position2.y - position1.y) +
                           (position2.z - position1.z) * (position2.z - position1.z);
                }
            })
            .Schedule();

        Entities
            .WithAll<Bucket>()
            .WithStoreEntityQueryInField(ref m_BucketQuery)
            .WithReadOnly(bucketOwners)
            .WithDisposeOnCompletion(bucketOwners)
            .ForEach((Entity entity, int entityInQueryIndex, ref BucketOwner bucketOwner, in LocalToWorld localToWorld) =>
            {
                bucketOwner.Value = bucketOwners[entityInQueryIndex].Value;
            })
            .Schedule();

        m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
