using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class LocalPositionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(RoadInfo)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfoElement)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfoElement)));
    }
    protected override void OnUpdate()
    {
        var roadInfo = GetSingleton<RoadInfo>();
        var entity = GetSingletonEntity<RoadInfo>();
        var laneInfoElements = EntityManager.GetBuffer<LaneInfoElement>(entity).AsNativeArray();
        var segmentInfoElements = EntityManager.GetBuffer<SegmentInfoElement>(entity).AsNativeArray();

        List<LaneAssignment> lanes = new List<LaneAssignment>();
        EntityManager.GetAllUniqueSharedComponentData<LaneAssignment>(lanes);

        foreach (LaneAssignment lane in lanes)
        {
            // Straight pieces
            Entities
                .WithNone<CurvedSegment>()
                .WithSharedComponentFilter(lane)
                .ForEach((ref LocalTranslation translation, ref LocalRotation rotation, in PercentComplete percentComplete, in SegmentAssignment segmentAssignment) =>
                {
                    translation.Value.x = roadInfo.LaneWidth * ((roadInfo.MaxLanes - 1) / 2f - lane.Value);
                    translation.Value.y = (percentComplete.Value * roadInfo.TotalLength) - segmentInfoElements[segmentAssignment.Value].Value.Length;
                    rotation.Value = 0;
                }).Schedule();

            // Curved pieces
            Entities
                .WithAll<CurvedSegment>()
                .WithSharedComponentFilter(lane)
                .ForEach((ref LocalTranslation translation, ref LocalRotation rotation, in PercentComplete percentComplete, in SegmentAssignment segmentAssignment) =>
                {
                    float radius = laneInfoElements[lane.Value].Value.Radius;
                    float localDistance = (percentComplete.Value * roadInfo.TotalLength) - segmentInfoElements[segmentAssignment.Value].Value.Length;
                    float angle = localDistance / radius;
                    translation.Value.x = roadInfo.MidRadius - math.cos(angle) * radius;
                    translation.Value.y = math.sin(angle) * radius;
                    rotation.Value = angle;
                }).Schedule();
        }
    }
}
