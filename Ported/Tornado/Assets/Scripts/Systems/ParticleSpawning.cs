using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class ParticleSpawning : SystemBase
{

    protected override void OnUpdate()
    {
        Random random = new Random(1234);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, in ParticleSpawner particleSpawner) =>
        {
            for (int i = 0; i < particleSpawner.ParticleCount; i++)
            {
                var instance = ecb.Instantiate(particleSpawner.ParticlePrefab);
                var translation = new Translation { Value = new float3(random.NextFloat(-50f, 50f), random.NextFloat(0, 50f), random.NextFloat(-50f, 50f)) };
                var scale = new Scale { Value = (random.NextFloat(0.1f, 0.7f)) };
                var particle = new Particle { radiusMult = random.NextFloat(0f, 1f)};

                ecb.AddComponent(instance, typeof(Particle));
                ecb.SetComponent(instance, translation);
                ecb.AddComponent(instance, scale);
            }
            ecb.DestroyEntity(entity);
        }).Run();

        ecb.Playback(EntityManager);


    }
}
