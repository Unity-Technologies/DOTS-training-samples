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
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfoElement)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfoElement)));
    }

    protected override void OnUpdate()
    {
        var entity = GetSingletonEntity<RoadInfo>();
        var laneInfoElements = EntityManager.GetBuffer<LaneInfoElement>(entity).AsNativeArray();
        
        Entities.ForEach(
            (ref Translation translation, in PercentComplete percentComplete, in LaneAssignment lane) =>
            {
                LaneInfoElement laneInfo = laneInfoElements[lane.Value];
                float xPos = laneInfoElements[lane.Value].Value.Pivot;
                // float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.y) * percentComplete.Value;
                // translation.Value.x = xPos;
                // translation.Value.z = yPos;
            }).Schedule();
    }
}