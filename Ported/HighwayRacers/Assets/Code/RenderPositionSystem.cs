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

        List<LaneAssignment> lanes = new List<LaneAssignment>();
        EntityManager.GetAllUniqueSharedComponentData<LaneAssignment>(lanes);
        
        //JobHandle combined = new JobHandle();
        foreach (LaneAssignment lane in lanes)
        {
            float xPos = laneInfoElements[lane.Value].Value.Pivot;
            
            // TODO Translations somehow causes conflict when these jobs overlap 
            
            // var temp = Entities.WithSharedComponentFilter(lane).ForEach(
            //     (ref Translation translation, in PercentComplete percentComplete) =>
            //     {
            //         float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.y) * percentComplete.Value;
            //         translation.Value.x = xPos;
            //         translation.Value.z = yPos;
            //     }).Schedule(Dependency);

            Entities.WithSharedComponentFilter(lane).ForEach(
                (ref Translation translation, in PercentComplete percentComplete) =>
                {
                    float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.y) * percentComplete.Value;
                    translation.Value.x = xPos;
                    translation.Value.z = yPos;
                }).Schedule();

            // TODO each lane is small enough to be done in single job, but try profile with ScheduleParallel 
            //combined = JobHandle.CombineDependencies(combined, temp);
        }

        //Dependency = combined;
    }
}