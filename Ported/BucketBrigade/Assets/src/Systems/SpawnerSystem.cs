using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SpawnerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfig>();
        }

        protected override void OnUpdate()
        {
            var config = GetSingleton<FireSimConfig>();
            
            // Fix configs to remove LinkedEntityGroup:
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.OmniWorkerPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.BucketThrowerWorkerPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.FullBucketPasserWorkerPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.EmptyBucketPasserWorkerPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.BucketPrefab);
            
            // Spawn buckets:
            const float mapSizeXZ = 100;
            const int numBucketsInMap = 30;
            var bucketEntities = new NativeArray<Entity>(numBucketsInMap, Allocator.Temp);
            EntityManager.Instantiate(config.BucketPrefab, bucketEntities);
            for (var i = 0; i < bucketEntities.Length; i++)
            {
                var entity = bucketEntities[i];
                EntityManager.SetComponentData(entity, new Position
                {
                    Value = new float2(UnityEngine.Random.value * mapSizeXZ, UnityEngine.Random.value * mapSizeXZ),
                });  
                EntityManager.SetComponentData(entity, new EcsBucket
                {
                   WaterLevel = UnityEngine.Random.value,
                });
            }
            
            // Once we've spawned, we can disable this system, as it's done its job.
            Enabled = false;
        }
    }
}
