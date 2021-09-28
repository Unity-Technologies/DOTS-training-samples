using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public partial class AntSpawnerSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        var random = new Unity.Mathematics.Random(1234);
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var config = GetSingleton<Config>();
        Entities
            .ForEach((Entity entity, in AntSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                for (int i = 0; i < config.AntCount; i++)
                {
                    var instance = ecb.Instantiate(spawner.Ant);

                    float randx = random.NextFloat(-spawner.SpawnAreaSize, spawner.SpawnAreaSize);
                    float randz = random.NextFloat(-spawner.SpawnAreaSize, spawner.SpawnAreaSize);

                    var rotation = new Rotation { Value = Quaternion.Euler(0.0f, random.NextFloat(0, 360), 0.0f) };

                    ecb.SetComponent(instance, new Translation
                    {
                        Value = new float3(randx, 0, randz)
                    });

                    ecb.SetComponent(instance, rotation);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    protected override void OnUpdate()
    {
        //throw new System.NotImplementedException();
    }
}
