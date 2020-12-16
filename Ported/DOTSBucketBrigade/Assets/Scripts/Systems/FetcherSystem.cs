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

        // var fetcherEntities = m_FetcherQuery.ToEntityArray(Allocator.TempJob);

        // Assign buckets to the fetchers
        Entities
            .WithAll<Bucket>()
            .ForEach((Entity entity, ref BucketOwner bucketOwner, in Position position) =>
            {
                float minDistance = float.MaxValue;
                int minDistanceIndex = -1;
                for (var i=0; i<fetcherPositions.Length; i++)
                {
                    var newMinDistance = GetSquaredDistance(fetcherPositions[i], position);
                    if (newMinDistance < minDistance)
                    {
                        minDistance = newMinDistance;
                        minDistanceIndex = i;
                    }
                }

                if (minDistanceIndex != -1)
                {
                    bucketOwner.SetBucketOwner(fetcherTeams[minDistanceIndex].Value, true);
                }

                float GetSquaredDistance(Position position1, Position position2)
                {
                    return (position2.coord.x - position1.coord.x) * (position2.coord.x - position1.coord.x) +
                           (position2.coord.y - position1.coord.y) * (position2.coord.y - position1.coord.y);
                }
            })
            .Schedule();

        Entities
            .WithAll<Fetcher>()
            .WithStoreEntityQueryInField(ref m_FetcherQuery)
            .WithDisposeOnCompletion(fetcherPositions)
            .WithDisposeOnCompletion(fetcherTranslations)
            // .WithDisposeOnCompletion(fetcherTeams)
            .ForEach((int entityInQueryIndex, ref Position position, ref Translation translation) =>
            {
                position = fetcherPositions[entityInQueryIndex];
                translation = fetcherTranslations[entityInQueryIndex];
            })
            .Schedule();
    }
}
