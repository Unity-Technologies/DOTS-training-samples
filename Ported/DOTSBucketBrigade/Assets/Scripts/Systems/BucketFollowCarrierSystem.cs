using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

//[UpdateAfter(MoveToSystem)]
public class BucketFollowCarrierSystem: SystemBase
{
    private EndSimulationEntityCommandBufferSystem sys;
    
    protected override void OnCreate()
    {
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = sys.CreateCommandBuffer();
        
        Dependency = Entities
            .WithAll<CarryingBucket>()
            .ForEach((Entity entity, ref BucketID bucketId, ref Translation position) =>
            {
                if (bucketId.Value != Entity.Null)
                {
                    var bucketPos = new float3(position.Value);
                    bucketPos.z += 2.0f;
                    ecb.SetComponent(bucketId.Value, new Translation() {Value = position.Value});
                }
            }).Schedule(Dependency);
        sys.AddJobHandleForProducer(Dependency);
    }
}