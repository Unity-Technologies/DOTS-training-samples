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

        entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Allocate an array of all lane assignments in the world.
        var laneAssignments =
            agentQuery.ToComponentDataArrayAsync<LaneAssignment>(Allocator.TempJob, out var laneAssignmentsHandle);

        // Allocate an array of all percents in the world.
        var percentCompletes =
            agentQuery.ToComponentDataArrayAsync<PercentComplete>(Allocator.TempJob, out var percentCompletesHandle);

        // Allocate an array of all speeds in the world.
        var speeds =
            agentQuery.ToComponentDataArrayAsync<Speed>(Allocator.TempJob, out var speedHandle);

        var agentEntities = agentQuery.ToEntityArrayAsync(Allocator.TempJob, out var agentEntitiesHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, laneAssignmentsHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, percentCompletesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, speedHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, agentEntitiesHandle);

        EntityCommandBuffer.Concurrent entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            // Used to help mark the lambda in the profiler.
            .WithName("block_system")
            // Only get entities that have a BlockSpeed component.
            .WithNone<BlockSpeed>()
            // Deallocate the arrays when this job completes.
            .WithDeallocateOnJobCompletion(laneAssignments)
            .WithDeallocateOnJobCompletion(percentCompletes)
            .WithDeallocateOnJobCompletion(speeds)
            .WithDeallocateOnJobCompletion(agentEntities)
            // Iterate through each entity with the following:
            .ForEach((Entity entity, int nativeThreadIndex, in MinimumDistance minimumDistance,
            in PercentComplete percentComplete, in LaneAssignment currentLane) =>
            {
                // Cache the minimum distance, which will be used for a depth test.
                var minimumDistanceTemp = minimumDistance.Value;

                // Speed of zero is invalid. Which means that there is no other agent blocking this agent.
                var blockSpeed = new BlockSpeed { Value = 0 };

                // For every agent on the board, perform the following:
                for (int i = 0; i < agentEntities.Length; ++i)
                {
                    // If the index matches this agent, then: 
                    if (entity.Index != agentEntities[i].Index)
                    {
                        // Do not proceed with this agent.
                        continue;
                    }

                    // If the other agent is not in the same lane, then:
                    if (currentLane.Value != laneAssignments[i].Value)
                    {
                        // Do not proceed with this agent.
                        continue;
                    }

                    // Otherwise, other agent is ahead of agent in the same lane.

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
                    entityCommandBuffer.AddComponent(nativeThreadIndex, entity, blockSpeed);
                }
            })
            .ScheduleParallel();

        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
