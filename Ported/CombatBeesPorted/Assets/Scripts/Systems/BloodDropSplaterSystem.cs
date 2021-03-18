using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BloodDropSplaterSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Unity.Mathematics.Random(0x123456);
        
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        
        var gameConfig = GetSingletonEntity<GameConfiguration>();
        var worldBound = GetComponent<WorldRenderBounds>(gameConfig);
        
        Entities
            .WithAll<BloodDroplet>()
            .ForEach((Entity entity, in Translation translation) =>
            {

                var bounds = worldBound.Value;
                bounds.Extents -= new float3(0.5f, 0.5f, 0.5f);

                if (!bounds.Contains(translation.Value))
                {
                    var normal = translation.Value / bounds.Extents;
                    var outsideNormal = new float3(new int3(normal));

                    commandBuffer.RemoveComponent<BloodDroplet>(entity);
                    commandBuffer.RemoveComponent<Force>(entity);
                    commandBuffer.RemoveComponent<Velocity>(entity);
                    commandBuffer.RemoveComponent<InitialScale>(entity);
                    
                    commandBuffer.SetComponent<Rotation>(entity, new Rotation());
                    commandBuffer.SetComponent<NonUniformScale>(entity, new NonUniformScale(){ Value = new float3(0.3f) + (new float3(1, 1, 1) - math.abs(outsideNormal))*random.NextFloat(0.5f, 4f)});
                    commandBuffer.AddComponent<ShrinkAndDestroy>(entity, new ShrinkAndDestroy() { lifetime = 1f, age = -2f });
                }
            }).Run();
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }

    private float3 DirectionalScale(float3 position)
    {

        return float3.zero;
    }
}
