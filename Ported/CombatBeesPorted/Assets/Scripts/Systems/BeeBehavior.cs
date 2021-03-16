using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BeeBehavior: SystemBase
{
    protected override void OnUpdate()
    {

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var random = new Unity.Mathematics.Random(1 + (uint)(Time.ElapsedTime*10000));                
        Entities
            .WithNone<GoingForFood,Attacking,BringingFoodBack>()
            .ForEach((in Entity entity, in Bee bee, in Translation translation) =>
            {
                if(random.NextBool()) {
                    // go for food                    
                } else {
                    // go for attack
                }
            }).Run();
        
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
