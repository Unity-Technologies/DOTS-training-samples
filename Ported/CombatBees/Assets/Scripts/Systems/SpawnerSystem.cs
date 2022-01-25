using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var reinitiate = Input.GetKeyDown(KeyCode.R);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                if (reinitiate)
                {
                    // TODO: Ask why this doesn't work with Burst, but the below does
                    //       - why can't we pass ecb to a method unless we use WithoutBurst()
                    // BufferEntityInstantiation(spawner.BeePrefab, new float3(-1, 0, -1), in ecb);
                    // BufferEntityInstantiation(spawner.BloodPrefab, new float3(0, 0, -1), in ecb);
                    // BufferEntityInstantiation(spawner.ResourcePrefab, new float3(1, 0, -1), in ecb);
                    
                    var instance = ecb.Instantiate(spawner.BeePrefab);
                    var translation = new Translation {Value = new float3(-1, 0, -1)};
                    ecb.SetComponent(instance, translation);
                    
                    instance = ecb.Instantiate(spawner.BloodPrefab);
                    translation = new Translation {Value = new float3(0, 0, -1)};
                    ecb.SetComponent(instance, translation);
                    
                    instance = ecb.Instantiate(spawner.ResourcePrefab);
                    translation = new Translation {Value = new float3(1, 0, -1)};
                    ecb.SetComponent(instance, translation);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    private void BufferEntityInstantiation(
        Entity prefabEntity,
        float3 position,
        in EntityCommandBuffer ecb)
    {
        var instance = ecb.Instantiate(prefabEntity);
        var translation = new Translation {Value = position};
        ecb.SetComponent(instance, translation);
    }
}