using Unity.Collections;
using Unity.Entities;

public class SimpleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float elapsedTime = Time.fixedDeltaTime;

        Entities
            .ForEach((Entity entity, ref SimpleIntersection simpleIntersection) =>
            {
                if (simpleIntersection.car == Entity.Null)
                {
                    Lane lane = GetComponent<Lane>(simpleIntersection.laneIn0);
                    if (lane.Car != Entity.Null)
                    {
                        CarPosition carPosition = GetComponent<CarPosition>(lane.Car);
                    }
                }
                else
                {
                        
                }
                
            }).Run();

        ecb.Playback(EntityManager);
    }
}