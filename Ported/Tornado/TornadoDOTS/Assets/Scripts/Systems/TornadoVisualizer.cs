using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class TornadoVisualizer : SystemBase
{
    const float k_MaxParticleScale = 0.8f;
    protected override void OnUpdate()
    {
        var tornado = GetSingletonEntity<Tornado>();
        var tornadoMovement = GetComponent<TornadoMovement>(tornado);
        var tornadoDimensions = GetComponent<TornadoDimensions>(tornado);
        var tornadoPos = GetComponent<Translation>(tornado);
        var time = (float)Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;
        var floor = GetSingletonEntity<Floor>();
        var terrainBounds = GetComponent<NonUniformScale>(floor).Value;
        tornadoPos.Value.x = math.cos(time * tornadoMovement.XFrequency) * terrainBounds.x*2.5f;
        tornadoPos.Value.z = math.sin(time * tornadoMovement.ZFrequency) * terrainBounds.z*2.5f;
        SetComponent(tornado, tornadoPos);

        Entities.WithAll<TornadoParticle>().
            ForEach((ref Translation particlePos, in TornadoParticle particleProp) =>
            {
                var tornadoPosPerParticle = new float3(
                    tornadoPos.Value.x + (math.sin(particlePos.Value.y/tornadoDimensions.TornadoHeight + time/4f) * 2f),
                    particlePos.Value.y,
                    tornadoPos.Value.z
                );
                var delta = tornadoPosPerParticle - particlePos.Value;
                var dist = math.sqrt( (delta.x*delta.x) + (delta.y*delta.y) + (delta.z * delta.z));

                delta /= dist;
                var inForce = dist - (math.clamp(tornadoPosPerParticle.y / tornadoDimensions.TornadoHeight, 0, 1)) * tornadoDimensions.TornadoRadius * particleProp.RadiusMult;

                var newPos = new float3(
                    (-delta.z * particleProp.SpinRate) + (delta.x * inForce),
                    particleProp.UpwardSpeed,
                    (delta.x * particleProp.SpinRate) + (delta.z * inForce)
                    );
                newPos *= deltaTime;
                newPos += particlePos.Value;
                if (newPos.y>tornadoDimensions.TornadoHeight)
                {
                    newPos = new Vector3(particlePos.Value.x,1f,particlePos.Value.z);
                }
                particlePos.Value = newPos;
            }).ScheduleParallel();
    }
}
