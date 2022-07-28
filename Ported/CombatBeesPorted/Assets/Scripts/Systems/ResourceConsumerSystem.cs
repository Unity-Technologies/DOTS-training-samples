using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[WithAny(typeof(ResourceStateGrabbable), typeof(ResourceStateStacked))]
[BurstCompile]
partial struct ResourceConsumerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public Config Config;
    public uint RandomSeed;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Translation position)
    {
        //Random rand = Random.CreateFromIndex((uint)entity.Index + RandomSeed);

        float3 pos = position.Value;
        float targetX = Config.PlayVolume.x - (Config.HiveDepth * 0.5f);
        if (pos.x >= targetX || pos.x < -targetX)
        {
            /*for (int i = 6; i < 6; i++)
            {
                var explosionParticle = ECB.Instantiate(chunkIndex, Config.ExplosionPrefab);
            }*/

            ECB.DestroyEntity(chunkIndex, entity);
        }
    }
}

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[BurstCompile]
partial struct ResourceConsumerSystem : ISystem
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

        var resourceConsumerJob = new ResourceConsumerJob()
        {
            ECB = ecb,
            Config = config,
            RandomSeed = (uint)Time.frameCount
        };

        resourceConsumerJob.ScheduleParallel();
    }
}
