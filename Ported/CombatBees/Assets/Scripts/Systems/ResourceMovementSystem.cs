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
    private EntityQuery _gatherableResourcesQuery;
    private EntityQuery _stackInProgressQuery;
    private ComponentLookup<Resource> _resourcesLookup;
    private ComponentLookup<ResourceGatherable> _resourceGatherablesLookup;
    private ComponentLookup<Physical> _physicaLookup;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _gatherableResourcesQuery = SystemAPI.QueryBuilder().WithAll<ResourceGatherable>().Build();
        _stackInProgressQuery = SystemAPI.QueryBuilder().WithAll<Resource, StackInProgress>().Build();

        _resourcesLookup = state.GetComponentLookup<Resource>();
        _resourceGatherablesLookup = state.GetComponentLookup<ResourceGatherable>();
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
        _resourcesLookup.Update(ref state);
        _resourceGatherablesLookup.Update(ref state);
        _physicaLookup.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var gatherables = _gatherableResourcesQuery.ToEntityArray(Allocator.TempJob);
        var stackInProgress = _stackInProgressQuery.ToEntityArray(Allocator.TempJob);

        // var comp = new AxesXZComparer();
        // comp.SetLookupReadOnly(_physicaLookup);
        // SortJob<Entity, AxesXZComparer> sortJob = gatherables.SortJob(comp);

        // JobHandle sortHandle = sortJob.Schedule();

        var carryingJob = new CarriedJob()
        {
            PhysicalLookup = _physicaLookup
        };

        var carryHandle = carryingJob.Schedule(state.Dependency);

        var stackingJob = new StackingJob()
        {
            ECB = ecb,
            Gatherables = gatherables,
            ResourceGatherablesLookup = _resourceGatherablesLookup,
            ResourceLookup = _resourcesLookup,
            PhysicalLookup = _physicaLookup
        };

        // Only try to fix stacking if there are falling resources in progress
        if (stackInProgress.Length > 0)
        {
            // Todo Fix sorting
            // var stackHandle = stackingJob.Schedule(sortHandle);
            // stackHandle.Complete();
            
             state.Dependency = stackingJob.Schedule(carryHandle);
        }


        gatherables.Dispose(state.Dependency);
        stackInProgress.Dispose(state.Dependency);
    }

    [BurstCompile]
    partial struct SortJob : IJobEntity
    {
    }

    [WithAll(typeof(Resource), typeof(Physical))]
    [BurstCompile]
    partial struct StackingJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> Gatherables;

        public EntityCommandBuffer ECB;
        public ComponentLookup<ResourceGatherable> ResourceGatherablesLookup;
        public ComponentLookup<Resource> ResourceLookup;
        public ComponentLookup<Physical> PhysicalLookup;

        void Execute([EntityInQueryIndex] int index, Entity entity, in StackInProgress stackInProgress)
        {
            Physical thisPhysical = PhysicalLookup[entity];
            Resource thisResource = ResourceLookup[entity];

            if (thisPhysical.Position.y <= Field.GroundLevel && thisResource.IsTopOfStack
                                                             && !ResourceGatherablesLookup.IsComponentEnabled(entity))
            {
                thisPhysical.IsFalling = false;
                thisPhysical.Velocity = float3.zero;
                thisPhysical.Position.y = Field.GroundLevel;
                ECB.SetComponent(entity, thisPhysical);
                ECB.SetComponentEnabled<ResourceGatherable>(entity, true);
                ResourceGatherablesLookup.SetComponentEnabled(entity, true);
                ECB.SetComponentEnabled<StackInProgress>(entity, false);

                return;
            }

            if (!thisPhysical.IsFalling)
            {
                ECB.SetComponentEnabled<StackInProgress>(entity, false);
            }

            foreach (var gatherableEntity in Gatherables)
            {
                Physical otherPhysical = PhysicalLookup[gatherableEntity];

                int compareX = math.round(thisPhysical.Position.x).CompareTo(math.round(otherPhysical.Position.x));
                int compareZ = math.round(thisPhysical.Position.z).CompareTo(math.round(otherPhysical.Position.z));

                // Todo fix
                // Sorted so if we are past our desired comparison then stop looping
                // if (compareX > 0 || compareZ > 0)
                // {
                //     break;
                // }
                // else
                if (!(compareX == 0 && compareZ == 0))
                {
                    continue;
                }

                if (ResourceGatherablesLookup.IsComponentEnabled(gatherableEntity))
                {
                    if (!ResourceGatherablesLookup.IsComponentEnabled(entity)
                        && thisPhysical.IsFalling)
                    {
                        Resource otherResource = ResourceLookup[gatherableEntity];
                        if (thisPhysical.Position.y <
                            otherPhysical.Position.y + ResourceSpawningSystem.RESOURCE_OBJ_HEIGHT)
                        {
                            thisPhysical.Position.y = otherPhysical.Position.y +
                                                      ResourceSpawningSystem.RESOURCE_OBJ_HEIGHT;

                            ResourceGatherablesLookup.SetComponentEnabled(entity, true);

                            thisPhysical.IsFalling = false;
                            thisPhysical.Velocity = float3.zero;
                            ResourceGatherablesLookup.SetComponentEnabled(gatherableEntity, false);

                            ECB.SetComponentEnabled<ResourceGatherable>(entity, true);
                            ECB.SetComponentEnabled<ResourceGatherable>(gatherableEntity, false);
                            ECB.SetComponent(entity, thisPhysical);

                            otherResource.IsTopOfStack = false;
                            ECB.SetComponent(gatherableEntity, otherResource);

                            break;
                        }
                        // else if (ResourceGatherablesLookup.IsComponentEnabled(entity)
                        //          && !thisPhysical.IsFalling
                        //          && matchesGridCell
                        //          && thisPhysical.Position.y <= otherPhysical.Position.y)
                        // {
                        //     ResourceGatherablesLookup.SetComponentEnabled(entity, false);
                        //     thisResource.IsTopOfStack = false;
                        //     ECB.SetComponentEnabled<ResourceGatherable>(entity, true);
                        //     ECB.SetComponent(entity, thisResource);
                        //     break;
                        // }
                    }
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