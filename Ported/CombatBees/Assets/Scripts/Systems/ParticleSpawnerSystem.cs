using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

#if COMMENT
public class ParticleSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random(1234789);

        /*
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Particle_Spawner")
            .ForEach((Entity spawnerEntity, ref ParticleSpawner particleSpawner, in Translation pos) =>
            {
                for(int i = 0; i < particleSpawner.count; i++)
                {
                    Entity particle;
                    float3 velocity;
                    float lifeDuration;
                    float3 size;
                    float4 baseColor;

                    if (particleSpawner.type == ParticleType.Type.Blood)
                    {
                        particle = ecb.Instantiate(particleSpawner.bloodPrefab);

                        velocity = particleSpawner.velocity + random.NextFloat3() * particleSpawner.velocityJitter;
                        ecb.AddComponent(particle, new Velocity { vel = velocity });

                        lifeDuration = random.NextFloat(3f, 5f);
                        ecb.AddComponent(particle, new LifeDuration { vel = lifeDuration });

                        size = new float3(random.NextFloat(.1f, .2f), 
                                            random.NextFloat(.1f, .2f), 
                                            random.NextFloat(.1f, .2f));
                        ecb.AddComponent(particle, new NonUniformScale { Value = size });

                        Color color = UnityEngine.Random.ColorHSV(-.05f, .05f, .75f, 1f, .3f, .8f);
                        baseColor.x = color.r;
                        baseColor.y = color.g;
                        baseColor.z = color.b;
                        baseColor.w = color.a;
                        ecb.AddComponent(particle, new URPMaterialPropertyBaseColor { Value = baseColor });
                    }
                    else
                    {
                        particle = ecb.Instantiate(particleSpawner.flashPrefab);

                        velocity = random.NextFloat3() * 5f;
                        ecb.AddComponent(particle, new Velocity { vel = velocity });

                        lifeDuration = random.NextFloat(.25f, .5f);
                        ecb.AddComponent(particle, new LifeDuration { vel = lifeDuration });

                        size = new float3(random.NextFloat(1f, 2f),
                                            random.NextFloat(1f, 2f),
                                            random.NextFloat(1f, 2f));
                        ecb.AddComponent(particle, new NonUniformScale { Value = size });

                        baseColor = new float4(1f, 1f, 1f, 1f);
                        ecb.AddComponent(particle, new URPMaterialPropertyBaseColor { Value = baseColor });
                    }

                    ecb.SetComponent<Translation>(particle, new Translation { Value = pos.Value });
                }

            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        */

    }

}
#endif

