using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(WallSpawnerSystem))]
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

                    ecb.SetComponent(instance, new Translation
                    {
                        Value = new float3(randx, 0, randz)
                    });

                    float randomAngle = random.NextFloat(0,1) * Mathf.PI * 2f;
                    var rotation = new Rotation { Value = quaternion.Euler(0,randomAngle, 0) };

                    ecb.SetComponent(instance, rotation);
                    ecb.SetComponent(instance, new AntMovement()
                    {
                        FacingAngle = randomAngle,
                        AntSpeed = config.MoveSpeed
                    });
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
