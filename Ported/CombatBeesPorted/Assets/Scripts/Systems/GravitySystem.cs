using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class GravitySystem: SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        //var vector = random.NextFloat3Direction();
        var random = new Unity.Mathematics.Random(1 + (uint)(Time.ElapsedTime*10000));
        Entities
            .WithNone<Bee>()
            .ForEach((ref Force force) =>
            {                
                force.Value += new float3{x=0,y=-1,z=0};
            }).Schedule();
    }
}