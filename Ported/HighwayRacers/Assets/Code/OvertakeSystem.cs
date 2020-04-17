using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(BlockSystem))]
public class OvertakeSystem : SystemBase
{
    public const int LEFT_LANE = 0;
    public const int RIGHT_LANE = 3;

    private EntityQuery[] m_laneQueries;
    private EntityCommandBufferSystem m_endSim;
    private bool m_doLeft = true; // toggle every frame

    protected override void OnCreate()
    {
        base.OnCreate();

        m_endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        m_laneQueries = new EntityQuery[RIGHT_LANE];

        for (int i = 0; i < RIGHT_LANE; i++)
        {
            m_laneQueries[i] = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<LaneAssignment>(),
                    ComponentType.ReadOnly<PercentComplete>(),
                }
            });
            m_laneQueries[i].AddSharedComponentFilter(new LaneAssignment() {Value = i});
        }
    }

    protected override void OnUpdate()
    {
        int outerLane = RIGHT_LANE;
        int laneInc = 1;
        if (m_doLeft)
        {
            outerLane = LEFT_LANE;
            laneInc = -1;
        }
        m_doLeft = !m_doLeft; // toggle for next frame

        List<LaneAssignment> lanes = new List<LaneAssignment>();
        EntityManager.GetAllUniqueSharedComponentData<LaneAssignment>(lanes);

        // TODO will we always have one RoadInfo?
        var roadInfo = GetSingleton<RoadInfo>();
        float carLength = roadInfo.CarSpawningDistance;

        JobHandle combined = new JobHandle();
        foreach (LaneAssignment lane in lanes)
        {
            EntityCommandBuffer.Concurrent ecb = m_endSim.CreateCommandBuffer().ToConcurrent();
            
            if (lane.Value != outerLane)
            {
                EntityQuery otherLaneQuery = m_laneQueries[lane.Value + laneInc];
                var percentCompletes =
                    otherLaneQuery.ToComponentDataArrayAsync<PercentComplete>(Allocator.TempJob,
                        out var percentCompletesHandle);

                LaneAssignment otherLane = new LaneAssignment() {Value = lane.Value + laneInc};

                var temp = Entities
                    .WithName("Update_Overtake_Merge_Right")
                    .WithAll<BlockSpeed>()
                    .WithSharedComponentFilter(lane)
                    .ForEach(
                        (Entity ent, int nativeThreadIndex, in PercentComplete percent,
                            in MinimumDistance minDist) =>
                        {
                            var min = minDist.Value;
                            if (min < carLength)
                            {
                                min = carLength;
                            }

                            bool clear = true;
                            for (int i = 0; i < percentCompletes.Length; i++)
                            {
                                var leftPercent = percentCompletes[i].Value;
                                if (percent.Value < leftPercent + min)
                                {
                                    if (percent.Value + minDist.Value > leftPercent)
                                    {
                                        clear = false;
                                        break;
                                    }
                                }
                            }
                            if (clear)
                            {
                                ecb.SetSharedComponent(nativeThreadIndex, ent, otherLane);
                                ecb.AddComponent<OvertakeTag>(nativeThreadIndex, ent);
                                ecb.RemoveComponent<BlockSpeed>(nativeThreadIndex, ent);
                            }
                        }
                    )
                    .ScheduleParallel(
                        JobHandle.CombineDependencies(percentCompletesHandle, Dependency));
                combined = JobHandle.CombineDependencies(combined, temp);
            }
        }
        m_endSim.AddJobHandleForProducer(combined);
    }
}