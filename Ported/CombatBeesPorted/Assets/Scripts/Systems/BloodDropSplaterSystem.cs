using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BloodDropSplaterSystem: SystemBase
{
    EndSimulationEntityCommandBufferSystem endSim;    

    protected override void OnCreate()
    {
        endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        
        var random = new Unity.Mathematics.Random((uint)(Time.ElapsedTime * 10000)+1);
        
        var commandBuffer = endSim.CreateCommandBuffer();
        
        
        var gameConfig = GetSingletonEntity<GameConfiguration>();
        var worldBound = GetComponent<WorldRenderBounds>(gameConfig);
        
        Entities
            .WithAll<BloodDroplet>()
            .ForEach((Entity entity, in Translation translation, in WorldRenderBounds entityBounds) =>
            {

                var bounds = worldBound.Value;

                var entitySize = entityBounds.Value.Size;
                var max = math.max(math.max(entitySize.x, entitySize.y), entitySize.z);
                
                bounds.Extents -= new float3(0.1f, 0.1f, 0.1f) + max/2f;

                if (!bounds.Contains(translation.Value))
                {
                    var normal = translation.Value / bounds.Extents;
                    var outsideNormal = new float3(new int3(normal));

                    if (outsideNormal.y != 0)
                    {
                        commandBuffer.RemoveComponent<Force>(entity);
                        commandBuffer.RemoveComponent<Velocity>(entity);
                    }
                    else
                    {
                        commandBuffer.SetComponent(entity, new Velocity() { Value = float3.zero } );
                        commandBuffer.SetComponent(entity, new Force() { Value = float3.zero } );
                    }

                    commandBuffer.RemoveComponent<BloodDroplet>(entity);
                    commandBuffer.RemoveComponent<InitialScale>(entity);
                    
                    commandBuffer.RemoveComponent<Rotation>(entity);
                    commandBuffer.SetComponent<NonUniformScale>(entity, new NonUniformScale(){ Value = new float3(0.2f) + (new float3(1, 1, 1) - math.abs(outsideNormal)) * random.NextFloat(0.0f, 4f)});
                    commandBuffer.AddComponent<ShrinkAndDestroy>(entity, new ShrinkAndDestroy() { lifetime = 1f, age = -2f });
                }
            }).Schedule();
        
        endSim.AddJobHandleForProducer(Dependency);
    }

    private float3 DirectionalScale(float3 position)
    {
        return float3.zero;
    }
}
