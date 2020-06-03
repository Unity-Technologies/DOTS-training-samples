using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(SegmentInfoBufferInitSystem))]
public class SegmentPercentageInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        
        RequireSingletonForUpdate<RoadInfo>();
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfo)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfoElement)));
    }

    protected override void OnUpdate()
    {
        var roadInfoEntity = GetSingletonEntity<RoadInfo>();
        if(!HasComponent<SegmentPercentagesInitializedTag>(roadInfoEntity))
        {
            var segmentInfoBuffer = EntityManager.GetBuffer<SegmentInfoElement>(roadInfoEntity);
            var laneInfoBuffer = EntityManager.GetBuffer<LaneInfoElement>(roadInfoEntity);
            var roadInfo = GetComponent<RoadInfo>(roadInfoEntity);

            // total length of each lane for all segments
            var laneLengths = new NativeArray<float>(roadInfo.MaxLanes, Allocator.Temp);
            // length of each lane of each segment
            var segmentLaneLengths = new NativeArray<float>[segmentInfoBuffer.Length];
            
            for (int i = 0; i < segmentInfoBuffer.Length; i++)
            {
                var segmentInfo = segmentInfoBuffer[i].SegmentInfo;
                var segmentLaneLengthElement = new NativeArray<float>(roadInfo.MaxLanes, Allocator.Temp);
                for (int lane = 0; lane < laneLengths.Length; lane++)
                {
                    var segmentLaneLength = segmentInfo.SegmentShape == SegmentShape.Straight ? roadInfo.StraightPieceLength : laneInfoBuffer[lane].Value.CurvedPieceLength;
                    segmentLaneLengthElement[lane] = segmentLaneLength;
                    laneLengths[lane] += segmentLaneLength;
                }

                segmentLaneLengths[i] = segmentLaneLengthElement;
            }

            // percentage ranges of each lane of each segment
            var segmentLanePercentageRanges = new NativeArray<LanePercentageRangeElement>[segmentInfoBuffer.Length];
            // accumulates the total percentage of each lane for all segments
            var lastPercentage = new NativeArray<float>(roadInfo.MaxLanes, Allocator.Temp);
            
            for (int i = 0; i < segmentInfoBuffer.Length; i++)
            {
                var lanePercentageRanges = new NativeArray<LanePercentageRangeElement>(roadInfo.MaxLanes, Allocator.Temp);
                
                for (int j = 0; j < roadInfo.MaxLanes; j++)
                {
                    var laneInfo = laneInfoBuffer[j].Value;
                    laneInfo.TotalLength = laneLengths[j];
                    laneInfoBuffer[j] = new LaneInfoElement
                    {
                        Value = laneInfo
                    };
                    
                    float nextPercentage = lastPercentage[j] + segmentLaneLengths[i][j] / laneLengths[j];
                    lanePercentageRanges[j] = new float2(lastPercentage[j], nextPercentage);
                    lastPercentage[j] = nextPercentage;
                }

                segmentLanePercentageRanges[i] = lanePercentageRanges;
            }

            Entities.WithStructuralChanges().ForEach((in SegmentInfo segmentInfo, in Entity entity) =>
            {
                var percentageBuffer = EntityManager.AddBuffer<LanePercentageRangeElement>(entity);
                percentageBuffer.AddRange(segmentLanePercentageRanges[segmentInfo.Order]);
            }).Run();

            EntityManager.AddComponent<SegmentPercentagesInitializedTag>(roadInfoEntity);
        }
    }
}
