using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateAfter(typeof(BlockSystem))]
public class OvertakeSystem : SystemBase
{
    public const int LEFT_LANE = 0;
    public const int RIGHT_LANE = 3;

    private EntityQuery m_laneQuery;
    private EntityCommandBufferSystem m_endSim;
    private bool m_doLeft = true; // toggle every frame

    protected override void OnCreate()
    {
        base.OnCreate();
        m_endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_laneQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<LaneAssignment>(),
                ComponentType.ReadOnly<PercentComplete>(),
            }
        });
    }


    protected override void OnUpdate()
    {
        var roadInfo =
            GetSingleton<RoadInfo>(); // TODO assumes one RoadInfo, but will we have multiple with segmentation? 
        float minDist = roadInfo.CarSpawningDistancePercent;
        minDist *= 1.1f; // add some extra padding

        int count = m_laneQuery.CalculateEntityCount();
        NativeMultiHashMap<int, PercentComplete> percents =
            new NativeMultiHashMap<int, PercentComplete>(count, Allocator.TempJob);

        Entities.ForEach((in PercentComplete percentComplete, in LaneAssignment laneAssignment) =>
        {
            percents.Add(laneAssignment.Value, percentComplete);
        }).Schedule(Dependency).Complete();

        int count0 = percents.CountValuesForKey(0);
        int count1 = percents.CountValuesForKey(1);
        int count2 = percents.CountValuesForKey(2);
        int count3 = percents.CountValuesForKey(3);

        NativeArray<PercentComplete> percents0 = new NativeArray<PercentComplete>(count0, Allocator.TempJob);
        NativeArray<PercentComplete> percents1 = new NativeArray<PercentComplete>(count1, Allocator.TempJob);
        NativeArray<PercentComplete> percents2 = new NativeArray<PercentComplete>(count2, Allocator.TempJob);
        NativeArray<PercentComplete> percents3 = new NativeArray<PercentComplete>(count3, Allocator.TempJob);

        Job.WithCode(() =>
        {
            for (int i = 0; i < 4; i++)
            {
                var percentList = percents.GetValuesForKey(i);

                switch (i)
                {
                    case 0:
                        for (int j = 0; j < count0; j++)
                        {
                            percents0[j] = percentList.Current;
                            percentList.MoveNext();
                        }

                        break;
                    case 1:
                        for (int j = 0; j < count1; j++)
                        {
                            percents1[j] = percentList.Current;
                            percentList.MoveNext();
                        }

                        break;
                    case 2:
                        for (int j = 0; j < count2; j++)
                        {
                            percents2[j] = percentList.Current;
                            percentList.MoveNext();
                        }

                        break;
                    case 3:
                        for (int j = 0; j < count3; j++)
                        {
                            percents3[j] = percentList.Current;
                            percentList.MoveNext();
                        }

                        break;
                }
            }
        }).Run();
        
        int laneInc = 1;
        if (m_doLeft)
        {
            laneInc = -1;
        }

        m_doLeft = !m_doLeft; // toggle for next frame

        EntityCommandBuffer.Concurrent ecb = m_endSim.CreateCommandBuffer().ToConcurrent();

        Entities.WithName("block_system")
            .WithNone<BlockSpeed>()
            .ForEach((Entity entity, int nativeThreadIndex,
                in PercentComplete percentComplete, in LaneAssignment laneAssignment) =>
            {
                // Speed of zero is invalid. Which means that there is no other agent blocking this agent.
                var blockSpeed = new BlockSpeed {Value = 0};

                int otherIdx = laneAssignment.Value + laneInc;
                if (otherIdx > 3 || otherIdx < 0) return;

                NativeArray<PercentComplete> otherPercents = default;
                switch (otherIdx)
                {
                    case 0:
                        otherPercents = percents0;
                        break;
                    case 1:
                        otherPercents = percents1;
                        break;
                    case 2:
                        otherPercents = percents2;
                        break;
                    case 3:
                        otherPercents = percents3;
                        break;
                }

                bool clear = true;
                for (int i = 0; i < otherPercents.Length; i++)
                {
                    var leftPercent = otherPercents[i].Value;
                    if (percentComplete.Value < leftPercent + minDist &&
                        percentComplete.Value + minDist > leftPercent)
                    {
                        clear = false; 
                        break;
                    }
                }

                if (clear)
                {
                    ecb.SetComponent(nativeThreadIndex, entity, new LaneAssignment() {Value = otherIdx});
                    ecb.AddComponent<OvertakeTag>(nativeThreadIndex, entity);
                    ecb.RemoveComponent<BlockSpeed>(nativeThreadIndex, entity);
                }
            }).ScheduleParallel();

        m_endSim.AddJobHandleForProducer(Dependency);

        Dependency = percents0.Dispose(Dependency);
        Dependency = percents1.Dispose(Dependency);
        Dependency = percents2.Dispose(Dependency);
        Dependency = percents3.Dispose(Dependency);

        Dependency = percents.Dispose(Dependency);
    }
}