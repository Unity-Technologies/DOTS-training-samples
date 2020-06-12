using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(PercentCompleteSystem))]
public class SegmentAssignmentSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentsAddedToBufferTag)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentPercentagesInitializedTag)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfoElement)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LanePercentageRange)));
    }

    protected override void OnUpdate()
    {
        var roadInfoEntity = GetSingletonEntity<SegmentsAddedToBufferTag>();
        var segmentInfoBuffer = EntityManager.GetBuffer<SegmentInfoElement>(roadInfoEntity).AsNativeArray();
        var rangesFromSegment = GetBufferFromEntity<LanePercentageRange>();

        Entities
            .WithReadOnly(segmentInfoBuffer)
            .WithReadOnly(rangesFromSegment)
            .ForEach((ref SegmentAssignment segmentAssignment, in PercentComplete percentComplete,
                in LaneAssignment lane) =>
            {
                for (int i = 0; i < segmentInfoBuffer.Length; i++)
                {
                    var segmentInfoBufferElement = segmentInfoBuffer[i];
                    var segmentInfoEntity = segmentInfoBufferElement.Entity;
                    var laneRanges = rangesFromSegment[segmentInfoEntity];
                    if (percentComplete.Value >= laneRanges[lane.Value].Value[0] &&
                        percentComplete.Value < laneRanges[lane.Value].Value[1])
                    {
                        segmentAssignment.Value = segmentInfoBufferElement.SegmentInfo.Order;
                        break;
                    }
                }
            }).ScheduleParallel();
    }
}