using Components;
using Helpers;
using Systems.Particles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [WithAll(typeof(Dead))]
    [BurstCompile]
    public partial struct PlayDeadJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public uint RandomSeed;
        public float DeltaTime;
        public Entity BloodParticlePrefab;

        public void Execute([ChunkIndexInQuery] int chunkIndex, in Entity entity, ref Dead deadComponent,
            ref Physical physical)
        {
            var random = Random.CreateFromIndex(RandomSeed + (uint)chunkIndex);
             if (random.NextFloat() < (deadComponent.DeathTimer - .5f) * .2f)
             {
                 ParticleBuilder.SpawnParticleEntity(Ecb, chunkIndex, random.NextUInt(), BloodParticlePrefab,
                     physical.Position,
                     ParticleType.Blood, float3.zero, 3f);
             }

            if (!deadComponent.IsSlowed)
            {
                physical.Velocity *= .5f;
                deadComponent.IsSlowed = true;
            }

            physical.IsFalling = true;
            physical.Collision = Physical.FieldCollisionType.Splat;
            deadComponent.DeathTimer -= DeltaTime;

            if (deadComponent.DeathTimer < 0f)
            {
                Ecb.DestroyEntity(chunkIndex, entity);
            }
        }
    }

    [BurstCompile]
    [UpdateAfter(typeof(BeeSwarmingSystem))]
    public partial struct DeathSystem : ISystem
    {
        private EntityQuery _allEntities;
        private ComponentLookup<Dead> _bees;
        private Random _random;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeeConfig>();
            _random = Random.CreateFromIndex(999);
            _allEntities = SystemAPI.QueryBuilder().WithAll<Bee>().WithNone<Dead>().Build();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var dt = SystemAPI.Time.DeltaTime;
            var config = SystemAPI.GetSingleton<BeeConfig>();

            var playDeadJob = new PlayDeadJob()
            {
                DeltaTime = dt,
                Ecb = ecb.AsParallelWriter(),
                RandomSeed = _random.NextUInt(),
                BloodParticlePrefab = config.BloodParticlePrefab,
            };

            state.Dependency = playDeadJob.ScheduleParallel(state.Dependency);
        }
    }
}