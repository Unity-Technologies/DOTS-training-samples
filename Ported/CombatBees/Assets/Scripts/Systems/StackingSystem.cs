using Components;
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
    private EntityQuery _stacksToFixQuery;
    private ComponentLookup<Resource> _resourcesLookup;
    private ComponentLookup<Physical> _physicaLookup;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _gatherableResourcesQuery = SystemAPI.QueryBuilder().WithAll<Resource, Physical, ResourceGatherable>().WithNone<StackNeedsFix>().Build();
        _stacksToFixQuery = SystemAPI.QueryBuilder().WithAll<Resource, Physical, StackNeedsFix>().Build();

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

        var stacksToFix = _stacksToFixQuery.ToEntityArray(Allocator.TempJob);

        // Early out if there are no stacks to fix
        if (stacksToFix.Length > 0)
        {
            var gatherables = _gatherableResourcesQuery.ToEntityArray(Allocator.TempJob);

            var fixStacksEcb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var fixStacksJob = new FixStacksJob()
            {
                ECB = fixStacksEcb,
                Resources = gatherables,
                StacksToFix = stacksToFix,
                ResourceLookup = _resourcesLookup,
                PhysicalLookup = _physicaLookup,
            };

            JobHandle jobHandle = fixStacksJob.Schedule(state.Dependency);
            state.Dependency = jobHandle;
            jobHandle.Complete();

            gatherables.Dispose(state.Dependency);
        }

        stacksToFix.Dispose(state.Dependency);
    }

    [BurstCompile]
    partial struct FixStacksJob : IJob
    {
        [ReadOnly] public NativeArray<Entity> Resources;
        [ReadOnly] public NativeArray<Entity> StacksToFix;
        public EntityCommandBuffer ECB;
        public ComponentLookup<Resource> ResourceLookup;
        public ComponentLookup<Physical> PhysicalLookup;
        
        public void Execute()
        {
            // Chances are there are not a lot of stacks to fix
            NativeHashMap<int2, Entity> stackTopEntities = new NativeHashMap<int2, Entity>(Field.GridSize, Allocator.Temp);

            foreach (var resourceToStack in StacksToFix)
            {
                var resource = ResourceLookup[resourceToStack];
                var physical = PhysicalLookup[resourceToStack];

                // Check if actual position matches GridIndex and update it otherwise
                // This handles edge case where a bee is killed and drops the resource elsewhere
                int2 actualGridIndex = ResourceSpawningSystem.GetGridIndex(physical.Position);
                if (!resource.GridIndex.Equals(actualGridIndex))
                {
                    resource.GridIndex = actualGridIndex;
                }
                
                stackTopEntities[resource.GridIndex] = Entity.Null;
            }

            /// Cache the top-most entity rather than the float y-value, so that we can disable the ResourceGatherable later
            /// if it doesn't end up being the top of stack anymore.
            foreach (var resourceEntity in Resources)
            {
                var resource = ResourceLookup[resourceEntity];

                // Only get calculate high if this resource is relevant to new stack
                if (!stackTopEntities.ContainsKey(resource.GridIndex) || resource.StackState != StackState.StackFixed)
                {
                    continue;
                }

                var physical = PhysicalLookup[resourceEntity];
                float value = physical.Position.y;
                if (stackTopEntities.ContainsKey(resource.GridIndex) && stackTopEntities[resource.GridIndex] != Entity.Null)
                {
                    var currentTallest = PhysicalLookup[stackTopEntities[resource.GridIndex]].Position.y;
                    if (value <= currentTallest)
                    {
                        continue;
                    }
                }
                stackTopEntities[resource.GridIndex] = resourceEntity;
            }
            
            NativeHashMap<int2, int> newStacks = new NativeHashMap<int2, int>(StacksToFix.Length, Allocator.Temp);
            foreach (var resourceToStack in StacksToFix)
            {
                var resource = ResourceLookup[resourceToStack];
                var physical = PhysicalLookup[resourceToStack];

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
                ECB.SetComponentEnabled<StackNeedsFix>(resourceToStack, false);

                /// The previous stack top is now underneath so disable the Resource Gatherable
                /// and update the ResourceUnder reference
                if (stackTopEntities.ContainsKey(resource.GridIndex) && stackTopEntities[resource.GridIndex] != Entity.Null)
                {
                    Entity resourceUnder = stackTopEntities[resource.GridIndex];
                    ECB.SetComponentEnabled<ResourceGatherable>(resourceUnder, false);
                    resource.ResourceUnder = resourceUnder;
                }
                ECB.SetComponent<Resource>(resourceToStack, resource);
            }
    
            stackTopEntities.Dispose();
            newStacks.Dispose();
        }
        
        float GetYOffset(NativeHashMap<int2, Entity> stackTopEntities, int2 gridIndex)
        {
            if (stackTopEntities.ContainsKey(gridIndex) && stackTopEntities[gridIndex] != Entity.Null)
            {
                var physical = PhysicalLookup[stackTopEntities[gridIndex]];
                return physical.Position.y;
            }
            
            return Field.GroundLevel - ResourceSpawningSystem.RESOURCE_OBJ_HEIGHT;
        }
    }
}