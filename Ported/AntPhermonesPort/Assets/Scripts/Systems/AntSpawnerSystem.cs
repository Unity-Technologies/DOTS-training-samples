using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial class AntSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((Entity entity, in AntSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                for (int i = 0; i < spawner.AntsToSpawn; i++)
                {
                    var instance = ecb.Instantiate(spawner.Ant);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
