using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CreateBucketSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var prefabs = GetSingleton<GlobalPrefabs>();
        var ecb = m_ECBSystem.CreateCommandBuffer();
        Entities
            .WithAll<BrigadeLineEstablished>()
            .ForEach((Entity e, in BrigadeLine line, in ResourceSourcePosition source, in DynamicBuffer<WorkerEntityElementData> workers) =>
            {
                var resource = GetComponent<ResourceAmount>(source.Id);
                var spawnTime = GetComponent<ResourceNextSpawnTime>(source.Id);
                if (time > spawnTime.Value)
                {
                    if (resource.Value > 0)
                    {
                        var resourcePos = GetComponent<Translation>(source.Id).Value;
                        var workerPos = GetComponent<Translation>(workers[0].Value).Value;
                        if (math.distancesq(resourcePos, workerPos) < 1)
                        {
                            ecb.SetComponent(source.Id, new ResourceAmount() { Value = resource.Value - 1 });
                            ecb.SetComponent(source.Id, new ResourceNextSpawnTime() { Value = time + prefabs.BucketSpawnInterval });
                            var bucket = ecb.Instantiate(prefabs.BucketPrefab);
                            ecb.AddComponent(bucket, new BucketWorkerRef() { WorkerRef = workers[0].Value });
                            ecb.AddComponent(workers[0].Value, new BucketRef() { Bucket = bucket });
                            ecb.AddComponent(source.Id, new Scale() { Value = (resource.Value / 255f) * 10 });
                        }
                    }
                    else
                    {
                        if (HasComponent<ResourceSourcePosition>(e))
                        {
                            var resourceId = GetComponent<ResourceSourcePosition>(e).Id;
                            ecb.RemoveComponent<ResourceClaimed>(resourceId);
                        }
                        ecb.RemoveComponent<ResourceSourcePosition>(e);
                        ecb.RemoveComponent<ResourceTargetPosition>(e);
                    }
                }
            }).Schedule();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
