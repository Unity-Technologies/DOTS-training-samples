using System;
using System.Collections;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
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
        _resourcesQuery = SystemAPI.QueryBuilder().WithAll<Resource, Physical, ResourceGatherable>().Build();
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

        var parallelStackingJob = new ResourceFallingJob()
        {
            ECB = ecb.AsParallelWriter(),
            HivePosition_Team1 = beeConfig.Team1.HivePosition,
            HivePosition_Team2 = beeConfig.Team2.HivePosition
        };

        var parallelStackingJobHandle = parallelStackingJob.ScheduleParallel(_resourcesQuery, carryHandle);
        parallelStackingJobHandle.Complete();
    }
    
    [WithAll(typeof(Resource), typeof(Physical), typeof(ResourceGatherable))]
    [BurstCompile]
    partial struct ResourceFallingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        // ALX: Hard-set values for each team
        public float3 HivePosition_Team1;
        public float3 HivePosition_Team2;
        
        void Execute([EntityInQueryIndex] int index, Entity entity, ref Physical physical, ref Resource resource)
        {
            if (resource.StackState == StackState.InProgress && physical.Position.y <= Field.GroundLevel && resource.Holder == Entity.Null)
            {
                physical.IsFalling = false;
                physical.Velocity = float3.zero;
                physical.Position.y = Field.GroundLevel;

                bool withinTeamBounds = Field.InTeamArea(physical.Position.x, HivePosition_Team1)
                                        || Field.InTeamArea(physical.Position.x, HivePosition_Team2);
                if (!withinTeamBounds)
                {
                    // Flag for stacking gatherable
                    resource.StackState = StackState.NeedsFix;
                }
                else
                {
                    // Mark resource as dead
                    ECB.SetComponentEnabled<Dead>(index, entity, true);
                    // ALX Todo spawn bees here instead of swarm system
                }
            }
        }
    }

    [WithAll(typeof(Resource), typeof(Physical))]
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