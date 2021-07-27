using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AntMovementSystem : SystemBase
{
    private const int mapSize = 128;
    private const float antSpeed = 0.2f;
    private const float randomSteering = 0.14f;
    protected override void OnUpdate()
    {
        Random rand = new Random(1234);

        var firstHandle = Entities
            .ForEach((ref FacingAngle facingAngle) =>
            {
                facingAngle.Value += rand.NextFloat(-randomSteering, randomSteering);
            }).ScheduleParallel(Dependency);
        
        var time = Time.DeltaTime;

        var secondHandle = Entities
            .ForEach((ref Translation translation, ref Speed speed, in Acceleration acceleration, in FacingAngle facingAngle) =>
            {
                var targetSpeed = antSpeed*time; //TODO: multiply by playingSpeed
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
            }).ScheduleParallel(firstHandle);
        Dependency = secondHandle;
    }
}

