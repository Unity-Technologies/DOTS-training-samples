using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CreateBrigadeLineSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.WithoutBurst().WithAll<Brigade>().ForEach((Entity e, in DynamicBuffer<ActorElement> actors, in LineComponent line) =>
        {
            float3 positionStart = EntityManager.GetComponentData<Translation>(e).Value;
            float3 positionEnd = EntityManager.GetComponentData<Translation>(e).Value;
            
            for (int i = 0; i < actors.Length; i++)
            {
                float newX = math.lerp(positionStart.x, positionEnd.x, i);
                float newZ = math.lerp(positionStart.z, positionEnd.z, i);
                
                float3 initPosition = EntityManager.GetComponentData<Translation>(actors[i].actor).Value;
                ecb.SetComponent(actors[i].actor, new Destination(){position = new float3(newX, initPosition.y, newZ)});
            }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

