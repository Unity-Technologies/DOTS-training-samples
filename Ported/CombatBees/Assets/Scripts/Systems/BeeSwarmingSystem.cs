using System;
using Components;
using DefaultNamespace;
using Systems.Particles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public struct ResourceClaim
    {
        public Entity Resource;
        public Entity Bee;
        public bool IsClaiming;
    }

    [BurstCompile]
    partial struct SwarmJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> AllyBees;
        [ReadOnly] public NativeArray<Entity> EnemyBees;
        [ReadOnly] public NativeArray<Entity> Resources;
        [ReadOnly] public ComponentLookup<Dead> DeadLookup;
        [ReadOnly] public ComponentLookup<LocalToWorldTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<ResourceGatherable> ResourceGatherableLookup;
        [ReadOnly] public ComponentLookup<Resource> ResourceLookup;
        public NativeList<ResourceClaim>.ParallelWriter ResourceClaims;
        public EntityCommandBuffer.ParallelWriter ECB;
        public BeeConfig Config;
        public float Dt;
        public uint Seed;

        void Execute([EntityInQueryIndex] int index, in Entity entity, ref Bee bee, ref Physical physical)
        {
            var random = Random.CreateFromIndex((uint)index + Seed);

            var randomInUnitSphere = random.NextFloat3Direction() * math.pow(random.NextFloat(), 1f / 3f);
            physical.Velocity += randomInUnitSphere * (bee.Team.Jitter * Dt);
            physical.Velocity *= (1f - bee.Team.Damping);

            if (AllyBees.Length != 0)
            {
                var randomBee1 = AllyBees[random.NextInt(AllyBees.Length)];
                var randomBee2 = AllyBees[random.NextInt(AllyBees.Length)];

                //Attractive swarming
                UpdateVelocity(ref physical, TransformLookup[randomBee1].Value.Position, 1, bee.Team.TeamAttraction, false);
                //Repelling swarming
                UpdateVelocity(ref physical, TransformLookup[randomBee2].Value.Position, -1,
                    0.8f * bee.Team.TeamAttraction, false);
            }

            if (bee.State == Beehaviors.Idle)
            {
                //Idle stuff
                var randomValue = random.NextFloat();

                if (randomValue < bee.Team.TeamAggression)
                {
                    if (EnemyBees.Length == 0)
                        return;

                    var enemyBee = EnemyBees[random.NextInt(EnemyBees.Length - 1)];

                    bee.State = Beehaviors.EnemySeeking;
                    bee.EntityTarget = enemyBee;
                }
                else
                {
                    if (Resources.Length == 0)
                        return;

                    var resourceEntity = Resources[random.NextInt(Resources.Length - 1)];

                    var resource = ResourceLookup[resourceEntity];

                    if (resource.Holder != Entity.Null)
                    {
                        if (resource.TeamNumber == bee.Team.TeamNumber)
                        {
                            bee.EntityTarget = Entity.Null;
                            bee.State = Beehaviors.Idle;
                        }
                        else
                        {
                            bee.EntityTarget = resource.Holder;
                            bee.State = Beehaviors.EnemySeeking;
                        }
                    }
                    else
                    {
                        bee.State = Beehaviors.ResourceSeeking;
                        bee.EntityTarget = resourceEntity;
                    }
                }
            }
            else if (bee.State == Beehaviors.EnemySeeking)
            {
                //Enemy seeking stuff
                var isTargetDead = !DeadLookup.TryGetComponent(bee.EntityTarget, out var asd) ||
                                   DeadLookup.IsComponentEnabled(bee.EntityTarget);
                if (isTargetDead)
                {
                    bee.State = Beehaviors.Idle;
                    bee.EntityTarget = Entity.Null;
                    return;
                }

                var delta = TransformLookup[bee.EntityTarget].Value.Position -
                            TransformLookup[entity].Value.Position;
                float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

                if (sqrDist > bee.Team.AttackDistance * bee.Team.AttackDistance)
                {
                    UpdateVelocity(ref physical, TransformLookup[bee.EntityTarget].Value.Position, 1,
                        bee.Team.ChaseForce);
                }
                else
                {
                    UpdateVelocity(ref physical, TransformLookup[bee.EntityTarget].Value.Position, 1,
                        bee.Team.AttackForce);

                    if (sqrDist < bee.Team.HitDistance * bee.Team.HitDistance)
                    {
                        //may need to set the resources holder to null here if they have one
                        ParticleBuilder.SpawnParticleEntity(ECB, index, random.NextUInt(),
                            Config.BloodParticlePrefab,
                            physical.Position,
                            ParticleType.Blood, -physical.Velocity, 20f, 5);

                        ECB.SetComponentEnabled<Dead>(index, bee.EntityTarget, true);
                        bee.EntityTarget = Entity.Null;
                        bee.State = Beehaviors.Idle;
                        physical.Velocity = 0f;
                    }
                }
            }
            else if (bee.State == Beehaviors.ResourceSeeking)
            {
                //Resource seeking state

                if (!ResourceLookup.TryGetComponent(bee.EntityTarget, out var targetResource))
                {
                    bee.State = Beehaviors.Idle;
                    return;
                }

                if (targetResource.Holder != Entity.Null)
                {
                    bee.State = Beehaviors.Idle;
                    return;
                }

                var gatherable = ResourceGatherableLookup.TryGetComponent(bee.EntityTarget, out var _) ||
                                 ResourceGatherableLookup.IsComponentEnabled(bee.EntityTarget);
                if (!gatherable)
                {
                    bee.State = Beehaviors.Idle;
                    return;
                }

                var delta = TransformLookup[bee.EntityTarget].Value.Position - TransformLookup[entity].Value.Position;
                var sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

                if (sqrDist > bee.Team.GrabDistance * bee.Team.GrabDistance)
                {
                    UpdateVelocity(ref physical, TransformLookup[bee.EntityTarget].Value.Position, 1,
                        bee.Team.ChaseForce);
                }
                else
                {
                    ResourceClaims.AddNoResize(new ResourceClaim
                    {
                        Resource = bee.EntityTarget,
                        Bee = entity,
                        IsClaiming = true,
                    });
                }
                // TODO if resource holder is enemy bee, become aggressive to it
                // TODO if resource holder is ally bee, go to idle
            }
            else if (bee.State == Beehaviors.ResourceGathering)
            {
                UpdateVelocity(ref physical, bee.Team.HivePosition, 1, bee.Team.CarryForce);

                if (Field.InTeamArea(physical.Position.x, bee.Team.HivePosition))
                {
                    ResourceClaims.AddNoResize(new ResourceClaim
                    {
                        Resource = bee.EntityTarget,
                        Bee = entity,
                        IsClaiming = false,
                    });

                    // TJA - shouldn't this happen once the resource hits the ground?
                    SpawnSomeBees(ECB, index, Config.BeePrefab, Config.BeesPerResource, Config.MinBeeSize,
                        Config.MaxBeeSize, Config.Stretch, bee.Team, random);

                    bee.State = Beehaviors.Idle;
                    
                    physical.Velocity = 0f;
                }
            }
        }

        private void SpawnSomeBees(EntityCommandBuffer.ParallelWriter ecb,
            int index, Entity beePrefab, int beesToSpawn, float minBeeSize, float maxBeeSize, float stretch, Team team,
            Random random)
        {
            var bees = CollectionHelper.CreateNativeArray<Entity>(beesToSpawn, Allocator.Temp);
            ecb.Instantiate(index, beePrefab, bees);

            foreach (var bee in bees)
            {
                var uniformScaleTransform = new UniformScaleTransform
                {
                    Position = random.NextFloat3(team.MinBounds, team.MaxBounds),
                    Rotation = quaternion.identity,
                    Scale = random.NextFloat(minBeeSize, maxBeeSize)
                };
                ecb.SetComponent(index, bee, new LocalToWorldTransform
                {
                    Value = uniformScaleTransform
                });
                ecb.AddComponent(index, bee, new PostTransformMatrix());
                ecb.SetComponent(index, bee, new URPMaterialPropertyBaseColor
                {
                    Value = team.Color
                });
                ecb.SetComponent(index, bee, new Bee
                {
                    Scale = uniformScaleTransform.Scale,
                    Team = team
                });
                ecb.AddComponent(index, bee, new Physical
                {
                    Position = uniformScaleTransform.Position,
                    Velocity = float3.zero,
                    IsFalling = false,
                    Collision = Physical.FieldCollisionType.Bounce,
                    Stretch = stretch,
                    SpeedModifier = .5f + random.NextFloat() * .5f
                });
                ecb.AddSharedComponent(index, bee, new TeamIdentifier
                {
                    TeamNumber = team.TeamNumber
                });

                ecb.AddComponent(index, bee, new Dead()
                {
                    DeathTimer = 2f
                });

                ecb.SetComponentEnabled<Dead>(index, bee, false);
            }
        }

        void UpdateVelocity(ref Physical originPhysical, float3 target, int sign, float power, bool normalize = true)
        {
            float3 delta = target - originPhysical.Position;
            if (normalize)
                delta = math.normalize(delta);

            float dist = math.lengthsq(delta);

            if (dist > 0f)
            {
                var velocity = originPhysical.Velocity;
                velocity += delta * (power * Dt) * sign;

                originPhysical.Velocity = velocity;
            }
        }
    }

    [BurstCompile]
    public struct ClaimingJob : IJob
    {
        public NativeList<ResourceClaim> ResourceClaims1;
        public NativeList<ResourceClaim> ResourceClaims2;
        public ComponentLookup<Resource> ResourceLookup;
        public ComponentLookup<Bee> BeeLookup;
        public ComponentLookup<Physical> PhysicalLookup;
        public EntityCommandBuffer Ecb;


        private void ProcessResourceClaimFromList(NativeList<ResourceClaim> list, int i)
        {
            if (i < list.Length)
            {
                var claim = list[i];
                
                //if(ResourceLookup)

                var resource = ResourceLookup.GetRefRWOptional(claim.Resource, isReadOnly: false);
                var bee = BeeLookup.GetRefRWOptional(claim.Bee, isReadOnly: false);
 
                if (claim.IsClaiming)
                {
                    if (resource.ValueRO.Holder == Entity.Null)
                    {
                        bee.ValueRW.State = Beehaviors.ResourceGathering;
                        resource.ValueRW.Holder = claim.Bee;
                        resource.ValueRW.TeamNumber = bee.ValueRO.Team.TeamNumber;
                        //ALX: Set the resource underneath as claimable if applicable (stacks)
                        if (resource.ValueRO.ResourceUnder != Entity.Null)
                        {
                            Ecb.SetComponentEnabled<ResourceGatherable>(resource.ValueRO.ResourceUnder, true);
                        }
                    }
                }
                else
                {
                    var resourcePhysical = PhysicalLookup.GetRefRWOptional(claim.Resource, isReadOnly: false);
                    if (resourcePhysical.IsValid)
                        return;
                    
                    bee.ValueRW.State = Beehaviors.Idle;
                    bee.ValueRW.EntityTarget = Entity.Null;
                    resource.ValueRW.Holder = Entity.Null;
                    resourcePhysical.ValueRW.IsFalling = true;
                    resourcePhysical.ValueRW.Velocity *= 0.5f;
                    Ecb.SetComponentEnabled<ResourceGatherable>(claim.Resource, false);
                    resource.ValueRW.StackState = StackState.InProgress;
                    // ALX: Purposely not resetting TeamNumber so that we can eventually use it to spawn the right
                    // bees when the resource lands in team area
                    
                    
                }
            }
        }

        public void Execute()
        {
            for (int i = 0; i < ResourceClaims1.Length || i < ResourceClaims2.Length; ++i)
            {
                ProcessResourceClaimFromList(ResourceClaims1, i);
                ProcessResourceClaimFromList(ResourceClaims2, i);
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
        [ReadOnly] private ComponentLookup<LocalToWorldTransform> _transformLookup;
        [ReadOnly] private ComponentLookup<ResourceGatherable> _resourceGatherableLookup;
        [ReadOnly] private ComponentLookup<Resource> _resourceLookup;
        private ComponentLookup<Resource> _RWresourceLookup;
        private ComponentLookup<Bee> _beeLookup;
        private ComponentLookup<Physical> _physicalLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Bee>();

            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<Bee, TeamIdentifier, Physical>().WithNone<Dead>();
            beeQuery = state.GetEntityQuery(builder);
            beeQuery.SetSharedComponentFilter(new TeamIdentifier { TeamNumber = 1 });

            resourceQuery = SystemAPI.QueryBuilder().WithAll<Resource>().WithNone<Dead>().Build();

            _random = Random.CreateFromIndex(4000);
            state.RequireForUpdate<BeeConfig>();

            _deadLookup = state.GetComponentLookup<Dead>();
            _transformLookup = state.GetComponentLookup<LocalToWorldTransform>();
            _resourceGatherableLookup = state.GetComponentLookup<ResourceGatherable>();
            _resourceLookup = state.GetComponentLookup<Resource>();
            _RWresourceLookup = state.GetComponentLookup<Resource>();
            _beeLookup = state.GetComponentLookup<Bee>();
            _physicalLookup = state.GetComponentLookup<Physical>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _deadLookup.Update(ref state);
            _transformLookup.Update(ref state);
            _resourceGatherableLookup.Update(ref state);
            _resourceLookup.Update(ref state);
            _RWresourceLookup.Update(ref state);
            _beeLookup.Update(ref state);
            _physicalLookup.Update(ref state);

            var dt = SystemAPI.Time.DeltaTime;
            var beeConfig = SystemAPI.GetSingleton<BeeConfig>();

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var parallelEcb = ecb.AsParallelWriter();

            beeQuery.SetSharedComponentFilter(new TeamIdentifier { TeamNumber = 1 });
            var team1Bees = beeQuery.ToEntityArray(Allocator.TempJob);

            beeQuery.SetSharedComponentFilter(new TeamIdentifier { TeamNumber = 2 });
            var team2Bees = beeQuery.ToEntityArray(Allocator.TempJob);

            var resources = resourceQuery.ToEntityArray(Allocator.TempJob);

            var seed1 = _random.NextUInt();
            var seed2 = _random.NextUInt();

            var resourceClaims1 = new NativeList<ResourceClaim>(beeConfig.BeesToSpawn, Allocator.TempJob);
            var resourceClaims2 = new NativeList<ResourceClaim>(beeConfig.BeesToSpawn, Allocator.TempJob);

            var team1Job = new SwarmJob
            {
                AllyBees = team1Bees,
                Dt = dt,
                Seed = seed1,
                TransformLookup = _transformLookup,
                DeadLookup = _deadLookup,
                EnemyBees = team2Bees,
                Resources = resources,
                ECB = parallelEcb,
                ResourceLookup = _resourceLookup,
                ResourceGatherableLookup = _resourceGatherableLookup,
                ResourceClaims = resourceClaims1.AsParallelWriter(),
                Config = beeConfig
            };

            ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            parallelEcb = ecb.AsParallelWriter();

            var team2Job = new SwarmJob
            {
                AllyBees = team2Bees,
                Dt = dt,
                Seed = seed2,
                TransformLookup = _transformLookup,
                DeadLookup = _deadLookup,
                EnemyBees = team1Bees,
                Resources = resources,
                ECB = parallelEcb,
                ResourceLookup = _resourceLookup,
                ResourceGatherableLookup = _resourceGatherableLookup,
                ResourceClaims = resourceClaims2.AsParallelWriter(),
                Config = beeConfig
            };

            beeQuery.SetSharedComponentFilter(new TeamIdentifier { TeamNumber = 2 });
            var team2Handle = team2Job.ScheduleParallel(beeQuery, state.Dependency);

            beeQuery.SetSharedComponentFilter(new TeamIdentifier { TeamNumber = 1 });
            var team1Handle = team1Job.ScheduleParallel(beeQuery, state.Dependency);

            var claimsJob = new ClaimingJob
            {
                ResourceClaims1 = resourceClaims1,
                ResourceClaims2 = resourceClaims2,
                ResourceLookup = _RWresourceLookup,
                BeeLookup = _beeLookup,
                PhysicalLookup = _physicalLookup,
                Ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
            };

            var claimHandle = claimsJob.Schedule(JobHandle.CombineDependencies(team1Handle, team2Handle));

            state.Dependency = claimHandle;

            resourceClaims2.Dispose(state.Dependency);
            resourceClaims1.Dispose(state.Dependency);
            resources.Dispose(state.Dependency);
            team2Bees.Dispose(state.Dependency);
            team1Bees.Dispose(state.Dependency);
        }
    }
}