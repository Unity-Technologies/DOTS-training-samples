using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class PickupBucketLineSystem : SystemBase
{
    private EntityQuery bucketQuery;
    private EndSimulationEntityCommandBufferSystem sys;

    protected override void OnCreate()
    {
        bucketQuery = GetEntityQuery(
            typeof(Bucket),
            typeof(Volume),
            typeof(Translation));
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = sys.CreateCommandBuffer();
        
        Dependency = Entities
            .WithAny<EmptyBucketer, FullBucketer>()
            .WithNone<CarryingBucket>()
            .ForEach((Entity entity, ref TargetPosition targetPosition, ref BucketID bucketId, in PassedBucketId passedBucketId , in Translation position, in Radius radius) =>
            {
                if (bucketId.Value != Entity.Null && passedBucketId.Value != bucketId.Value)
                {
                    ecb.AddComponent<CarryingBucket>(entity);
                    ecb.SetComponent<Translation>(bucketId.Value, position);
                }
            }).Schedule(Dependency);
        sys.AddJobHandleForProducer(Dependency);
    }
}