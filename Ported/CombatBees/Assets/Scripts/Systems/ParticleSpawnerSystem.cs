using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ParticleSpawnerSystem : SystemBase
{
    private uint Seed;

    protected override void OnCreate()
    {
        Seed = (uint)System.DateTime.Now.Ticks;
    }
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random(Seed);

        Entities
            .ForEach((Entity entity, in ParticleSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.Count; ++i)
                {
                    var instance = ecb.Instantiate(spawner.Prefab);

                    var particleComponent = new Particle
                    {
                        LifeRemaining = spawner.Lifetime,
                    };
                    ecb.AddComponent(instance, particleComponent);
                    
                    var translationComponent = new Translation { Value = spawner.Position };
                    ecb.SetComponent(instance, translationComponent);

                    // Use Direction + Spread to calculate rotation (forward +x)
                    quaternion rotation = quaternion.identity;
                    var rotationComponent = new Rotation { Value = rotation };
                    ecb.SetComponent(instance, rotationComponent);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}