using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class PassBucketSystem : SystemBase
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
            .WithAll<CarryingBucket>()
            .ForEach((Entity entity, ref TargetPosition targetPosition, ref BucketID bucketId, 
                ref PassedBucketId passedBucketId, in NextPerson nextPerson) =>
            {
                if (nextPerson.Value != Entity.Null)
                {
                    ecb.SetComponent(nextPerson.Value, new BucketID() {Value = bucketId.Value});
                    passedBucketId.Value = bucketId.Value;
                    bucketId.Value = Entity.Null;
                    ecb.RemoveComponent<CarryingBucket>(entity);
                }

            }).Schedule(Dependency);
        sys.AddJobHandleForProducer(Dependency);
    }
}