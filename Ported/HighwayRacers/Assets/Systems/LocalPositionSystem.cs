using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(SegmentAssignmentSystem))]
public class LocalPositionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(RoadInfo)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfo)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfoElement)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LanePercentageRange)));
    }

    protected override void OnUpdate()
    {
        var roadInfo = GetSingleton<RoadInfo>();
        var roadEntity = GetSingletonEntity<RoadInfo>();
        var laneInfos = EntityManager.GetBuffer<LaneInfo>(roadEntity).AsNativeArray();
        var segmentInfoBufferLookup = GetBufferFromEntity<SegmentInfoElement>();
        var segmentInfoBuffer = segmentInfoBufferLookup[roadEntity];
        var lanePercentageBufferLookup = GetBufferFromEntity<LanePercentageRange>();


        Entities
            .WithReadOnly(laneInfos)
            .WithReadOnly(segmentInfoBuffer)
            .WithReadOnly(lanePercentageBufferLookup)
            .ForEach((ref LocalTranslation translation, ref LocalRotation rotation, in Entity entity,
                in PercentComplete percentComplete, in SegmentAssignment segment, in LaneAssignment lane) =>
            {
                var laneInfo = laneInfos[lane.Value];
                var segmentInfo = segmentInfoBuffer[segment.Value].SegmentInfo;
                var segmentEntity = segmentInfoBuffer[segment.Value].Entity;
                var lanePercentageBuffer = lanePercentageBufferLookup[segmentEntity];
                float localDistance = (percentComplete.Value - lanePercentageBuffer[lane.Value].Value.x) *
                                      laneInfo.TotalLength;

                if (segmentInfo.SegmentShape == SegmentShape.Straight)
                {
                    translation.Value.x = laneInfo.Pivot;
                    translation.Value.y = localDistance;
                    rotation.Value = 0;
                }
                else
                {
                    float radius = laneInfo.Radius;
                    float angle = localDistance / radius;

                    translation.Value.x = roadInfo.MidRadius - math.cos(angle) * radius;
                    translation.Value.y = math.sin(angle) * radius;
                    rotation.Value = angle;
                }
            }).Run();
    }
}