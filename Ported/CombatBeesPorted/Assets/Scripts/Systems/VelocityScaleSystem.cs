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
        
        Entities
            .WithAll<VelocityScale, Bee>()
            .ForEach((Entity entity, ref NonUniformScale nonUniformScale, in Velocity velocity, in InitialScale initialScale) =>
            {
                float yScale = math.lerp(initialScale.Value.y, initialScale.Value.y * 4f, math.clamp((math.length(velocity.Value) / 30f) - 0.5f,0,1)); // Expose these numbers?
                nonUniformScale.Value = new float3( initialScale.Value.x, yScale, initialScale.Value.y);
            }).Schedule();
        
        Entities
            .WithAll<VelocityScale, BloodDroplet>()
            .ForEach((Entity entity, ref NonUniformScale nonUniformScale, in Velocity velocity, in InitialScale initialScale) =>
            {
                float yScale = math.lerp(initialScale.Value.y, initialScale.Value.y * 4f, math.clamp((math.length(velocity.Value) / 10f) - 0.5f,0,1)); // Expose these numbers?
                nonUniformScale.Value = new float3( initialScale.Value.x, yScale, initialScale.Value.y);
            }).Schedule();
    }
    
}