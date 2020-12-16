using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class InitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var random = new Random(6541);
        var center = new Translation{ Value = new float3(64, 64, 0) };
        var minRange = new float2(-1,-1);
        var maxRange = new float2(1,1);
        
        Entities
            .ForEach((Entity entity, in Init init) =>
            {
                ecb.DestroyEntity(entity);

                // Create Board
                
                // Create Ants
                for (var i = 0; i < init.antCount; i++)
                {
                    var ant = ecb.Instantiate(init.antPrefab);
                    var translation = new Translation{Value = new float3(64,64, 0)};
                    ecb.SetComponent(ant, translation);
                    
                    ecb.SetComponent(ant, new Heading
                    {
                        heading = math.normalize(random.NextFloat2(minRange, maxRange))
                    });
                }
                
                // Create Home
                
                
                // Create Food
                
                // Create Obstacles
                
            }).Run();

        ecb.Playback(EntityManager);
        
        ecb.Dispose();
    }
}
