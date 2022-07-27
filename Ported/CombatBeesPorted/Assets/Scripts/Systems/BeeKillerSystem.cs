using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[WithAll(typeof(BeeStateDead))]
partial struct BeeKillerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public uint RandomSeed;
    public Config Config;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Velocity velocity, in Translation position, in EntityOfInterest entityOfInterest)
    {
        Random rand = Random.CreateFromIndex((uint)(entity.Index) + RandomSeed);
        
        ECB.DestroyEntity(chunkIndex, entity);

        // TODO - enable once we have particle clean-up
        /*
        for (int i = 0; i < 6; ++i)
        {
            var bloodParticle = ECB.Instantiate(chunkIndex, Config.BloodPrefab);
            var particleVelocity = velocity.Value + rand.NextFloat3() * 2;
            var particleScale = Vector3.one * rand.NextFloat(.1f, .2f);
            ECB.SetComponent(chunkIndex, bloodParticle, new NonUniformScale { Value = particleScale });
            ECB.SetComponent(chunkIndex, bloodParticle, new Velocity { Value = particleVelocity });
        }
        */
    }
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(BeeAttackingSystem))]
[BurstCompile]
public partial struct BeeKillerSystem : ISystem
{
    private StorageInfoFromEntity storageInfo;
    private ComponentDataFromEntity<Translation> translationComponentData;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        
        storageInfo = state.GetStorageInfoFromEntity();
        translationComponentData = state.GetComponentDataFromEntity<Translation>();
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

        storageInfo.Update(ref state);
        translationComponentData.Update(ref state);

        var beeKillerJob = new BeeKillerJob()
        {
            RandomSeed = (uint)Time.frameCount,
            Config = config,
            ECB = ecb,
        };

        beeKillerJob.ScheduleParallel();
    }
}
