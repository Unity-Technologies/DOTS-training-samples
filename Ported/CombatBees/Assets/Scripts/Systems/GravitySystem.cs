using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class GravitySystem : SystemBase
{
    private float3 gravity = new float3(0, -1, 0);

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var g = new float3(gravity);
        Entities
            .ForEach((ref Translation translation, ref Velocity velocity, in Gravity gravity) =>
            {
                velocity.Value += g * deltaTime;
              //  translation.Value += new float3(velocity.Value) * deltaTime;
            }).Run();
    }
}