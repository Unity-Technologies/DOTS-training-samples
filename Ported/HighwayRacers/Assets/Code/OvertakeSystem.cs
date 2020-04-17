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
        m_laneQuery.AddSharedComponentFilter(new LaneAssignment());
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
        var roadInfo = GetSingleton<RoadInfo>(); // TODO assumes one RoadInfo, but will we have multiple with segmentation? 
        float carLength = roadInfo.CarSpawningDistancePercent;
        
        List<LaneAssignment> lanes = new List<LaneAssignment>();
        EntityManager.GetAllUniqueSharedComponentData<LaneAssignment>(lanes);
        
        JobHandle combined = new JobHandle();
        foreach (LaneAssignment lane in lanes)
        {
            if (lane.Value != outerLane) 
            {
                EntityCommandBuffer.Concurrent ecb = m_endSim.CreateCommandBuffer().ToConcurrent();
                
                m_laneQuery.SetSharedComponentFilter(new LaneAssignment() {Value = lane.Value + laneInc});
                var percentCompletes = 
                    m_laneQuery.ToComponentDataArrayAsync<PercentComplete>(Allocator.TempJob,
                        out var percentCompletesHandle);
        
                LaneAssignment otherLane = new LaneAssignment() {Value = lane.Value + laneInc};  

                var temp = Entities
                    .WithName("Update_Overtake_Merge_Right")
                    .WithAll<BlockSpeed>()
                    .WithSharedComponentFilter(lane)
                    .WithoutBurst()    // TODO get rid of setting shared component to enable burst
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
                
                combined = JobHandle.CombineDependencies(percentCompletes.Dispose(temp), combined);
            }
        }

        Dependency = combined;
        m_endSim.AddJobHandleForProducer(combined);
    }
}