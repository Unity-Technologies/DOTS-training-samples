using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
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
            var outputEntities = new NativeArray<Entity>(1000, Allocator.TempJob);

            var start = UnityEngine.Time.realtimeSinceStartupAsDouble;
            EntityManager.Instantiate(config.BucketThrowerWorkerPrefab, outputEntities);
            var elapsed = UnityEngine.Time.realtimeSinceStartupAsDouble - start;
            
            Enabled = false;

            //outputEntities.Dispose();

            Debug.LogError($"SPAWNED 1k ENTITIES! {elapsed*1000:0.000}ms");
        }
    }
}
