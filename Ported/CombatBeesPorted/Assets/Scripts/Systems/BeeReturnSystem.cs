using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[WithAll(typeof(BeeStateReturning))]
[BurstCompile]
partial struct BeeReturnJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    [ReadOnly] public ComponentDataFromEntity<Translation> TargetTranslationComponentData;
    // [ReadOnly] public ComponentDataFromEntity<NonUniformScale> TargetNonUniformScaleComponentData;
    public Config Config;

    void Execute([ChunkIndexInQuery] int chunkIndex, in Entity entity, in NonUniformScale scale, in Translation position, in EntityOfInterest entityOfInterest)
    {
        Entity targetEntity = entityOfInterest.Value;
        if (TargetTranslationComponentData.HasComponent(targetEntity))
        {
            float3 entityPos = position.Value;
            float offset = (scale.Value.y * 0.5f) + 0.5f;// (TargetNonUniformScaleComponentData[targetEntity].Value.y * 0.5f);
            entityPos.y -= offset;
            ECB.SetComponent<Translation>(chunkIndex, targetEntity, new Translation { Value = entityPos });
        }

        float targetX = Config.PlayVolume.x - (Config.HiveDepth * 0.5f);
        if (position.Value.x >= targetX || position.Value.x < -targetX)
        {
            ECB.SetComponentEnabled<ResourceStateGrabbed>(chunkIndex, targetEntity, false);
            ECB.SetComponentEnabled<Gravity>(chunkIndex, targetEntity, true);

            ECB.SetComponentEnabled<BeeStateReturning>(chunkIndex, entity, false);
            ECB.SetComponentEnabled<BeeStateIdle>(chunkIndex, entity, true);
        }
    }
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct BeeReturnSystem : ISystem
{
    private ComponentDataFromEntity<Translation> targetTranslationComponentData;
    // private ComponentDataFromEntity<NonUniformScale> targetNonUniformScaleComponentData;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        targetTranslationComponentData = state.GetComponentDataFromEntity<Translation>();
        // targetNonUniformScaleComponentData = state.GetComponentDataFromEntity<NonUniformScale>();
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

        targetTranslationComponentData.Update(ref state);
        // targetNonUniformScaleComponentData.Update(ref state);

        var returnJob = new BeeReturnJob()
        {
            ECB = ecb,
            TargetTranslationComponentData = targetTranslationComponentData,
            // TargetNonUniformScaleComponentData = targetNonUniformScaleComponentData,
            Config = config
        };
        returnJob.ScheduleParallel();
    }
}
