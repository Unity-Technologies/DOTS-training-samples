using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class ParticleMovementSystem : SystemBase
{


    protected override void OnStartRunning()
    {
        RequireSingletonForUpdate<TornadoSettings>();
    }

    protected override void OnUpdate()
    {
        var settings = this.GetSingleton<TornadoSettings>();
        var spinRate = settings.SpinRate;
        var speedUpward = settings.SpeedUpward;

        float tornadoElapsedTime = (float)Time.ElapsedTime;
        float tornadoDeltaTime = Time.DeltaTime;

        float3 tornadoMovement = new float3(0.0f, 0.0f, 0.0f);

        tornadoMovement.x = (float)math.sin(tornadoElapsedTime) * 10f;
        tornadoMovement.z = (float)math.sin(tornadoElapsedTime) * 10f;

        Entities.ForEach((Entity entity, ref Translation transl, in Particle particle) =>
        {
            float tornadoSway = (float)math.sin(transl.Value.y / 5f + tornadoElapsedTime / 4f) * 3f;
            float3 tornadoPos = new float3(tornadoMovement.x+tornadoSway, transl.Value.y, tornadoMovement.z);
            float3 delta = (tornadoPos - transl.Value);
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - math.clamp(tornadoPos.y / 50f, 0.0f, 1.0f) * 30f * particle.radiusMult + 2f;
            transl.Value += new float3(-delta.z * spinRate + delta.x * inForce, speedUpward, delta.x * spinRate + delta.z * inForce) * tornadoDeltaTime;
            if(transl.Value.y > 50f)
            {
                transl.Value = new float3(transl.Value.x, 0f, transl.Value.z);
            }

        }).ScheduleParallel();

    }

}