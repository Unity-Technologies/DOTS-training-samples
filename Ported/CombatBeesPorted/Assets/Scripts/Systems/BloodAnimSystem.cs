using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WithAll(typeof(BloodAnim))]
[BurstCompile]
partial struct BloodAnimJob : IJobEntity
{
    public float DeltaTime;
    public float BloodDuration;
    public float3 Bounds;
    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref NonUniformScale scale,
        ref AnimationTime animTime, in Translation pos)
    {
        if (pos.Value.x <= -Bounds.x + 0.01f ||
            pos.Value.x >= Bounds.x - 0.01f ||
            pos.Value.y <= -Bounds.y + 0.01f ||
            pos.Value.y >= Bounds.y - 0.01f ||
            pos.Value.z <= -Bounds.z + 0.01f ||
            pos.Value.z >= Bounds.z - 0.01f)
        {
            ECB.SetComponentEnabled<Gravity>(chunkIndex, entity, false);
            ECB.SetComponent(chunkIndex, entity, new Velocity() { Value = float3.zero });
        }

        animTime.Value -= DeltaTime;
        if (animTime.Value <= 0f)
        {
            ECB.DestroyEntity(chunkIndex, entity);
        }
        else
        {
            // TODO: Animate Material Alpha
            scale.Value = math.lerp(float3.zero, 1f, math.remap(0f, BloodDuration, 0f, 1f, animTime.Value));
        }
    }
}


[BurstCompile]
public partial struct BloodAnimSystem : ISystem
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
        var bloodAnimJob = new BloodAnimJob
        {
            DeltaTime = state.Time.DeltaTime,
            BloodDuration = config.BloodDuration,
            ECB = ecb,
            Bounds = config.PlayVolume
        };
        bloodAnimJob.ScheduleParallel();
    }
}