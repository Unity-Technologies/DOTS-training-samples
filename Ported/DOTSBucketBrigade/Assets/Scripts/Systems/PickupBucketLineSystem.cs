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
            .WithAll<EmptyBucketer>()
            .WithAll<FullBucketer>()
            .WithNone<CarryingBucket>()
            .ForEach((Entity entity, ref TargetPosition targetPosition, ref BucketID bucketId, in Translation position, in Radius radius) =>
            {
                if (bucketId.Value != Entity.Null)
                {
                    var targetBucketPos = GetComponent<Translation>(bucketId.Value);
                    var distx = math.distance(targetBucketPos.Value.x, position.Value.x);
                    var distz = math.distance(targetBucketPos.Value.z, position.Value.z);
                    if (distx < radius.Value || distz < radius.Value)
                    {
                        ecb.AddComponent<CarryingBucket>(entity);
                    }
                }
            }).Schedule(Dependency);
        sys.AddJobHandleForProducer(Dependency);
    }
}