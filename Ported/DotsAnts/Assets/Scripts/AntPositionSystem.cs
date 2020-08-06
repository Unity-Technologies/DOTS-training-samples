using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class AntPositionSystem : SystemBase
{
    private struct DefaultValues
    {
        public int antCount;
        public int mapSize;
        public int bucketResolution;
        public Vector3 antSize;
        public float antSpeed;
    
        [Range(0f,1f)]
        public float antAccel;
        public float trailAddSpeed;
        public float trailDecay;
        public float randomSteering;
        public float pheromoneSteerStrength;
        public float wallSteerStrength;
        public float goalSteerStrength;
        public float outwardStrength;
        public float inwardStrength;
        public int rotationResolution;
        public int obstacleRingCount;
        public float obstaclesPerRing;
        public float obstacleRadius;
    }

    private DefaultValues defaults = new DefaultValues();

    
    protected override void OnCreate()
    {
        base.OnCreate();

        
    }

    
    protected override void OnUpdate()
    {
        var antDefaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
                
        defaults.antCount = antDefaults.antCount;
        defaults.mapSize = antDefaults.mapSize;
        defaults.bucketResolution = antDefaults.bucketResolution;
        defaults.antSize = antDefaults.antSize;
        defaults.antSpeed = antDefaults.antSpeed;
        
        defaults.antAccel = antDefaults.antAccel;
        defaults.trailAddSpeed = antDefaults.trailAddSpeed;
        defaults.trailDecay = antDefaults.trailDecay;
        defaults.randomSteering = antDefaults.randomSteering;
        defaults.pheromoneSteerStrength = antDefaults.pheromoneSteerStrength;
        defaults.wallSteerStrength = antDefaults.wallSteerStrength;
        defaults.goalSteerStrength = antDefaults.goalSteerStrength;
        defaults.outwardStrength = antDefaults.outwardStrength;
        defaults.inwardStrength = antDefaults.inwardStrength;
        defaults.rotationResolution = antDefaults.rotationResolution;
        defaults.obstacleRingCount = antDefaults.obstacleRingCount;
        defaults.obstaclesPerRing = antDefaults.obstaclesPerRing;
        defaults.obstacleRadius = antDefaults.obstacleRadius;
        
        Unity.Mathematics.Random mathRandom = new Unity.Mathematics.Random(23345678);
        var defaultValues = defaults;
        
        float2 colonyPosition = GetSingleton<ColonyLocation>().value;

        Entities.WithAll<Ant, LocalToWorld>().ForEach(
                (ref Translation translation, ref Rotation rotation, ref Position position, ref DirectionAngle angle,
                    ref Speed speed, in SteeringAngle steeringAngle, in CarryingFood carryingFood) =>
                {
                    float targetSpeed = speed.value;

                    // add random turn
                    angle.value += mathRandom.NextFloat(-defaultValues.randomSteering, defaultValues.randomSteering);

                    // add the steering angles
                    angle.value += steeringAngle.value.x * defaultValues.pheromoneSteerStrength;
                    angle.value += steeringAngle.value.y * defaultValues.wallSteerStrength;

                    targetSpeed *= 1f - (Mathf.Abs(steeringAngle.value.x) + Mathf.Abs(steeringAngle.value.y)) / 3f;
                    //speed.value += (targetSpeed - speed.value) * defaultValues.antAccel;
                    speed.value = 0.1f;

                    // TODO: need a way to update ant color 

                    // TODO: Do the linecasting to bounce from wall

                    // update ant position
                    float vx = Mathf.Cos(angle.value) * speed.value;
                    float vy = Mathf.Sin(angle.value) * speed.value;
                    float ovx = vx;
                    float ovy = vy;

                    if (position.value.x + vx < 0f || position.value.x + vx > defaultValues.mapSize)
                    {
                        vx = -vx;
                    }
                    else
                    {
                        position.value.x += vx;
                    }

                    if (position.value.y + vy < 0f || position.value.y + vy > defaultValues.mapSize)
                    {
                        vy = -vy;
                    }
                    else
                    {
                        position.value.y += vy;
                    }

                    float dx, dy, dist;
                    // TODO: Check for obstacles

                    // TODO: Check if this makes sense
                    float inwardOrOutward = -defaultValues.outwardStrength;
                    float pushRadius = defaultValues.mapSize * .4f;
                    if (carryingFood.value)
                    {
                        inwardOrOutward = defaultValues.inwardStrength;
                        pushRadius = defaultValues.mapSize;
                    }



                    dx = colonyPosition.x - position.value.x;
                    dy = colonyPosition.y - position.value.y;
                    dist = Mathf.Sqrt(dx * dx + dy * dy);
                    inwardOrOutward *= 1f - Mathf.Clamp01(dist / pushRadius);
                    vx += dx / dist * inwardOrOutward;
                    vy += dy / dist * inwardOrOutward;

                    if (ovx != vx || ovy != vy)
                    {
                        angle.value = Mathf.Atan2(vy, vx);
                    }

                    // updating the actual entity positions in the world
                    translation.Value = new float3(position.value.x, 0.0f, position.value.y);
                    //translation.Value += new float3(0.001f, 0.0f, 0.0f);
                    rotation.Value = quaternion.Euler(0, -angle.value, 0, math.RotationOrder.XYZ);
                }
            )
            .ScheduleParallel();
    }
}
