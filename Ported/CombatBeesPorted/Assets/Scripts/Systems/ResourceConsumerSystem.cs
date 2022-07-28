using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[WithAny(typeof(ResourceStateGrabbable), typeof(ResourceStateStacked))]
[BurstCompile]
partial struct ResourceConsumerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public Config Config;
    public uint RandomSeed;

    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Translation position, in Velocity velocity)
    {
        Random rand = Random.CreateFromIndex((uint)entity.Index + RandomSeed);

        float3 pos = position.Value;
        float targetX = Config.PlayVolume.x - (Config.HiveDepth * 0.5f);
        if (pos.x >= targetX || pos.x < -targetX)
        {
            for (int i = 0; i < 6; i++)
            {
                var explosionParticle = ECB.Instantiate(chunkIndex, Config.ExplosionPrefab);
                var particleVelocity = velocity.Value + rand.NextFloat3() * 5;
                var particleScale = math.float3(1f, 1f, 1f) * rand.NextFloat(1f, 2f);

                ECB.SetComponent(chunkIndex, explosionParticle, new AnimationTime { Value = Config.ExplosionDuration });
                ECB.SetComponent(chunkIndex, explosionParticle, new NonUniformScale { Value = particleScale });
                ECB.SetComponent(chunkIndex, explosionParticle, new Velocity { Value = particleVelocity });
                ECB.SetComponent(chunkIndex, explosionParticle, new Translation { Value = position.Value });
            }

            for (int i = 0; i < Config.BeesPerResource; i++)
            {
                var beeEntity = ECB.Instantiate(chunkIndex, Config.BeePrefab);

                if (pos.x > targetX)
                {
                    ECB.AddComponent<YellowTeam>(chunkIndex, beeEntity);
                    ECB.SetComponent(chunkIndex, beeEntity, new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)Color.yellow });
                }
                else
                {
                    ECB.AddComponent<BlueTeam>(chunkIndex, beeEntity);
                    ECB.SetComponent(chunkIndex, beeEntity, new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)Color.blue });
                }

                ECB.SetComponent<Translation>(chunkIndex, beeEntity, position);
                ECB.SetComponentEnabled<BeeStateAttacking>(chunkIndex, beeEntity, false);
                ECB.SetComponentEnabled<BeeStateGathering>(chunkIndex, beeEntity, false);
                ECB.SetComponentEnabled<BeeStateReturning>(chunkIndex, beeEntity, false);
                ECB.SetComponentEnabled<BeeStateDead>(chunkIndex, beeEntity, false);
            }

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
