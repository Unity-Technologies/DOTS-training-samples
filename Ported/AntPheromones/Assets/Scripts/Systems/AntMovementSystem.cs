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
    protected override void OnUpdate()
    {
        Random rand = new Random((uint)UnityEngine.Random.Range(1, 1000000));
        var mapSize = GetComponent<MapSetting>(GetSingletonEntity<MapSetting>()).WorldSize;
        var generalSettingsEntity = GetSingletonEntity<GeneralSettings>();
        var antSpeed = GetComponent<GeneralSettings>(generalSettingsEntity).AntSpeed;
        var randomSteering = GetComponent<GeneralSettings>(generalSettingsEntity).RandomSteering;
        var goalSteerStrength = GetComponent<GeneralSettings>(generalSettingsEntity).GoalSteerStrength;
        var pheronomeSteeringDistance = GetComponent<GeneralSettings>(generalSettingsEntity).PheromoneSteeringDistance;
        var pheromoneSteerStrength = GetComponent<GeneralSettings>(generalSettingsEntity).PheromoneSteerStrength;
        var inwardStrength = GetComponent<GeneralSettings>(generalSettingsEntity).InwardStrength;
        var outwardStrength = GetComponent<GeneralSettings>(generalSettingsEntity).OutwardStrength;
        var playerEntity = GetSingletonEntity<PlayerInput>();
        var playerSpeed = GetComponent<PlayerInput>(playerEntity).Speed;
        var colonyEntity = GetSingletonEntity<Colony>();
        var colonyPosition = GetComponent<Translation>(colonyEntity).Value;
        var mapSetting = GetSingleton<MapSetting>();
        var pheromoneMapEntity = GetSingletonEntity<Pheromone>();
        var pheromoneMapBuffer = GetBuffer<Pheromone>(pheromoneMapEntity).Reinterpret<float4>();
        var resourceEntity = GetSingletonEntity<Resource>();
        var resourcePosition = GetComponent<Translation>(resourceEntity).Value;

        var time = Time.DeltaTime * playerSpeed;

        Entities
            .ForEach((ref FacingAngle facingAngle) =>
            {
                facingAngle.Value += rand.NextFloat(-randomSteering, randomSteering) * time;
            }).ScheduleParallel();

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
        
                    if (PheromoneMapSystem.TryGetClosestPheronomoneIndexFromTranslation(position.Value, mapSetting, out int index))
                    {
                        float value = pheromoneMapBuffer[index].x;
                        pheroSteering += value * i;
                    }
                }
                facingAngle.Value += math.sign(pheroSteering) * pheromoneSteerStrength * time;
        
            }).ScheduleParallel();
        
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelWriter = ecb.AsParallelWriter();
        Entities
            .WithAll<HoldingResource>()
            .ForEach((Entity entity, int entityInQueryIndex, ref FacingAngle facingAngle, in Translation translation) =>
            {
                float3 targetPos = colonyPosition;
                facingAngle.Value = SteerToTarget(targetPos, translation.Value, facingAngle.Value, goalSteerStrength, time);
        
                if (math.lengthsq(translation.Value - targetPos) < 4f * 4f)
                {
                    parallelWriter.RemoveComponent<HoldingResource>(entityInQueryIndex, entity);
                    facingAngle.Value += math.PI;
                }
            }).ScheduleParallel();
        Entities
            .WithNone<HoldingResource>()
            .ForEach((Entity entity, int entityInQueryIndex, ref FacingAngle facingAngle, in Translation translation) =>
            {
                float3 targetPos = resourcePosition;
                facingAngle.Value = SteerToTarget(targetPos, translation.Value, facingAngle.Value, goalSteerStrength, time);
        
                if (math.lengthsq(translation.Value - targetPos) < 4f * 4f)
                {
                    parallelWriter.AddComponent<HoldingResource>(entityInQueryIndex, entity);
                    facingAngle.Value += math.PI;
                }
            }).ScheduleParallel();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();

        Entities
            .ForEach((ref Translation translation, ref Speed speed, in Acceleration acceleration, in FacingAngle facingAngle) =>
            {
                var targetSpeed = antSpeed*time;
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
        
        Entities
            .WithAll<HoldingResource>()
            .ForEach((ref FacingAngle facingAngle, ref Translation translation, ref Rotation rotation, in Speed speed) =>
            {
                ResolveMovement(ref facingAngle, ref translation, ref rotation, speed, mapSize, colonyPosition, true, outwardStrength, inwardStrength);
            }).ScheduleParallel();
        Entities
            .WithNone<HoldingResource>()
            .ForEach((ref FacingAngle facingAngle, ref Translation translation, ref Rotation rotation, in Speed speed) =>
            {
                ResolveMovement(ref facingAngle, ref translation, ref rotation, speed, mapSize, colonyPosition, false, outwardStrength, inwardStrength);
            }).ScheduleParallel();
    }

    private static float SteerToTarget(float3 targetPos, float3 antPos, float facingAngle, float goalSteerStrength, float time)
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
                    facingAngle += (targetAngle - facingAngle) * goalSteerStrength * time;
            }
            return facingAngle;
        }
    }

    private static void ResolveMovement(ref FacingAngle facingAngle, ref Translation translation, ref Rotation rotation, Speed speed, float mapSize, float3 colonyPosition, bool holdingResource, float outwardStrength, float inwardStrength)
    {
        float vx = math.cos(facingAngle.Value) * speed.Value;
        float vy = math.sin(facingAngle.Value) * speed.Value;
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

        float dx, dy, dist;
        //TODO: Add collision resolution here

        float inwardOrOutward = -outwardStrength;
        float pushRadius = mapSize * .1f;
        if (holdingResource) {
            inwardOrOutward = inwardStrength;
            pushRadius = mapSize;
        }
        dx = colonyPosition.x - translation.Value.x;
        dy = colonyPosition.y - translation.Value.y;
        dist = Mathf.Sqrt(dx * dx + dy * dy);
        inwardOrOutward *= 1f-Mathf.Clamp01(dist / pushRadius);
        vx += dx / dist * inwardOrOutward;
        vy += dy / dist * inwardOrOutward;

        if (ovx != vx || ovy != vy) {
            facingAngle.Value = Mathf.Atan2(vy,vx);
        }
        rotation.Value = quaternion.AxisAngle(Vector3.forward,facingAngle.Value);
    }
}

