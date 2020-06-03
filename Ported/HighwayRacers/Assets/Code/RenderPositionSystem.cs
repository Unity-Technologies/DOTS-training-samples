using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PercentCompleteSystem))]
public class RenderPositionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(RoadInfo)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfoElement)));
    }

    protected override void OnUpdate()
    {
        var laneInfo = GetSingleton<RoadInfo>();
        var entity = GetSingletonEntity<RoadInfo>();
        var laneInfoElements = EntityManager.GetBuffer<LaneInfoElement>(entity).AsNativeArray();
        
        Entities.ForEach(
            (ref Translation translation, in PercentComplete percentComplete, in LaneAssignment lane) =>
            {
                float xPos = laneInfoElements[lane.Value].Value.Pivot;
                float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.y) * percentComplete.Value;
                translation.Value.x = xPos;
                translation.Value.z = yPos;
            }).Schedule();
    }
}