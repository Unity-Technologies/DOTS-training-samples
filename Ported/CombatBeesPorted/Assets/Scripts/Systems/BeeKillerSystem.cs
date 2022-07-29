using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Random = Unity.Mathematics.Random;

[WithAll(typeof(BeeStateDead))]
[BurstCompile]
partial struct BeeKillerJob : IJobEntity
{
    [ReadOnly] public StorageInfoFromEntity TargetStorageInfo;
    public EntityCommandBuffer.ParallelWriter ECB;
    public uint RandomSeed;
    public Config Config;
    [ReadOnly] public ComponentDataFromEntity<ResourceStateGrabbed> ResourceStateGrabbedComponentData;
    [ReadOnly] public ComponentDataFromEntity<ResourceStateGrabbable> ResourceStateGrabbableComponentData;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Velocity velocity, in Translation position,
        in EntityOfInterest entityOfInterest)
    {
        Random rand = Random.CreateFromIndex((uint)(entity.Index) + RandomSeed);
        
        ECB.SetComponent(chunkIndex, entity, new EntityOfInterest());
        ECB.DestroyEntity(chunkIndex, entity);

        for (int i = 0; i < 5; ++i)
        {
            var bloodParticle = ECB.Instantiate(chunkIndex, Config.BloodPrefab);
            var particleVelocity = velocity.Value + rand.NextFloat3Direction() * 4f;
            var particleScale = math.float3(1f,1f,1f) * rand.NextFloat(0.33f, 1f);

            ECB.SetComponent(chunkIndex, bloodParticle, new NonUniformScale { Value = particleScale });
            ECB.SetComponent(chunkIndex, bloodParticle, new Velocity { Value = particleVelocity });
            ECB.SetComponent(chunkIndex, bloodParticle, new Translation { Value = position.Value });
            ECB.SetComponent(chunkIndex, bloodParticle, new AnimationTime() { Value = Config.BloodDuration });
        }
        
        bool exists = TargetStorageInfo.Exists(entityOfInterest.Value);
        bool isGrabbable = exists
                           && ResourceStateGrabbableComponentData.HasComponent(entityOfInterest.Value)
                           && ResourceStateGrabbableComponentData.IsComponentEnabled(entityOfInterest.Value);

        bool isGrabbed = exists
                         && ResourceStateGrabbedComponentData.HasComponent(entityOfInterest.Value)
                         && ResourceStateGrabbedComponentData.IsComponentEnabled(entityOfInterest.Value);
        
        if (isGrabbable || isGrabbed)
        {
            ECB.SetComponentEnabled<ResourceStateStacked>(chunkIndex, entityOfInterest.Value, false);
            ECB.SetComponentEnabled<ResourceStateGrabbable>(chunkIndex, entityOfInterest.Value, true);
            ECB.SetComponentEnabled<ResourceStateGrabbed>(chunkIndex, entityOfInterest.Value, false);
            ECB.SetComponentEnabled<Gravity>(chunkIndex, entityOfInterest.Value, true);
        }
    }
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(BeeAttackingSystem))]
[BurstCompile]
public partial struct BeeKillerSystem : ISystem
{
    private StorageInfoFromEntity _storageInfo;
    private ComponentDataFromEntity<ResourceStateGrabbed> _resourceStateGrabbedComponentData;
    private ComponentDataFromEntity<ResourceStateGrabbable> _resourceStateGrabbableComponentData;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        _storageInfo = state.GetStorageInfoFromEntity();
        _resourceStateGrabbedComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbed>();
        _resourceStateGrabbableComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbable>();
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

        _storageInfo.Update(ref state);
        _resourceStateGrabbedComponentData.Update(ref state);
        _resourceStateGrabbableComponentData.Update(ref state);

        var beeKillerJob = new BeeKillerJob()
        {
            ResourceStateGrabbedComponentData = _resourceStateGrabbedComponentData,
            ResourceStateGrabbableComponentData = _resourceStateGrabbableComponentData,
            RandomSeed = (uint)Time.frameCount,
            Config = config,
            ECB = ecb,
            TargetStorageInfo = _storageInfo,
        };

        beeKillerJob.ScheduleParallel();
    }
}