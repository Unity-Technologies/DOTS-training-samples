using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TornadoSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TornadoSettings>();
        RequireSingletonForUpdate<Tornado>();
        RequireSingletonForUpdate<CameraReference>();
    }

    protected override void OnUpdate()
    {
        // Update tornado position
        var elapsedTime = (float)Time.ElapsedTime;

        var tornadoPosition = new float3
        {
            x = math.cos(elapsedTime / 6f) * 30f,
            y = 0,
            z = math.sin(elapsedTime / 6f * 1.618f) * 30f
        };

        var tornadoTranslation = new Translation { Value = tornadoPosition };
        var tornadoEntity = GetSingletonEntity<Tornado>();
        SetComponent(tornadoEntity, tornadoTranslation);

        // Update camera position
        var camera = this.GetSingleton<CameraReference>().Camera;
        camera.transform.position = new Vector3(tornadoPosition.x, 10f, tornadoPosition.z) - camera.transform.forward * 60f;

        var settings = GetSingleton<TornadoSettings>();
        var spinRate = settings.SpinRate;
        var speedUpward = settings.SpeedUpward;
        var tornadoDeltaTime = Time.DeltaTime;

        // Update tornado particles
        Dependency = Entities.ForEach((Entity entity, ref Translation translation, in Particle particle) =>
        {
            var tornadoSway = math.sin(translation.Value.y / 5f + elapsedTime / 4f) * 3f;
            var tornadoPos = new float3(tornadoTranslation.Value.x + tornadoSway, translation.Value.y, tornadoTranslation.Value.z);
            var delta = (tornadoPos - translation.Value);
            var dist = math.length(delta);
            delta = math.normalize(delta);
            var inForce = dist - math.clamp(tornadoPos.y / 50f, 0.0f, 1.0f) * 30f * particle.radiusMult + 2f;
            translation.Value += new float3(-delta.z * spinRate + delta.x * inForce, speedUpward, delta.x * spinRate + delta.z * inForce) * tornadoDeltaTime;

            if (translation.Value.y > 50f)
            {
                translation.Value = new float3(translation.Value.x, 0f, translation.Value.z);
            }
        }).ScheduleParallel(Dependency);
    }
}
