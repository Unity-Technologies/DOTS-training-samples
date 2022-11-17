using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(ResourceMovementSystem))]
public partial struct StackingSystem : ISystem
{
    private EntityQuery _gatherableResourcesQuery;
    private ComponentLookup<Resource> _resourcesLookup;
    private ComponentLookup<Physical> _physicaLookup;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _gatherableResourcesQuery = SystemAPI.QueryBuilder().WithAll<Resource, Physical, ResourceGatherable>().Build();

        _resourcesLookup = state.GetComponentLookup<Resource>();
        _physicaLookup = state.GetComponentLookup<Physical>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _resourcesLookup.Update(ref state);
        _physicaLookup.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        var gatherables = _gatherableResourcesQuery.ToEntityArray(Allocator.TempJob);

        var fixStacksEcb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var fixStacksJob = new FixStacksJob()
        {
            ECB = fixStacksEcb,
            Resources = gatherables,
            ResourceLookup = _resourcesLookup,
            PhysicalLookup = _physicaLookup,
        };

        JobHandle jobHandle = fixStacksJob.Schedule(state.Dependency);
        state.Dependency = jobHandle;
        jobHandle.Complete();

        gatherables.Dispose(state.Dependency);
    }
    
    [BurstCompile]
    partial struct FixStacksJob : IJob
    {
        [ReadOnly] public NativeArray<Entity> Resources;
        public EntityCommandBuffer ECB;
        public ComponentLookup<Resource> ResourceLookup;
        public ComponentLookup<Physical> PhysicalLookup;
        
        public void Execute()
        {
            /// Cache the top-most entity rather than the float y-value, so that we can disable the ResourceGatherable later
            /// if it doesn't end up being the top of stack anymore.
            NativeHashMap<int2, Entity> stackTopEntities = new NativeHashMap<int2, Entity>(Field.GridSize, Allocator.Temp);
            foreach (var resourceEntity in Resources)
            {
                var resource = ResourceLookup[resourceEntity];
                var physical = PhysicalLookup[resourceEntity];

                if (resource.StackState != StackState.StackFixed)
                {
                    continue;
                }
                
                float value = physical.Position.y;
                if (stackTopEntities.ContainsKey(resource.GridIndex))
                {
                    var currentTallest = PhysicalLookup[stackTopEntities[resource.GridIndex]].Position.y;
                    if (value <= currentTallest)
                    {
                        continue;
                    }
                }
                stackTopEntities[resource.GridIndex] = resourceEntity;
            }
            
            NativeHashMap<int2, int> newStacks = new NativeHashMap<int2, int>(Field.GridSize, Allocator.Temp);
            
            foreach (var resourceToStack in Resources)
            {
                var resource = ResourceLookup[resourceToStack];
                var physical = PhysicalLookup[resourceToStack];

                if (resource.StackState != StackState.NeedsFix)
                {
                    continue;
                }
                
                if (newStacks.ContainsKey(resource.GridIndex))
                {
                    newStacks[resource.GridIndex]++;
                }
                else
                {
                    newStacks[resource.GridIndex] = 1;
                }
                
                physical.Position.y = GetYOffset(stackTopEntities, resource.GridIndex) +
                                      (newStacks[resource.GridIndex] * ResourceSpawningSystem.RESOURCE_OBJ_HEIGHT);
                ECB.SetComponent(resourceToStack, physical);
                resource.StackState = StackState.StackFixed;

                /// The previous stack top is now underneath so disable the Resource Gatherable
                /// and update the ResourceUnder reference
                if (stackTopEntities.ContainsKey(resource.GridIndex))
                {
                    Entity resourceUnder = stackTopEntities[resource.GridIndex];
                    ECB.SetComponentEnabled<ResourceGatherable>(resourceUnder, false);
                    resource.ResourceUnder = resourceUnder;
                    ECB.SetComponent<Resource>(resourceToStack, resource);
                }
            }
    
            stackTopEntities.Dispose();
            newStacks.Dispose();
        }
          
        float GetYOffset(NativeHashMap<int2, Entity> stackTopEntities, int2 gridIndex)
        {
            if (stackTopEntities.ContainsKey(gridIndex))
            {
                var physical = PhysicalLookup[stackTopEntities[gridIndex]];
                return physical.Position.y;
            }
            
            return Field.GroundLevel - ResourceSpawningSystem.RESOURCE_OBJ_HEIGHT;
        }
    }
}