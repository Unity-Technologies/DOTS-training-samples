using Components;
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
                //ParticleManager.SpawnParticle(bee.position,ParticleType.Blood,Vector3.zero);
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
    public partial struct DeathSystem : ISystem
    {
        private EntityQuery _allEntities;
        private ComponentLookup<Dead> _bees;
        private Random _random;
        

        public void OnCreate(ref SystemState state)
        {
            _random = Random.CreateFromIndex(999);
            _allEntities = state.GetEntityQuery(typeof(Bee), ComponentType.Exclude<Dead>());
            _bees = state.GetComponentLookup<Dead>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            _bees.Update(ref state);
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var allEntities = _allEntities.ToEntityArray(Allocator.Temp);
                var target = allEntities[_random.NextInt(allEntities.Length)];
                _bees.SetComponentEnabled(target, true);
            }
            
            var dt = SystemAPI.Time.DeltaTime;
            
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

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