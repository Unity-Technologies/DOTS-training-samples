using System;
using Components;
using DefaultNamespace;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    
    [BurstCompile]
    partial struct SwarmJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> AllyBees;
        [ReadOnly] public NativeArray<Entity> EnemyBees;
        [ReadOnly] public NativeArray<Entity> Resources;
        [ReadOnly] public ComponentLookup<Dead> DeadLookup;
        [ReadOnly] public ComponentLookup<LocalToWorldTransform> TransformLookup;
        public EntityCommandBuffer.ParallelWriter ECB;
        // [ReadOnly] public ComponentLookup<Physical> PhysicalLookup;
        public float Dt;
        public uint Seed;

        void Execute([EntityInQueryIndex] int index, in Entity entity, ref Bee bee, ref Physical physical)
        {
            var random = Random.CreateFromIndex((uint)index + Seed);
            var randomBee1 = AllyBees[random.NextInt(AllyBees.Length)];
            var randomBee2 = AllyBees[random.NextInt(AllyBees.Length)];
            
            //TODO: collisions
            
            //Attractive swarming
            UpdateVelocity(ref physical, TransformLookup[randomBee1], 1, bee.Team.TeamAttraction);
            //Repelling swarming
            UpdateVelocity(ref physical, TransformLookup[randomBee2], -1, 0.8f * bee.Team.TeamAttraction);

            //TODO: states
            
            switch (bee.State)
            {
                case Beehaviors.Idle:
                    //Idle stuff
                    var randomValue = random.NextFloat();
            
                    if (randomValue < bee.Team.TeamAggression)
                    {
                        var enemyBee  = EnemyBees[random.NextInt(EnemyBees.Length - 1)];
            
                        bee.State = Beehaviors.EnemySeeking;
                        bee.EntityTarget = enemyBee;
                    }
                    else
                    {
                        var resource = Resources[random.NextInt(Resources.Length - 1)];
            
                        bee.State = Beehaviors.ResourceGathering;
                        bee.EntityTarget = resource;
                    }
                    
                    break;
                case Beehaviors.EnemySeeking:
                    //Enemy seeking stuff
                    var isTargetDead = DeadLookup.IsComponentEnabled(bee.EntityTarget);
                    if (isTargetDead)
                    {
                        bee.State = Beehaviors.Idle;
                        bee.EntityTarget = Entity.Null;
                        return;
                    }
            
                    var delta = TransformLookup[bee.EntityTarget].Value.Position - TransformLookup[entity].Value.Position;
                    float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
            
                    if (sqrDist > math.pow(bee.Team.AttackDistance, 2))
                    {
                        UpdateVelocity(ref physical, TransformLookup[bee.EntityTarget], 1, bee.Team.ChaseForce);
                    }
                    else
                    {
                        UpdateVelocity(ref physical, TransformLookup[bee.EntityTarget], 1, bee.Team.AttackForce);

                        if (sqrDist < math.pow(bee.Team.HitDistance, 2))
                        {
                            //spawn particle
                            
                        }

                    }
            
                    
                    
                    break;
                case Beehaviors.ResourceSeeking:
                    //Resource seeking state
                    break;
                case Beehaviors.ResourceGathering:
                    //Resource Gather state
                    break;
            
            }
            
        }

        void UpdateVelocity(ref Physical originPhysical, LocalToWorldTransform target, int sign, float power)
        {
            float3 delta = target.Value.Position - originPhysical.Position;
            float dist = math.length(delta);
            if (dist > 0f)
            {
                var velocity = originPhysical.Velocity;
                velocity += delta * (power * Dt / dist) * sign;
                
                originPhysical.Velocity = velocity;
            }
        }
    }
    
    [BurstCompile]
    public partial struct BeeSwarmingSystem : ISystem
    {
        private Random _random;
        private uint _randomSeed;
        private EntityQuery beeQuery;
        private EntityQuery resourceQuery;
        [ReadOnly] private ComponentLookup<Dead> _deadLookup;
        [ReadOnly] private ComponentLookup<Physical> _physicalLookup;
        [ReadOnly] private ComponentLookup<LocalToWorldTransform> _transformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Bee>();

            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<Bee, TeamIdentifier, Physical>().WithNone<Dead>();
            beeQuery = state.GetEntityQuery(builder);
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 1});

            resourceQuery = SystemAPI.QueryBuilder().WithAll<Resource>().WithNone<Dead>().Build();
            
            _random = Random.CreateFromIndex(4000);
            state.RequireForUpdate<BeeConfig>();

            _deadLookup = state.GetComponentLookup<Dead>();
            _physicalLookup = state.GetComponentLookup<Physical>();
            _transformLookup = state.GetComponentLookup<LocalToWorldTransform>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _deadLookup.Update(ref state);
            _physicalLookup.Update(ref state);
            _transformLookup.Update(ref state);
            
            var dt = SystemAPI.Time.DeltaTime;
            var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var parallelEcb = ecb.AsParallelWriter();
            //
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 1});
            var team1Bees = beeQuery.ToEntityArray(Allocator.TempJob);
            
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 2});
            var team2Bees = beeQuery.ToEntityArray(Allocator.TempJob);

            var resources = resourceQuery.ToEntityArray(Allocator.TempJob);
            
            var seed1 = _random.NextUInt();
            var seed2 = _random.NextUInt();
            
            var team1Job = new SwarmJob
            {
                AllyBees = team1Bees,
                Dt = dt,
                Seed = seed1,
                TransformLookup = _transformLookup,
                DeadLookup = _deadLookup,
                EnemyBees = team2Bees,
                Resources = resources,
                ECB = parallelEcb
            };

            var team2Job = new SwarmJob
            {
                AllyBees = team2Bees,
                Dt = dt,
                Seed = seed2,
                TransformLookup = _transformLookup,
                DeadLookup = _deadLookup,
                EnemyBees = team1Bees,
                Resources = resources,
                ECB = parallelEcb
            };
            
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 2});
            var team2Handle = team2Job.ScheduleParallel(beeQuery, state.Dependency);
            
            beeQuery.SetSharedComponentFilter(new TeamIdentifier{TeamNumber = 1});
            var team1Handle = team1Job.ScheduleParallel(beeQuery, state.Dependency);

            state.Dependency = JobHandle.CombineDependencies(team1Handle, team2Handle);

            team1Bees.Dispose(state.Dependency);
            team2Bees.Dispose(state.Dependency);
            resources.Dispose(state.Dependency);
        }
    }
}