using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class RenderPositionSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfo)));
    }

    protected override void OnUpdate()
    {
        var laneInfo = GetSingleton<LaneInfo>();

        Entities.ForEach((ref Position position, in PercentComplete percentComplete, in LaneAssignment laneAssignment) =>
        {
            float xPos = ((laneInfo.StartXZ.x - laneInfo.EndXZ.y) / 4) * laneAssignment.Value;
            float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.x) * percentComplete.Value;
            position.Value = new float2(xPos, yPos);
        }).ScheduleParallel();
    }
}
