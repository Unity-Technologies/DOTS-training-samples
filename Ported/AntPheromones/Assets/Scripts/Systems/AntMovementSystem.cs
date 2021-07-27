using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AntMovementSystem : SystemBase
{
    private const int mapSize = 128;
    private const float antSpeed = 0.2f;
    private const float randomSteering = 0.14f;
    private const float goalSteerStrength = 0.03f;
    protected override void OnUpdate()
    {
        Random rand = new Random(1234);

        Entities
            .ForEach((ref FacingAngle facingAngle) =>
            {
                facingAngle.Value += rand.NextFloat(-randomSteering, randomSteering);
            }).ScheduleParallel();

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelWriter = ecb.AsParallelWriter();
        var colonyEntity = GetSingletonEntity<Colony>();
        var colonyPosition = GetComponent<Translation>(colonyEntity).Value;
        Entities
            .WithAll<HoldingResource>()
            .ForEach((Entity entity, int entityInQueryIndex, ref FacingAngle facingAngle, in Translation translation) =>
            {
                float3 targetPos = colonyPosition;
                facingAngle.Value = SteerToTarget(targetPos, translation.Value, facingAngle.Value);
                
                if (math.lengthsq(translation.Value - targetPos) < 4f * 4f)
                {
                    parallelWriter.RemoveComponent<HoldingResource>(entityInQueryIndex, entity);
                    facingAngle.Value += math.PI;
                }
            }).ScheduleParallel();
        var resourceEntity = GetSingletonEntity<Resource>();
        var resourcePosition = GetComponent<Translation>(resourceEntity).Value;
        Entities
            .WithNone<HoldingResource>()
            .ForEach((Entity entity, int entityInQueryIndex, ref FacingAngle facingAngle, in Translation translation) =>
            {
                float3 targetPos = resourcePosition;
                facingAngle.Value = SteerToTarget(targetPos, translation.Value, facingAngle.Value);
                              
                if (math.lengthsq(translation.Value - targetPos) < 4f * 4f)
                {
                    parallelWriter.AddComponent<HoldingResource>(entityInQueryIndex, entity);
                    facingAngle.Value += math.PI;
                }
            }).ScheduleParallel();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        var time = Time.DeltaTime;
        Entities
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
            }).ScheduleParallel();
    }

    private static float SteerToTarget(float3 targetPos, float3 antPos, float facingAngle)
    {
        //TODO: handle obstacles
        //if (Linecast(ant.position,targetPos)==false)
        {
            float targetAngle = Mathf.Atan2(targetPos.y-antPos.y,targetPos.x-antPos.x);
            if (targetAngle - facingAngle > Mathf.PI) 
            {
                facingAngle += Mathf.PI * 2f;
            } 
            else if (targetAngle - facingAngle < -Mathf.PI) 
            {
                facingAngle -= Mathf.PI * 2f;
            }
            else
            {
                if (Mathf.Abs(targetAngle - facingAngle) < Mathf.PI * .5f)
                    facingAngle += (targetAngle - facingAngle) * goalSteerStrength;
            }
            return facingAngle;
        }
    }
}

