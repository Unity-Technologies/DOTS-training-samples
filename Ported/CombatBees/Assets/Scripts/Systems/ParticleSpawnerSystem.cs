using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial class ParticleSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

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

                    // Use spawner.Direction + spawner.Spread to calculate rotation (forward +x)
                    quaternion rotation = quaternion.identity;
                    var rotationComponent = new Rotation { Value = rotation };
                    ecb.SetComponent(instance, rotationComponent);
                    
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(instance);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}