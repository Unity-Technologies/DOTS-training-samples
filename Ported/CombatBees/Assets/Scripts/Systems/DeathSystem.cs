﻿using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
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
        public float Gravity;

        public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Bee bee, ref Dead deadComponent, ref Physical physical)
        {
            var random = Random.CreateFromIndex(RandomSeed + (uint)chunkIndex);
            if (random.NextFloat() < (deadComponent.DeathTimer - .5f) * .5f)
            {
                //ParticleManager.SpawnParticle(physical.Position,ParticleType.Blood,Vector3.zero);
                //Debug.Log($"I'm spawning blood particle!");
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
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicalSystem))]
    public partial struct DeathSystem : ISystem
    {
        private EntityQuery _allEntities;
        private ComponentLookup<Dead> _bees;
        private Random _random;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
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
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var allEntities = _allEntities.ToEntityArray(Allocator.Temp);
                var target = allEntities[_random.NextInt(allEntities.Length)];
                ecb.SetComponentEnabled<Dead>(target, true);
            }

            var dt = SystemAPI.Time.DeltaTime;

            var playDeadJob = new PlayDeadJob()
            {
                Gravity = Field.gravity,
                DeltaTime = dt,
                Ecb = ecb.AsParallelWriter(),
                RandomSeed = _random.NextUInt()
            };

            playDeadJob.ScheduleParallel();
        }
    }
}