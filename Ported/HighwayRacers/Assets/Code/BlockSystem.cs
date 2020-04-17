using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateAfter(typeof(PercentCompleteSystem))]
public class BlockSystem : SystemBase
{
    private EntityQuery agentQuery;

    private EntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Used to describe entities that can be collected.
        agentQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<LaneAssignment>(),
                ComponentType.ReadOnly<PercentComplete>(),
                ComponentType.ReadOnly<Speed>(),
            }
        });
        agentQuery.AddSharedComponentFilter(new LaneAssignment());  // TODO is this going to stop system from ever running?

        entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        List<LaneAssignment> lanes = new List<LaneAssignment>();
        EntityManager.GetAllUniqueSharedComponentData<LaneAssignment>(lanes);
        
        foreach (LaneAssignment lane in lanes)
        {
            agentQuery.SetSharedComponentFilter(lane);
            
            // get all percents, speeds, and Entity in lane
            JobHandle percentCompletesHandle;
            var percentCompletes =
                agentQuery.ToComponentDataArrayAsync<PercentComplete>(Allocator.TempJob, out percentCompletesHandle);
            var speeds =
                agentQuery.ToComponentDataArrayAsync<Speed>(Allocator.TempJob, out var speedHandle);
            var agentEntities = agentQuery.ToEntityArrayAsync(Allocator.TempJob, out var agentEntitiesHandle);
            var dep = JobHandle.CombineDependencies(percentCompletesHandle, speedHandle, agentEntitiesHandle);
            dep = JobHandle.CombineDependencies(Dependency, dep);
            
            EntityCommandBuffer.Concurrent ecb = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            dep = Entities
                // Used to help mark the lambda in the profiler.
                .WithName("block_system")
                // Only get entities that have a BlockSpeed component.
                .WithNone<BlockSpeed>()
                .WithDeallocateOnJobCompletion(percentCompletes)
                .WithDeallocateOnJobCompletion(speeds)
                .WithDeallocateOnJobCompletion(agentEntities)
                .WithSharedComponentFilter<LaneAssignment>(lane)
                // Iterate through each entity with the following:
                .ForEach((Entity entity, int nativeThreadIndex, in MinimumDistance minimumDistance,
                    in PercentComplete percentComplete) =>
                {
                    // Cache the minimum distance, which will be used for a depth test.
                    var minimumDistanceTemp = minimumDistance.Value;

                    // Speed of zero is invalid. Which means that there is no other agent blocking this agent.
                    var blockSpeed = new BlockSpeed {Value = 0};

                    // For every agent on the board, perform the following:
                    for (int i = 0; i < agentEntities.Length; ++i)
                    {
                        // If the index matches this agent, then: 
                        if (entity.Index == agentEntities[i].Index)
                        {
                            // Do not proceed with this agent.
                            continue;
                        }

                        // Get the distance of between this agent and the other agent, with agents in front yielding a positive value.
                        float distance = percentCompletes[i].Value - percentComplete.Value;

                        // If the other agent is behind this agent, then:
                        if (distance < 0)
                        {
                            // Recalculate the distance in case od modulo wrap around.
                            distance = 1 - (percentComplete.Value - percentCompletes[i].Value);
                        }

                        // If the distance is within the minimum distance between two agents, then:
                        if (distance < minimumDistanceTemp)
                        {
                            // Update the minimum distance and make the speed matc the closer agent.
                            minimumDistanceTemp = distance;
                            blockSpeed.Value = speeds[i].Value;
                        }
                    }

                    // If there is a valid block speed value, then:
                    if (math.abs(blockSpeed.Value) > float.Epsilon)
                    {
                        // Add a BlockSpeed component with the speed of the car that is ahead of it.
                        ecb.AddComponent(nativeThreadIndex, entity, blockSpeed);
                        ecb.RemoveComponent<OvertakeTag>(nativeThreadIndex, entity);
                    }
                }).ScheduleParallel(dep);
            
            entityCommandBufferSystem.AddJobHandleForProducer(dep);
            
            Dependency = JobHandle.CombineDependencies(Dependency, dep);
        }
    }
}
