using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(LocalPositionSystem))]
public class RenderPositionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(RoadInfo)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfo)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfoElement)));
    }

    protected override void OnUpdate()
    {
        var entity = GetSingletonEntity<RoadInfo>();
        var laneInfos = EntityManager.GetBuffer<LaneInfo>(entity).AsNativeArray();

        var segmentInfos = EntityManager.GetBuffer<SegmentInfoElement>(entity).AsNativeArray();
        
        
        Entities.ForEach(
            (ref Translation translation, in PercentComplete percentComplete, in LaneAssignment lane, in SegmentAssignment segment) =>
            {
                LaneInfo laneInfo = laneInfos[lane.Value];
                float xPos = laneInfos[lane.Value].Pivot;

                SegmentInfo seg = segmentInfos[segment.Value].SegmentInfo;

                float yPos = seg.StartXZ.y + (seg.EndXZ.y - seg.StartXZ.y) * percentComplete.Value;
                translation.Value.x = xPos;
                translation.Value.z = yPos;
            }).Schedule();
    }
}