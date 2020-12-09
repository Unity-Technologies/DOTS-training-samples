using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TornadoMovementSystem))]
public class ParticleMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TornadoSettings>();
        RequireSingletonForUpdate<Tornado>();
    }

    protected override void OnUpdate()
    {
        var settings = GetSingleton<TornadoSettings>();
        var tornado = GetSingletonEntity<Tornado>();
        var tornadoTranslation = GetComponent<Translation>(tornado);

        var spinRate = settings.SpinRate;
        var speedUpward = settings.SpeedUpward;

        float tornadoElapsedTime = (float)Time.ElapsedTime;
        float tornadoDeltaTime = Time.DeltaTime;

        Dependency = Entities.ForEach((Entity entity, ref Translation transl, in Particle particle) =>
        {
            float tornadoSway = math.sin(transl.Value.y / 5f + tornadoElapsedTime / 4f) * 3f;
            float3 tornadoPos = new float3(tornadoTranslation.Value.x + tornadoSway, transl.Value.y, tornadoTranslation.Value.z);
            float3 delta = (tornadoPos - transl.Value);
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - math.clamp(tornadoPos.y / 50f, 0.0f, 1.0f) * 30f * particle.radiusMult + 2f;
            transl.Value += new float3(-delta.z * spinRate + delta.x * inForce, speedUpward, delta.x * spinRate + delta.z * inForce) * tornadoDeltaTime;
            if (transl.Value.y > 50f)
            {
                transl.Value = new float3(transl.Value.x, 0f, transl.Value.z);
            }
        }).ScheduleParallel(Dependency);
    }
}
