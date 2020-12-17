using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FetcherSystem : SystemBase
{
    private EntityQuery m_FetcherQuery;

    // Update is called once per frame
    protected override void OnUpdate()
    {

        // Assign a bucket to each fetcher
        Entities
            .WithAll<Fetcher>()
            .ForEach((Entity entity, ref Position position, ref Translation translation) =>
            {
                translation.Value = new float3(position.coord.x, 0.9f, position.coord.y);
            })
            .Schedule();

        var fetcherPositions = m_FetcherQuery.ToComponentDataArray<Position>(Allocator.TempJob);
        var fetcherTranslations = m_FetcherQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var fetcherTeams = m_FetcherQuery.ToComponentDataArray<TeamIndex>(Allocator.TempJob);

        // For each fetcher, bucket entity that was assigned to it
        var assignedBucketEntities = new NativeArray<Entity>(FireSimConfig.maxTeams, Allocator.TempJob);

        for (var i=0; i<FireSimConfig.maxTeams; i++)
        {
            assignedBucketEntities[i] = Entity.Null;
        }

        // Assign buckets to the fetchers
        Entities
            .WithAll<Bucket>()
            .WithDisposeOnCompletion(fetcherTeams)
            .ForEach((Entity entity, ref BucketOwner bucketOwner, in Position position) =>
            {
                if (bucketOwner.IsAssigned())
                {
                    return;
                }
                float minDistance = float.MaxValue;
                int minDistanceIndex = -1;
                for (var i=0; i<fetcherPositions.Length; i++)
                {
                    if (assignedBucketEntities[i] == Entity.Null)
                    {
                        var newMinDistance = GetSquaredDistance(fetcherPositions[i], position);
                        if (newMinDistance < minDistance)
                        {
                            minDistance = newMinDistance;
                            minDistanceIndex = i;
                        }
                    }
                }

                if (minDistanceIndex != -1)
                {
                    if (assignedBucketEntities[minDistanceIndex] == Entity.Null)
                    {
                        bucketOwner.SetBucketOwner(fetcherTeams[minDistanceIndex].Value, true);
                        assignedBucketEntities[minDistanceIndex] = entity;
                    }
                }

                float GetSquaredDistance(Position position1, Position position2)
                {
                    return (position2.coord.x - position1.coord.x) * (position2.coord.x - position1.coord.x) +
                           (position2.coord.y - position1.coord.y) * (position2.coord.y - position1.coord.y);
                }
            })
            .Schedule();

        Entities
            .WithAll<Fetcher, TeamIndex>()
            .WithStoreEntityQueryInField(ref m_FetcherQuery)
            .WithReadOnly(fetcherPositions)
            .WithReadOnly(fetcherTranslations)
            .WithReadOnly(assignedBucketEntities)
            .WithDisposeOnCompletion(fetcherPositions)
            .WithDisposeOnCompletion(fetcherTranslations)
            .WithDisposeOnCompletion(assignedBucketEntities)
            .ForEach((int entityInQueryIndex, ref Position position, ref Translation translation,
                ref Entity assignedBucket) =>
            {
                position = fetcherPositions[entityInQueryIndex];
                translation = fetcherTranslations[entityInQueryIndex];
                assignedBucket = assignedBucketEntities[entityInQueryIndex];
            })
            .Schedule();
    }
}
