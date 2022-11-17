using System;
using System.Collections;
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
        /// Only query for stacks that might be ready to land on floor
        _resourcesQuery = SystemAPI.QueryBuilder().WithAll<Resource, Physical, StackInProgress>().WithNone<ResourceGatherable>().Build();
        _physicaLookup = state.GetComponentLookup<Physical>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;
        _physicaLookup.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var carryingJob = new CarriedJob()
        {
            PhysicalLookup = _physicaLookup
        };

        var carryHandle = carryingJob.Schedule(state.Dependency);

        var parallelStackingJob = new ParallelStackingJob()
        {
            ECB = ecb.AsParallelWriter(),
        };

        var parallelStackingJobHandle = parallelStackingJob.ScheduleParallel(_resourcesQuery, carryHandle);
        parallelStackingJobHandle.Complete();
    }
    
    [WithAll(typeof(Resource), typeof(Physical), typeof(StackInProgress))]
    [WithNone(typeof(ResourceGatherable))]
    [BurstCompile]
    partial struct ParallelStackingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        void Execute([EntityInQueryIndex] int index, Entity entity, ref Physical physical, Resource resource)
        {
            if (physical.Position.y <= Field.GroundLevel && resource.Holder == Entity.Null)
            {
                physical.IsFalling = false;
                physical.Velocity = float3.zero;
                physical.Position.y = Field.GroundLevel;
                ECB.SetComponentEnabled<ResourceGatherable>(index, entity, true);
            }
        }
    }

    [WithAll(typeof(Resource), typeof(Physical))]
    [BurstCompile]
    partial struct CarriedJob : IJobEntity
    {
        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<Physical> PhysicalLookup;

        void Execute([EntityInQueryIndex] int index, ref Resource resource, ref Physical physical)
        {
            if (resource.Holder != Entity.Null)
            {
                var holderPhysical = PhysicalLookup[resource.Holder];

                physical.Velocity = holderPhysical.Velocity;
                var delta = holderPhysical.Position - physical.Position;
                float attract = math.max(0, math.lengthsq(delta) - 1f);
                physical.Velocity += delta * (attract * 0.01f);
            }

        }
    }

}