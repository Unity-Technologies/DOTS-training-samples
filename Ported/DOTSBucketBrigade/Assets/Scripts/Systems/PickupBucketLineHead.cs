using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class PickupBucketLineHead : SystemBase
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
        var bucketPositions = bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketVolumes = bucketQuery.ToComponentDataArray<Volume>(Allocator.TempJob);
        var bucketIDs = bucketQuery.ToEntityArray(Allocator.TempJob);
        
        Dependency = Entities
            .WithAll<Line>()
            .WithDisposeOnCompletion(bucketIDs)
            .WithDisposeOnCompletion(bucketVolumes)
            .WithDisposeOnCompletion(bucketPositions)
            .ForEach((Entity entity, in Line line) =>
            {
                var lastPassedBucket = GetComponent<PassedBucketId>(line.FullHead);
                var bucketId = GetComponent<BucketID>(line.FullHead);
                if (!HasComponent<CarryingBucket>(line.FullHead))
                {
                    Translation position = GetComponent<Translation>(line.FullHead);
                    Radius radius = GetComponent<Radius>(line.FullHead);
                    for (int i = 0; i < bucketPositions.Length; i++)
                    {
                        if (bucketVolumes[i].Value >= 1.0f && bucketIDs[i] != lastPassedBucket.Value)
                        {
                            if (math.distance(bucketPositions[i].Value, position.Value) <= 10)//radius.Value)
                            {
                                ecb.AddComponent<CarryingBucket>(line.FullHead);
                                ecb.SetComponent(line.FullHead, new BucketID() {Value = bucketIDs[i]});
                                ecb.SetComponent(line.FullHead, new PassedBucketId() {Value = bucketIDs[i]});
                                break;
                            }
                        }
                    }
                }
            }).Schedule(Dependency);
        sys.AddJobHandleForProducer(Dependency);
    }
}