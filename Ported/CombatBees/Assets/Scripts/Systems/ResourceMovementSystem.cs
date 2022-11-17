using System;
using System.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public partial struct ResourceMovementSystem : ISystem
{
    private EntityQuery _resourcesQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        /// Only query for stacks that might be ready to land on floor
        _resourcesQuery = SystemAPI.QueryBuilder().WithAll<Resource, Physical, StackInProgress>().WithNone<ResourceGatherable>().Build();
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

        var parallelStackingJob = new ParallelStackingJob()
        {
            ECB = ecb.AsParallelWriter(),
        };

        var parallelStackingJobHandle = parallelStackingJob.ScheduleParallel(_resourcesQuery, state.Dependency);
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
            if (physical.Position.y <= Field.GroundLevel)
            {
                physical.IsFalling = false;
                physical.Velocity = float3.zero;
                physical.Position.y = Field.GroundLevel;
                ECB.SetComponentEnabled<ResourceGatherable>(index, entity, true);
            }
        }
    }
}