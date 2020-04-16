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
    }

    protected override void OnUpdate()
    {
        var laneInfo = GetSingleton<RoadInfo>();

        Entities.ForEach((ref Translation translation, in PercentComplete percentComplete, in LaneAssignment laneAssignment) =>
        {
            float xPos = ((laneInfo.EndXZ.x - laneInfo.StartXZ.x) / 4) * laneAssignment.Value;
            float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.y) * percentComplete.Value;
            translation.Value.x = xPos;
            translation.Value.z = yPos;
        }).ScheduleParallel();
    }
}
