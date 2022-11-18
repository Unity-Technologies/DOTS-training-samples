using System;
using System.Collections;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct ResourceMovementSystem : ISystem
{
    private EntityQuery _resourcesQuery;
    private ComponentLookup<Physical> _physicaLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();

        /// Only query for stacks that might be ready to land on floor
        _resourcesQuery = SystemAPI.QueryBuilder().WithAll<Resource, Physical>().WithAny<ResourceGatherable, Claimed>().WithNone<Dead>().Build();
        _physicaLookup = state.GetComponentLookup<Physical>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var beeConfig = SystemAPI.GetSingleton<BeeConfig>();
        _physicaLookup.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var carryingJob = new CarriedJob()
        {
            PhysicalLookup = _physicaLookup,
            EM = state.EntityManager,
        };

        var carryHandle = carryingJob.Schedule(state.Dependency);

        var fallingJob = new ResourceFallingJob()
        {
            ECB = ecb.AsParallelWriter(),
            Config = beeConfig
        };

        var parallelStackingJobHandle = fallingJob.ScheduleParallel(_resourcesQuery, carryHandle);
        parallelStackingJobHandle.Complete();
    }
    
    [WithAll(typeof(Resource), typeof(Physical), typeof(ResourceGatherable))]
    [WithNone(typeof(StackNeedsFix))]
    [BurstCompile]
    partial struct ResourceFallingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public BeeConfig Config;
        
        void Execute([EntityInQueryIndex] int index, Entity entity, ref Physical physical, ref Resource resource)
        {
            if (resource.StackState == StackState.InProgress && physical.Position.y <= Field.GroundLevel && resource.Holder == Entity.Null)
            {
                physical.IsFalling = false;
                physical.Velocity = float3.zero;
                physical.Position.y = Field.GroundLevel;

                bool inTeam1Bounds = Field.InTeamArea(physical.Position.x, Config.Team1.HivePosition);
                bool inTeam2Bounds = Field.InTeamArea(physical.Position.x, Config.Team2.HivePosition);
                if (!inTeam1Bounds && !inTeam2Bounds)
                {
                    // Flag stack for needing fix
                    ECB.SetComponentEnabled<StackNeedsFix>(index, entity, true);
                }
                else
                {
                    // Resource successfully gathered!
                    // Mark resource as dead
                    ECB.SetComponentEnabled<Dead>(index, entity, true);
                    
                    // Spawn bees where the resource landed
                    var random = Unity.Mathematics.Random.CreateFromIndex((uint)index);
                    var team = inTeam1Bounds
                        ? Config.Team1
                        : Config.Team2;
                    SpawnSomeBees(ECB, index, Config.BeePrefab, Config.BeesPerResource, Config.MinBeeSize,
                        Config.MaxBeeSize, Config.Stretch, team, random, physical.Position);
                }
            }
        }

        private void SpawnSomeBees(EntityCommandBuffer.ParallelWriter ecb,
            int index, Entity beePrefab, int beesToSpawn, float minBeeSize, float maxBeeSize, float stretch, Team team,
            Unity.Mathematics.Random random, float3 spawnPosition)
        {
            var bees = CollectionHelper.CreateNativeArray<Entity>(beesToSpawn, Allocator.Temp);
            ecb.Instantiate(index, beePrefab, bees);

            foreach (var bee in bees)
            {
                var uniformScaleTransform = new UniformScaleTransform
                {
                    Position = spawnPosition,
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
    }

    [WithAll(typeof(Resource), typeof(Physical))]
    [WithNone(typeof(Claimed))]
    [BurstCompile]
    partial struct CarriedJob : IJobEntity
    {
        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<Physical> PhysicalLookup;

        public EntityManager EM;

        void Execute([EntityInQueryIndex] int index, ref Resource resource, ref Physical physical)
        {
            if (resource.Holder != Entity.Null)
            {
                if (!EM.Exists(resource.Holder))
                {
                    resource.Holder = Entity.Null;
                    physical.IsFalling = true;
                    resource.StackState = StackState.InProgress;
                    return;
                }
                var holderPhysical = PhysicalLookup[resource.Holder];

                physical.Velocity = holderPhysical.Velocity;
                var delta = holderPhysical.Position - physical.Position;
                float attract = math.max(0, math.lengthsq(delta) - 1f);
                physical.Velocity += delta * (attract * 0.01f);
            }

        }
    }

}