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
            .ForEach((Entity entity, ref TargetPosition targetPosition, ref PlaceInLine placeInLine, ref BucketID bucketId, 
                    ref PassedBucketId passedBucketId, in Translation position, in Radius radius, in NextPerson nextPerson) =>
                {
                    if (nextPerson.Value != Entity.Null)
                    {
                        var nextPersonLoc = GetComponent<Translation>(nextPerson.Value);
                        var distx = math.distance(nextPersonLoc.Value.x, position.Value.x);
                        var distz = math.distance(nextPersonLoc.Value.z, position.Value.z);
                        // If next person is close enough, pass my bucket
                        if ((distx < 0.000001 && distz < 0.000001))
                        {
                            ecb.SetComponent(nextPerson.Value, new BucketID() {Value = bucketId.Value});
                            passedBucketId.Value = bucketId.Value;
                            bucketId.Value = Entity.Null;
                            ecb.RemoveComponent<CarryingBucket>(entity);
                            // return to where I was before I moved to pass bucket
                            ecb.SetComponent(entity, new TargetPosition() {Value = placeInLine.Value});
                        }
                        else
                        {
                            // move closer to person
                            if(nextPersonLoc.Value.x > 0.01 && nextPersonLoc.Value.z > 0.01)
                                targetPosition.Value = nextPersonLoc.Value;
                        }
                    }

                }).Schedule(Dependency);
        sys.AddJobHandleForProducer(Dependency);
    }
}