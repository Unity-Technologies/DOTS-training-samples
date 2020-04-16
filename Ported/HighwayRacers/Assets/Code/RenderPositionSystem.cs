using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PercentCompleteSystem))]
public class RenderPositionSystem : SystemBase
{
    private EntityQuery m_RoadInfoQuery;
    protected override void OnCreate()
    {
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(RoadInfo)));
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(LaneInfoElement)));
        m_RoadInfoQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<RoadInfo>()
            }
        }); 
    }

    protected override void OnUpdate()
    {
        var laneInfo = GetSingleton<RoadInfo>();
        var entities = m_RoadInfoQuery.ToEntityArray(Allocator.Persistent);
        DynamicBuffer<LaneInfoElement> m_LaneInfoElements = EntityManager.GetBuffer<LaneInfoElement>(entities[0]);

        Entities.ForEach((ref Translation translation, in PercentComplete percentComplete, in LaneAssignment laneAssignment) =>
        {
            LaneInfoElement lane = m_LaneInfoElements[laneAssignment.Value];
            float xPos = lane.Value.Pivot;
            float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.y) * percentComplete.Value;
            translation.Value.x = xPos;
            translation.Value.z = yPos;
        }).ScheduleParallel();
    }
}
