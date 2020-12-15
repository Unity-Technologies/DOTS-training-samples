using UnityEngine;
using Unity.Collections;
using Unity.Entities;

public class FarmerSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, ref FarmerSpawner spawner) =>
            {
                Debug.Log("In Entities.ForEach");
                for (int i = 0; i < spawner.FarmerCounter; ++i)
                {
                    Debug.Log("In Farmer Spawn Loop");
                    var instance = ecb.Instantiate(spawner.FarmerPrefab);
                }

                spawner.FarmerCounter = 0;
            }).Run();
        
        ecb.Playback(EntityManager);
    }
}