using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(ColonySpawnerSystem))]
[UpdateAfter(typeof(ResourceSpawnerSystem))]
[UpdateAfter(typeof(PlayerSystem))]
public class AntMovementSystem : SystemBase
{
    private const int mapSize = 128;
    private const float antSpeed = 0.2f;
    private const float randomSteering = 0.14f;
    private const float goalSteerStrength = 0.03f;
    private const float pheronomeSteeringDistance = 3f;
    private const float pheromoneSteerStrength = 0.015f;

    protected override void OnUpdate()
    {
        Random rand = new Random(1234);

        Entities
            .ForEach((ref FacingAngle facingAngle) =>
            {
                facingAngle.Value += rand.NextFloat(-randomSteering, randomSteering);
            }).ScheduleParallel();

        var mapSetting = GetSingleton<PheromoneMapSetting>();

        var pheromoneMapEntity = GetSingletonEntity<Pheromone>();
        var pheromoneMapBuffer = GetBuffer<Pheromone>(pheromoneMapEntity).Reinterpret<float4>();
        Entities
            //Need to add this so the job system stop complaining. We are simply reading and never setting.
            //I don't know if we can make a DynamicBuffer Readonly.
            .WithNativeDisableParallelForRestriction(pheromoneMapBuffer)
            .ForEach((ref FacingAngle facingAngle, in Translation translation) => 
            {
                float pheroSteering = 0;
                for (int i = -1; i <= 1; i += 2)
                {
                    float angle = facingAngle.Value + i * math.PI * .25f;
                    Translation position = translation;
                    position.Value.x += math.cos(angle) * pheronomeSteeringDistance;
                    position.Value.y += math.sin(angle) * pheronomeSteeringDistance;

                    if (PheromoneMapSystem.TryGetClosestPheronomoneIndexFromTranslation(position, mapSetting, out int index))
                    {
                        float value = pheromoneMapBuffer[index].x;
                        pheroSteering += value * i;
                    }
                }
                facingAngle.Value += math.sign(pheroSteering) * pheromoneSteerStrength;

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
        var playerEntity = GetSingletonEntity<PlayerInput>();
        var playerSpeed = GetComponent<PlayerInput>(playerEntity).Speed;
        Entities
            .ForEach((ref Translation translation, ref Speed speed, in Acceleration acceleration, in FacingAngle facingAngle) =>
            {
                var targetSpeed = antSpeed*time*playerSpeed;
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

