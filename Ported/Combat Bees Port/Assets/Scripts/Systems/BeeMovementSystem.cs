using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        var et = (float)Time.ElapsedTime;
        var perlinOffset = 3f;
        var speed = 0.005f;
        Random random = new Random(1234);
        
        //Entities.ForEach((ref Translation translation, ref Rotation rotation, in LocalToWorld transform) =>
                                                                        // CHANGE TO BEE ASPECT
        Entities
            .WithAll<Bee>()
            .ForEach((ref Translation translation, ref Rotation rotation, in LocalToWorld transform) =>
            {
                var direction = math.normalizesafe(
                    new float3(random.NextFloat(-40,40), random.NextFloat(-10,10), random.NextFloat(-10,10)) - translation.Value);
                
                rotation.Value = quaternion.LookRotationSafe(direction, new float3(0,1,0));
               // var position = transform.Position;

                var offset = new float3(
                    noise.cnoise(new float2(et, perlinOffset)) - 0.5f,
                    noise.cnoise(new float2(et, perlinOffset)) - 0.5f,
                    noise.cnoise(new float2(et, perlinOffset)) - 0.5f
                );
                translation.Value += direction + offset * speed;

            }).ScheduleParallel();
    }
}
