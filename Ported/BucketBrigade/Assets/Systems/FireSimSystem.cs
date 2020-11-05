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
    public static EntityQuery emptyBucketQuery;


    protected override void OnCreate()
    {
        emptyBucketQuery = GetEntityQuery(ComponentType.ReadOnly<EmptyBucket>(), ComponentType.ReadOnly<Translation>());
    }

    public static Entity GetClosestEntity(float3 position, NativeArray<Entity> emptyEntities , NativeArray<Translation> translations)
    {
        float distance = float.MaxValue;
        Entity closestEntity = Entity.Null;
        for(int i = 0; i < emptyEntities.Length; i++)
        {
            var entity = emptyEntities[i];
            var translation = translations[i].Value;
            var currentDistance = math.distance(position, translation);

            if (currentDistance < distance)
            {
                distance = currentDistance;
                closestEntity = entity;
            }
        }
        
        return closestEntity;
    }

    protected override void OnUpdate()
    {
    }
}