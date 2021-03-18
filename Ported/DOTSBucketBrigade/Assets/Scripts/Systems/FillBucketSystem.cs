using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class FillBucketSystem : SystemBase
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
            .WithAll<BucketFetcher, CarryingBucket>()
            .ForEach((Entity entity, ref BucketID bucketId, in TargetPosition waterLocation, in Translation position) =>
            {
                if (math.distance(waterLocation.Value.x,position.Value.x) < 0.001f && math.distance(waterLocation.Value.z, position.Value.z) < 0.001f)
                {
                    var bucketVol = GetComponent<Volume>(bucketId.Value).Value;
                    // todo only fill when sure we are on water source
                    if (bucketVol < 1.0f)
                    {
                        ecb.SetComponent(bucketId.Value, new Volume(){ Value = bucketVol + 0.01f});
                    }
                    else
                    {
                        ecb.RemoveComponent<CarryingBucket>(entity);
                        bucketId.Value = Entity.Null;
                        // todo: change bucket y to 0
                    }
                }
            }).Schedule(Dependency);
        sys.AddJobHandleForProducer(Dependency);
    }
}