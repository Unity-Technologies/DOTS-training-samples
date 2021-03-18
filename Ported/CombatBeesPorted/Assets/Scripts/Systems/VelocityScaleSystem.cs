using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class VelocityScaleSystem: SystemBase
{
    
    protected override void OnUpdate()
    {
        // var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithAll<VelocityScale>()
            .ForEach((Entity entity, ref NonUniformScale nonUniformScale, in Velocity velocity, in InitialScale initialScale) =>
            {
                float yScale = math.lerp(initialScale.Value.y, initialScale.Value.y * 4f, math.clamp((math.length(velocity.Value) / 40f) - 0.5f,0,1)); // Expose these numbers?
                nonUniformScale.Value = new float3( initialScale.Value.x, yScale, initialScale.Value.y);
            }).Run();
        
        // commandBuffer.Playback(EntityManager);
        // commandBuffer.Dispose();
    }
    
}