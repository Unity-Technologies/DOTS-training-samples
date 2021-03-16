using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities
            .ForEach((ref Translation translation, ref Force force, ref Velocity velocity) =>
            {
                velocity.Value = velocity.Value * 0.8f + force.Value;
                force.Value = float3.zero;

                translation.Value += velocity.Value * deltaTime;
            }).Schedule();
    }
}
