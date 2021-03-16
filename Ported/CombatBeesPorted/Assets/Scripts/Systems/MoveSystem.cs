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
    protected override void OnUpdate()
    {
        if(skipUpdate) { // TODO remove hack wolrdBound on frame value heve no Value            
            skipUpdate=false;
            return;
        }

        var deltaTime = Time.DeltaTime;
        
        var gameConfig = GetSingletonEntity<GameConfiguration>();
        var worldBound = GetComponent<WorldRenderBounds>(gameConfig);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            //.WithoutBurst() // TODO remove in final version, keep for Debug.Log
            .ForEach((ref Translation translation, ref Force force, ref Velocity velocity) =>
            {
                velocity.Value = velocity.Value * 0.95f + force.Value;
                force.Value = float3.zero;

                translation.Value += velocity.Value * deltaTime;
                
                if (translation.Value.x < worldBound.Value.Min.x && velocity.Value.x < 0 ) velocity.Value.x *= -1;
                if (translation.Value.x > worldBound.Value.Max.x && velocity.Value.x > 0 ) velocity.Value.x *= -1;
                if (translation.Value.y < worldBound.Value.Min.y && velocity.Value.y < 0 ) velocity.Value.y *= -1;
                if (translation.Value.y > worldBound.Value.Max.y && velocity.Value.y > 0 ) velocity.Value.y *= -1;
                if (translation.Value.z < worldBound.Value.Min.z && velocity.Value.z < 0 ) velocity.Value.z *= -1;
                if (translation.Value.z > worldBound.Value.Max.z && velocity.Value.z > 0 ) velocity.Value.z *= -1;

                    //if( worldBound.Value.Contains(translation.Value))
                    
            }).Schedule();
    }
}
