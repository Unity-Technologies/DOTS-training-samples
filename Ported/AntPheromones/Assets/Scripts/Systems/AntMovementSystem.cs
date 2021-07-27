using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntMovementSystem : SystemBase
{
    private const int mapSize = 128;
    private const float antSpeed = 0.2f;
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        Entities
            .ForEach((ref Translation translation, ref Speed speed, in Acceleration acceleration, in FacingAngle facingAngle) =>
            {
                var targetSpeed = antSpeed;
                speed.Value = targetSpeed * acceleration.Value;
                float vx = math.cos(facingAngle.Value) * speed.Value;
                float vy = Mathf.Sin(facingAngle.Value) * speed.Value;
                float ovx = vx;
                float ovy = vy;

                if (translation.Value.x + vx < 0f || translation.Value.x + vx > mapSize) {
                    vx = -vx;
                } else {
                    translation.Value.x += vx;
                }
                if (translation.Value.y + vy < 0f || translation.Value.y + vy > mapSize) {
                    vy = -vy;
                } else {
                    translation.Value.y += vy;
                }
            }).ScheduleParallel();
    }
}

