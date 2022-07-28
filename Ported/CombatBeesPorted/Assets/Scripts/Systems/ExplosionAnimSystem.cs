using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WithAll(typeof(ExplosionAnim))]
[BurstCompile]
partial struct ExplosionAnimJob : IJobEntity
{
    public float DeltaTime;
    public float ExplosionDuration;
    public float3 Bounds;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref NonUniformScale scale,
        ref AnimationTime animTime, in Translation pos)
    {
        animTime.Value -= DeltaTime;
        if (animTime.Value <= 0f)
        {
            ECB.DestroyEntity(chunkIndex, entity);
        }
        else
        {
            scale.Value = math.lerp(float3.zero, new float3(1f, 1f, 1f),
                math.remap(0f, ExplosionDuration, 0f, 1f, animTime.Value));
        }
    }
}


[BurstCompile]
public partial struct ExplosionAnimSystem : ISystem
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

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        var explosionAnimJob = new ExplosionAnimJob
        {
            DeltaTime = state.Time.DeltaTime,
            ExplosionDuration = config.ExplosionDuration,
            ECB = ecb,
            Bounds = config.PlayVolume
        };
        explosionAnimJob.ScheduleParallel();
    }
}