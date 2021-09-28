using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public partial class AntSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random(1234);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .ForEach((Entity entity, in AntSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                for (int i = 0; i < spawner.AntsToSpawn; i++)
                {
                    var instance = ecb.Instantiate(spawner.Ant);
                    var translation = new Translation { Value = new float3(0, 0, i) };

                    float randx = random.NextFloat(-spawner.SpawnAreaSize, spawner.SpawnAreaSize);
                    float randz = random.NextFloat(-spawner.SpawnAreaSize, spawner.SpawnAreaSize);

                    ecb.SetComponent(instance, new Translation
                    {
                        Value = new float3(randx, 0, randz)
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
