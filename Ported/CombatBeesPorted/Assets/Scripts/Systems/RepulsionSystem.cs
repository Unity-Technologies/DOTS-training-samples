using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class RepulsionSystem: SystemBase
{
    private EntityQueryDesc allBeesQueryDesc; 
    protected override void OnCreate()
    {
        allBeesQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Bee)} //, ComponentType.ReadOnly<WorldRenderBounds>()
        };

    }

    protected override void OnUpdate()
    {

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        EntityQuery allBeesQuery = GetEntityQuery(allBeesQueryDesc);
        NativeArray<Entity> allBees = allBeesQuery.ToEntityArray(Allocator.Temp);

        Entities
            .WithNone<Attacking>()
            .ForEach((Entity entity, ref Force beeForce, in Bee bee, in Translation translation) =>
            {
                float3 sumForce = float3.zero;
                int count = 0;

                foreach(Entity anotherBee in allBees) {
                    if(entity == anotherBee) continue;
                    if( HasComponent<Attacking>(anotherBee) ) continue;
                    
                    float3 vec = translation.Value - GetComponent<Translation>(anotherBee).Value;
                    float distance = math.length(vec);
                    float amout = 0;

                    amout = 1.0f - (math.clamp(distance, 0.0f, 10.0f) / 10.0f);
                    sumForce +=  math.normalize(vec) * amout;
                    count += 1;
                }

                beeForce.Value += sumForce *3 / count;
            }).Run();
        
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
