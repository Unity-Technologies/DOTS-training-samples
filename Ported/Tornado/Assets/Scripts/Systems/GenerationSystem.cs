using Components;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;


namespace Systems
{
    public partial class GenerationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // run once on spawner entity & delete it (maybe?)
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
            // and can be used in jobs. For simplicity and debuggability in development,
            // we'll initialize it with a constant. (In release, we'd want a seed that
            // randomly varies, such as the time from the user's system clock.)
            var random = new Random(1234);

            Entities
                .ForEach((Entity entity, in GenerationParameters spawner) =>
                {
                    ecb.DestroyEntity(entity);

                    for (int i = 0; i < spawner.particleCount; ++i)
                    {
                        var instance = ecb.Instantiate(spawner.particlePrefab);
                        var particlePosition = random.NextFloat3(
                            spawner.minParticleSpawnPosition,
                            spawner.maxParticleSpawnPosition);
                        var particleScale = random.NextFloat(spawner.minParticleScale, spawner.maxParticleScale);

                        ecb.AddComponent(instance, new Particle { radiusMult = random.NextFloat()});
                        ecb.AddComponent(instance,
                            new Color {color = new float4(1.0f, 1.0f, 1.0f, 1.0f) * random.NextFloat()});
                        ecb.SetComponent(instance, new Translation { Value = particlePosition });
                        ecb.AddComponent(instance, new Scale { Value = particleScale });
                    }
                }).WithoutBurst().Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}