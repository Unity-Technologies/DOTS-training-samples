using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Dots
{
    public partial class DebrisSimulatorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var elapsedTime = Time.ElapsedTime;

            Entities
                .WithAll<Simulated>()
                .ForEach((ref Translation translation, in Debris debris) =>
                {
                    var tornado = debris.tornado;
                    var tornadoData = GetComponent<Tornado>(tornado);
                    var tornadoTranslation = tornadoData.position;

                    var tornadoPos = new Vector3(tornadoTranslation.x + PointManager.TornadoSway(translation.Value.y, (float)elapsedTime), translation.Value.y, tornadoTranslation.z);
                    var delta = tornadoPos - new Vector3(translation.Value.x, translation.Value.y, translation.Value.z);
                    var dist = delta.magnitude;
                    float inForce = dist - Mathf.Clamp01(tornadoPos.y / tornadoData.height) * tornadoData.maxForceDist * debris.radiusMult + 2f;

                    delta /= dist;
                    translation.Value.x += (-delta.z * tornadoData.spinRate + delta.x * inForce) * deltaTime;
                    translation.Value.y += tornadoData.upwardSpeed * deltaTime;
                    translation.Value.z += (delta.x * tornadoData.spinRate + delta.z * inForce) * deltaTime;
                    if (translation.Value.y > tornadoData.height)
                        translation.Value.y = 0f;

                }).ScheduleParallel();
        }
    }
}