using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Dots
{
    public partial class TornadoSpawner : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var random = new Random(1234);
            
            Entities.WithoutBurst()
                .ForEach((Entity entity, in Translation translation, in DebrisSpawnerData spawner) =>
                {
                    // Destroying the current component is a classic ECS pattern,
                    // when something should only be processed once then forgotten.
                    ecb.RemoveComponent<DebrisSpawnerData>(entity);

                    float4 white = new float4(1f, 1f, 1f, 1f);
                    float3 initialPosition = translation.Value;
                    for (int i = 0; i < spawner.debrisCount; ++i)
                    {
                        var debris = ecb.Instantiate(spawner.debrisPrefab);
                        
                        float3 position = new float3(
                            random.NextFloat(-spawner.initRange, spawner.initRange), 
                            random.NextFloat(0f, spawner.height), 
                            random.NextFloat(-spawner.initRange, spawner.initRange));
                        ecb.SetComponent(debris, new Translation { Value = position });
                        
                        ecb.AddComponent(debris, new DebrisTag { tornado = entity} );
                        
                        float4 color = white * random.NextFloat(.3f, .7f);
                        ecb.AddComponent(debris, new URPMaterialPropertyBaseColor { Value = color });
                        
                        // Make the debris a child of the tornado
                        ecb.AddComponent(debris, new Parent { Value = entity });
                        ecb.AddComponent(debris, new LocalToParent { });
                    }
                }).Run();
        
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}