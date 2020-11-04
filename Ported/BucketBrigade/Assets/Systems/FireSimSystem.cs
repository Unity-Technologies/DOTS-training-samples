using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.UIElements;
using UnityEngine;

public struct GetClosestBucket : IJob
{
    public Translation ScooperPosition;
    public Translation BucketPosition;

    public void Execute()
    {
    }
}

public class FireSimSystem : SystemBase
{
    static EntityQuery emptyBucketQuery;

    protected override void OnCreate()
    {
        emptyBucketQuery = GetEntityQuery(ComponentType.ReadOnly<EmptyBucket>(), ComponentType.ReadOnly<Translation>());
    }

    public static Entity GetClosestBucket(float3 position, EntityManager em)
    {
        var emptyBuckets = emptyBucketQuery.ToEntityArray(Allocator.Temp);
        //var bucketPositions = emptyBucketQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        float3 closestPosition = new float3();
        float distance = float.MaxValue;
        Entity closestBucket = Entity.Null;
        
        foreach (var bucket in emptyBuckets)
        {
            var translation = em.GetComponentData<Translation>(bucket);
            var bucketPosition = translation.Value;
            var currentDistance = math.distance(closestPosition, bucketPosition);

            if (currentDistance < distance)
            {
                distance = currentDistance;
                closestPosition = bucketPosition;
                closestBucket = bucket;
            }
        }

        return closestBucket;
    }

    protected override void OnUpdate()
    {
    }
}