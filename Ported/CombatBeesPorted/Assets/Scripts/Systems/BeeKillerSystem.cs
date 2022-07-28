using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
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

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Velocity velocity, in Translation position, in EntityOfInterest entityOfInterest)
    {
        Random rand = Random.CreateFromIndex((uint)(entity.Index) + RandomSeed);
        
        ECB.DestroyEntity(chunkIndex, entity);
        
        for (int i = 0; i < 6; ++i)
        {
            var bloodParticle = ECB.Instantiate(chunkIndex, Config.BloodPrefab);
            var particleVelocity = velocity.Value + rand.NextFloat3() * 2;
            var particleScale = Vector3.one * rand.NextFloat(.1f, .2f);
            
            ECB.SetComponent(chunkIndex, bloodParticle, new NonUniformScale { Value = particleScale });
            ECB.SetComponent(chunkIndex, bloodParticle, new Velocity { Value = particleVelocity });
            ECB.SetComponent(chunkIndex, bloodParticle, new Translation { Value = position.Value });
        }

        // If the bee had a grabbed resource when it died, then we need to turn on gravity and re-enable it.
        if (TargetStorageInfo.Exists(entityOfInterest.Value) 
            && ResourceStateGrabbedComponentData.HasComponent(entityOfInterest.Value) 
            && ResourceStateGrabbedComponentData.IsComponentEnabled(entityOfInterest.Value))
        {
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

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        _storageInfo = state.GetStorageInfoFromEntity();
        _resourceStateGrabbedComponentData = state.GetComponentDataFromEntity<ResourceStateGrabbed>();
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

        var beeKillerJob = new BeeKillerJob()
        {
            ResourceStateGrabbedComponentData = _resourceStateGrabbedComponentData,
            RandomSeed = (uint)(Time.frameCount + System.DateTime.Now.Second),
            Config = config,
            ECB = ecb,
            TargetStorageInfo = _storageInfo,
        };

        beeKillerJob.ScheduleParallel();
        
    }
}
