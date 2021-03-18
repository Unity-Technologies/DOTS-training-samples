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
    private EntityQuery waterQuery;
    
    protected override void OnCreate()
    {
        waterQuery = GetEntityQuery(
            typeof(River),
            typeof(Volume),
            typeof(Translation));
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = sys.CreateCommandBuffer();
        var waterLocations = waterQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var waterVolume = waterQuery.ToComponentDataArray<Volume>(Allocator.TempJob);

        Dependency =
            Entities
            .WithDisposeOnCompletion(waterLocations)
            .WithDisposeOnCompletion(waterLocations)
            .WithDisposeOnCompletion(waterVolume)
            .WithAll<BucketFetcher, CarryingBucket>()
            .ForEach((Entity entity, ref BucketID bucketId, ref TargetPosition waterLocation, in Translation position) =>
            {
                if (math.distance(waterLocation.Value.x,position.Value.x) < 0.001f && math.distance(waterLocation.Value.z, position.Value.z) < 0.001f)
                {
                    var bucketVol = GetComponent<Volume>(bucketId.Value).Value;
                    // only fill when sure we are on water source
                    if (bucketVol < 1.0f)
                    {
                        int currentWaterSource = waterLocations.Length;
                        for (int i = 0; i < waterLocations.Length; i++)
                        {
                            var distanceToWater = math.distance(position.Value, waterLocations[i].Value);
                            if (distanceToWater< 1.5f)
                            {
                                currentWaterSource = i;
                            }
                        }

                        if(currentWaterSource != waterLocations.Length && waterVolume[currentWaterSource].Value > 0.01f)
                        {
                            ecb.SetComponent(bucketId.Value, new Volume() {Value = bucketVol + 0.01f});
                            // todo: deplete water sourceecb.SetComponent(waterVolume[minWaterLocation]);
                        }
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