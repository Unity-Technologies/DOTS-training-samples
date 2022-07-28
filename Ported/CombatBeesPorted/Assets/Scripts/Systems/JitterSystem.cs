using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct JitterJob : IJobEntity
{
    public float DeltaTime;
    public float JitterTimeMin;
    public float JitterTimeMax;
    public float JitterDistanceMax;
    public float BeeMoveSpeed;
    public Entity TargetEntity;
    public uint RandomSeed;

    void Execute(Entity e, ref Bee bee, in Translation pos, ref Velocity vel, ref NonUniformScale scale,
        in TargetPosition targetPos)
    {
        var rand = Random.CreateFromIndex(RandomSeed + (uint)e.Index);

        bee.JitterTime -= DeltaTime;
        if (bee.JitterTime <= 0f)
        {
            float3 targetDir;
            var deltaPos = targetPos.Value - pos.Value;
            var dist = math.length(deltaPos);
            if (dist > math.EPSILON)
            {
                targetDir = math.normalize(deltaPos) * BeeMoveSpeed * rand.NextFloat(0.85f, 1.15f);
            }
            else
            {
                targetDir = math.normalize(vel.Value);
                dist = BeeMoveSpeed;
            }

            var distFactor = math.remap(0f, 12f * 0.67f, 0.1f, 1f, dist);
            distFactor = math.clamp(distFactor, 0f, 1f);

            bee.JitterTime = rand.NextFloat(JitterTimeMin, JitterTimeMax);

            var randomDir = rand.NextFloat2(-JitterDistanceMax, JitterDistanceMax);
            randomDir.y = math.abs(randomDir.y);
            var jitterDir = new float3(0f, randomDir.y * 2.5f, randomDir.x) * distFactor;

            vel.Value = targetDir + jitterDir;
        }

        scale.Value.z = math.lerp(bee.JitterTime / JitterTimeMax, 0.67f, 2.33f);
    }
}


[BurstCompile]
public partial struct JitterSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var jitterTimeMin = config.JitterTimeMin;
        var jitterTimeMax = config.JitterTimeMax;
        var jitterDistanceMax = config.JitterDistanceMax;
        var beeMoveSpeed = config.BeeMoveSpeed;
        var gravityJob = new JitterJob()
        {
            DeltaTime = state.Time.DeltaTime,
            JitterTimeMin = jitterTimeMin,
            JitterTimeMax = jitterTimeMax,
            JitterDistanceMax = jitterDistanceMax,
            BeeMoveSpeed = beeMoveSpeed,
            RandomSeed = (uint)UnityEngine.Time.frameCount
        };
        gravityJob.ScheduleParallel();
    }
}