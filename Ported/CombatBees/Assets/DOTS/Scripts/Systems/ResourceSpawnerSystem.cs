using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SpawnGroup))]
[UpdateBefore(typeof(BeeUpdateGroup))]
public class ResourceSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            var arena = GetSingletonEntity<IsArena>();
            var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

            var random = Utils.GetRandom();
            
            var spawnerEntity = GetSingletonEntity<ResourceSpawner>();
            var resourceSpawner = GetComponent<ResourceSpawner>(spawnerEntity);
            
            var instance = ecb.Instantiate(resourceSpawner.ResourcePrefab);

            ecb.SetComponent(instance, new Translation
            {
                Value = Utils.BoundedRandomPosition(arenaAABB, ref random)
            });
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
