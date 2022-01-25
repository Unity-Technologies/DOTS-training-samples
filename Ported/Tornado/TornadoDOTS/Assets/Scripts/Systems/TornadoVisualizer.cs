using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class TornadoVisualizer : SystemBase
{
    protected override void OnUpdate()
    {
        var tornado = GetSingletonEntity<Tornado>();
        var tornadoMovement = GetComponent<TornadoMovement>(tornado);
        var tornadoPos = GetComponent<Translation>(tornado);
        var time = (float)Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;
        tornadoPos.Value.x = (float) math.cos(time * tornadoMovement.XFrequency) * tornadoMovement.Amplitude;
        tornadoPos.Value.z = (float) math.sin(time * tornadoMovement.ZFrequency) * tornadoMovement.Amplitude;
        
        SetComponent(tornado, tornadoPos);
        
        Entities.WithAll<TornadoParticle>().
            ForEach((ref Translation particlePos, in TornadoParticle particleProp) =>
            {
                var tornadoPosPerParticle = new float3(
                    tornadoPos.Value.x + (math.sin(particlePos.Value.y/tornadoMovement.MaxHeight + time/4f) * 3f),
                    //tornadoPos.Value.x,
                    particlePos.Value.y,
                    tornadoPos.Value.z
                );
                var delta = tornadoPosPerParticle - particlePos.Value;
                var dist = math.sqrt( (delta.x*delta.x) + (delta.y*delta.y) + (delta.z * delta.z));
                delta /= dist;
                var inForce = dist - (math.clamp(tornadoPosPerParticle.y / tornadoMovement.MaxHeight, 0, 1)) * tornadoMovement.Amplitude * particleProp.RadiusMult;
                
                var newPos = new float3(
                    (-delta.z * particleProp.SpinRate) + (delta.x * inForce),
                    particleProp.UpwardSpeed,
                    (delta.x * particleProp.SpinRate) + (delta.z * inForce)
                    );
                newPos *= deltaTime;
                newPos += particlePos.Value;
                if (newPos.y>tornadoMovement.MaxHeight)
                {
                    newPos = new Vector3(particlePos.Value.x,0f,particlePos.Value.z);
                }
                particlePos.Value = newPos;
            }).ScheduleParallel();
    }
}
