
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class SpawnerSystem : SystemBase
{


    protected override void OnUpdate()
    {
        var random = new Random(0x1234567);   
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .WithStructuralChanges()
            .ForEach((Entity entity, in Spawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            for (int i = 0; i < spawner.CubeCount; i++)
            {
                var instance = ecb.Instantiate(spawner.CubePrefab);

                
                float3 pos = new float3(random.NextFloat(-50f,50f),random.NextFloat(0f,50f),random.NextFloat(-50f,50f));
                ecb.SetComponent(instance, new Translation()
                {
                    Value = pos
                });
                
                ecb.SetComponent(instance, new Cube()
                {
                    radius = random.NextFloat(0,1f)
                });
                
                var color = random.NextFloat(.3f, .7f);
                ecb.SetComponent(instance, new URPMaterialPropertyBaseColor()
                {
                    Value =  new float4(color,color,color, 1f) 
                });
                
                
            }
            
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
