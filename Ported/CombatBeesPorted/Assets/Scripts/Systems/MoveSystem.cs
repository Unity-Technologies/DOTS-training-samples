using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem: SystemBase
{
    bool skipUpdate = true;
    EndSimulationEntityCommandBufferSystem endSim;    

    protected override void OnCreate()
    {
        base.OnCreate();
        endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        if(skipUpdate) { // TODO remove hack wolrdBound on frame value heve no Value            
            skipUpdate=false;
            return;
        }

        var deltaTime = Time.DeltaTime;
        
        var gameConfig = GetSingletonEntity<GameConfiguration>();
        var worldBound = GetComponent<WorldRenderBounds>(gameConfig);

        
        var commandBuffer = endSim.CreateCommandBuffer();

        Entities
            //.WithoutBurst() // TODO remove in final version, keep for Debug.Log
            .ForEach((Entity entity, ref Translation translation, ref Force force, ref Velocity velocity, in WorldRenderBounds entityBounds) =>
            {
                velocity.Value = velocity.Value * 0.99f + force.Value*0.17f;
                force.Value = float3.zero;

                translation.Value += velocity.Value * deltaTime;
                
                float3 entitySize = entityBounds.Value.Size * 0.5f;
                
                if (translation.Value.x - entitySize.x < worldBound.Value.Min.x && velocity.Value.x < 0 ) {
                    translation.Value.x = worldBound.Value.Min.x + entitySize.x;
                    velocity.Value.x *= -1;
                }
                if (translation.Value.x + entitySize.x > worldBound.Value.Max.x && velocity.Value.x > 0 ) {
                    translation.Value.x = worldBound.Value.Max.x - entitySize.x;
                    velocity.Value.x *= -1;
                }
                if (translation.Value.y - entitySize.y < worldBound.Value.Min.y && velocity.Value.y < 0 ) {
                    // ground
                    translation.Value.y = worldBound.Value.Min.y + entitySize.y;
                    if (math.abs(velocity.Value.y) < 1)
                    {
                        velocity.Value.y = 0;
                        velocity.Value.x *= 0.9f;
                        velocity.Value.z *= 0.9f;
                    }
                    else
                    {
                        velocity.Value.y *= -0.7f;
                        velocity.Value.x *= 0.9f;
                        velocity.Value.z *= 0.9f;
                    }
                    if(math.lengthsq(velocity.Value)<0.1f)
                        velocity.Value=float3.zero;
                    if(!HasComponent<Grounded>(entity)){ commandBuffer.AddComponent<Grounded>(entity,new Grounded());}
                } else {
                    // in air
                    if(HasComponent<Grounded>(entity)) commandBuffer.RemoveComponent<Grounded>(entity);
                }
                if (translation.Value.y + entitySize.x > worldBound.Value.Max.y && velocity.Value.y > 0 ) {
                    translation.Value.y = worldBound.Value.Max.y - entitySize.y;
                    velocity.Value.y *= -1;                    
                }
                if (translation.Value.z - entitySize.z < worldBound.Value.Min.z && velocity.Value.z < 0 ) {
                    translation.Value.z = worldBound.Value.Min.z + entitySize.z;
                    velocity.Value.z *= -1;
                }
                if (translation.Value.z + entitySize.z > worldBound.Value.Max.z && velocity.Value.z > 0 ) {
                    translation.Value.z = worldBound.Value.Max.z - entitySize.z;
                    velocity.Value.z *= -1;
                }
                
            }).Schedule();

        endSim.AddJobHandleForProducer(Dependency);   
    }
}
