using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

/*
public struct ParticleSpawner : IComponentData
{
    //public Entity bloodPrefab;
    //public Entity flashPrefab;
    public int count;
    public ParticleType.Type type;
    public float3 velocity;
    public float velocityJitter;
}
*/

public class ParticleSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var particleParams = GetSingleton<ParticleParams>();
        var random = new Unity.Mathematics.Random(1234789);

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
                    float size;
                    float4 baseColor;

                    if (particleSpawner.type == ParticleType.Type.Blood)
                    {
                        particle = ecb.Instantiate(particleParams.bloodPrefab);

                        velocity = particleSpawner.velocity + random.NextFloat3() * particleSpawner.velocityJitter;
                        ecb.AddComponent(particle, new Velocity { vel = velocity });

                        lifeDuration = random.NextFloat(3f, 5f);
                        ecb.AddComponent(particle, new LifeDuration { vel = lifeDuration });

                        size = random.NextFloat(particleParams.minBloodSize, particleParams.minBloodSize);
                        ecb.AddComponent(particle, new NonUniformScale { Value = new float3(size, size, size) });

                        Color color = UnityEngine.Random.ColorHSV(-.05f, .05f, .75f, 1f, .3f, .8f);
                        baseColor.x = color.r;
                        baseColor.y = color.g;
                        baseColor.z = color.b;
                        baseColor.w = color.a;
                        ecb.AddComponent(particle, new URPMaterialPropertyBaseColor { Value = baseColor });
                    }
                    else
                    {
                        particle = ecb.Instantiate(particleParams.flashPrefab);

                        velocity = random.NextFloat3() * 5f;
                        ecb.AddComponent(particle, new Velocity { vel = velocity });

                        lifeDuration = random.NextFloat(.25f, .5f);
                        ecb.AddComponent(particle, new LifeDuration { vel = lifeDuration });

                        size = random.NextFloat(particleParams.minFlashSize, particleParams.maxFlashSize);
                        ecb.AddComponent(particle, new NonUniformScale { Value = new float3(size, size, size) });

                        baseColor = new float4(1f, 1f, 1f, 1f);
                        ecb.AddComponent(particle, new URPMaterialPropertyBaseColor { Value = baseColor });
                    }

                    ecb.SetComponent<Translation>(particle, new Translation { Value = pos.Value });
                }

                ecb.DestroyEntity(spawnerEntity);

            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        

    }

}

