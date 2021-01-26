using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.CarCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.CarPrefab);
                    var translation = new Translation {Value = new float3(0, 0, i)};
                    ecb.SetComponent(instance, translation);

                    for (int j = 0; j < 100; ++j)
                    {
                        if (random.NextFloat() < spawner.CarFrequency)
                        {
                            var vehicle = ecb.Instantiate(spawner.CarPrefab);

                            ecb.SetComponent(vehicle, new Translation
                            {
                                Value = new float3(0, 0, i)
                            });

                            ecb.SetComponent(vehicle, new URPMaterialPropertyBaseColor
                            {
                                Value = random.NextFloat4()
                            });

                            ecb.SetComponent(vehicle, new CarMovement
                            {
                                Offset = j
                            });
                        }
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
