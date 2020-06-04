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

        entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var roadInfo =
            GetSingleton<RoadInfo>(); // TODO assumes one RoadInfo, but will we have multiple with segmentation? 
        

        int count = agentQuery.CalculateEntityCount();
        NativeMultiHashMap<int, PercentComplete> percents =
            new NativeMultiHashMap<int, PercentComplete>(count, Allocator.TempJob);
        NativeMultiHashMap<int, Speed> speeds = new NativeMultiHashMap<int, Speed>(count, Allocator.TempJob);

        Entities.ForEach((in PercentComplete percentComplete, in Speed speed, in LaneAssignment laneAssignment) =>
        {
            percents.Add(laneAssignment.Value, percentComplete);
            speeds.Add(laneAssignment.Value, speed);
        }).Schedule(Dependency).Complete();

        int count0 = percents.CountValuesForKey(0);
        int count1 = percents.CountValuesForKey(1);
        int count2 = percents.CountValuesForKey(2);
        int count3 = percents.CountValuesForKey(3);

        NativeArray<PercentComplete> percents0 = new NativeArray<PercentComplete>(count0, Allocator.TempJob);
        NativeArray<Speed> speeds0 = new NativeArray<Speed>(count0, Allocator.TempJob);
        NativeArray<PercentComplete> percents1 = new NativeArray<PercentComplete>(count1, Allocator.TempJob);
        NativeArray<Speed> speeds1 = new NativeArray<Speed>(count1, Allocator.TempJob);
        NativeArray<PercentComplete> percents2 = new NativeArray<PercentComplete>(count2, Allocator.TempJob);
        NativeArray<Speed> speeds2 = new NativeArray<Speed>(count2, Allocator.TempJob);
        NativeArray<PercentComplete> percents3 = new NativeArray<PercentComplete>(count3, Allocator.TempJob);
        NativeArray<Speed> speeds3 = new NativeArray<Speed>(count3, Allocator.TempJob);

        Job.WithCode(() =>
        {
            for (int i = 0; i < 4; i++)
            {
                var percentList = percents.GetValuesForKey(i);
                var speedList = speeds.GetValuesForKey(i);

                switch (i)
                {
                    case 0:
                        for (int j = 0; j < count0; j++)
                        {
                            speeds0[j] = speedList.Current;
                            speedList.MoveNext();

                            percents0[j] = percentList.Current;
                            percentList.MoveNext();
                        }

                        break;
                    case 1:
                        for (int j = 0; j < count1; j++)
                        {
                            speeds1[j] = speedList.Current;
                            speedList.MoveNext();

                            percents1[j] = percentList.Current;
                            percentList.MoveNext();
                        }

                        break;
                    case 2:
                        for (int j = 0; j < count2; j++)
                        {
                            speeds2[j] = speedList.Current;
                            speedList.MoveNext();

                            percents2[j] = percentList.Current;
                            percentList.MoveNext();
                        }

                        break;
                    case 3:
                        for (int j = 0; j < count3; j++)
                        {
                            speeds3[j] = speedList.Current;
                            speedList.MoveNext();

                            percents3[j] = percentList.Current;
                            percentList.MoveNext();
                        }

                        break;
                }
            }
        }).Run();


        EntityCommandBuffer.Concurrent ecb = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        Entities.WithName("block_system")
            .WithNone<BlockSpeed>()
            .ForEach((Entity entity, int nativeThreadIndex,
                in PercentComplete percentComplete, in LaneAssignment laneAssignment) =>
            {
                // Speed of zero is invalid. Which means that there is no other agent blocking this agent.
                var blockSpeed = new BlockSpeed {Value = 0};

                NativeArray<Speed> speedsLocal =  default;
                NativeArray<PercentComplete> percentsLocal = default;
                switch (laneAssignment.Value)
                {
                    case 0:
                        speedsLocal = speeds0;
                        percentsLocal = percents0;
                        break;
                    case 1:
                        speedsLocal = speeds1;
                        percentsLocal = percents1;
                        break;
                    case 2:
                        speedsLocal = speeds2;
                        percentsLocal = percents2;
                        break;
                    case 3:
                        speedsLocal = speeds3;
                        percentsLocal = percents3;
                        break;
                }

                float minDist = roadInfo.CarSpawningDistancePercent;
                // For every agent on the board, perform the following:
                for (int i = 0; i < speedsLocal.Length; ++i)
                {
                    float otherSpeed = speedsLocal[i].Value;
                    float otherPercent = percentsLocal[i].Value;
                    
                    // skip self
                    if (percentComplete.Value == otherPercent)
                    {
                        continue;
                    }

                    // Get the distance of between this agent and the other agent, with agents in front yielding a positive value.
                    float distance = otherPercent - percentComplete.Value;

                    // If the other agent is behind this agent, then:
                    if (distance < 0)
                    {
                        // Recalculate the distance in case od modulo wrap around.
                        distance = 1 - (percentComplete.Value - otherPercent);
                    }
                    
                    // If the distance is within the minimum distance between two agents, then:
                    if (distance < minDist)
                    {
                        // Update the minimum distance and make the speed matc the closer agent.
                        minDist = distance;
                        blockSpeed.Value = otherSpeed;
                    }
                }

                // If there is a valid block speed value, then:
                if (math.abs(blockSpeed.Value) > float.Epsilon)
                {
                    // Add a BlockSpeed component with the speed of the car that is ahead of it.
                    ecb.AddComponent(nativeThreadIndex, entity, blockSpeed);
                    ecb.RemoveComponent<OvertakeTag>(nativeThreadIndex, entity);
                }
            }).ScheduleParallel();

        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        Dependency = speeds0.Dispose(Dependency);
        Dependency = speeds1.Dispose(Dependency);
        Dependency = speeds2.Dispose(Dependency);
        Dependency = speeds3.Dispose(Dependency);
        Dependency = percents0.Dispose(Dependency);
        Dependency = percents1.Dispose(Dependency);
        Dependency = percents2.Dispose(Dependency);
        Dependency = percents3.Dispose(Dependency);
        
        Dependency = percents.Dispose(Dependency);
        Dependency = speeds.Dispose(Dependency);
    }
}