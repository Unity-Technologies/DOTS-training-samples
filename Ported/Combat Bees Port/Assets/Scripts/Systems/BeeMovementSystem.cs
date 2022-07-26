using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        var et = (float)Time.ElapsedTime;
        var perlinOffset = 3f;
        var speed = 5f;
        
                                                                        // CHANGE TO BEE ASPECT
        Entities.WithAll<BlueTeam>().ForEach((ref Translation translation, ref Rotation rotation, ref Bee bee, in LocalToWorld transform) =>
            {
                var direction = math.normalizesafe(new float3(30, 0, 0) - translation.Value);
                rotation.Value = quaternion.LookRotationSafe(direction, new float3(0,1,0));
                var position = transform.Position;
                
                bee.position += direction * speed * dt;

                var offset = new float3(
                    noise.cnoise(new float2(et, perlinOffset)) - 0.5f,
                    noise.cnoise(new float2(et, perlinOffset)) - 0.5f,
                    noise.cnoise(new float2(et, perlinOffset)) - 0.5f
                );
                translation.Value = bee.position + offset;

            }).ScheduleParallel();
        
        Entities.WithAll<YellowTeam>().ForEach((ref Translation translation, ref Rotation rotation, ref Bee bee) =>
        {
            var direction = math.normalizesafe(new float3(-30, 0, 0) - translation.Value);
            rotation.Value = quaternion.LookRotationSafe(direction, new float3(0,1,0));
            bee.position += direction * speed * dt;

            var offset = new float3(
                noise.cnoise(new float2(et, perlinOffset)) - 0.5f,
                noise.cnoise(new float2(et, perlinOffset)) - 0.5f,
                noise.cnoise(new float2(et, perlinOffset)) - 0.5f
            );
            translation.Value = bee.position + offset;

        }).ScheduleParallel();
    }
}
