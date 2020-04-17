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
        RequireForUpdate(EntityManager.CreateEntityQuery(typeof(SegmentInfoElement)));
    }

    protected override void OnUpdate()
    {
        var laneInfo = GetSingleton<RoadInfo>();
        var entity = GetSingletonEntity<RoadInfo>();

        var laneInfoElements = EntityManager.GetBuffer<LaneInfoElement>(entity).AsNativeArray();
        var segmentInfoElements = EntityManager.GetBuffer<SegmentInfoElement>(entity).AsNativeArray();

        List<LaneAssignment> lanes = new List<LaneAssignment>();
        EntityManager.GetAllUniqueSharedComponentData<LaneAssignment>(lanes);
        
        //JobHandle combined = new JobHandle();
        foreach (LaneAssignment lane in lanes)
        {
            
            // TODO Translations somehow causes conflict when these jobs overlap 
            
            // var temp = Entities.WithSharedComponentFilter(lane).ForEach(
            //     (ref Translation translation, in PercentComplete percentComplete) =>
            //     {
            //         float yPos = laneInfo.StartXZ.y + (laneInfo.EndXZ.y - laneInfo.StartXZ.y) * percentComplete.Value;
            //         translation.Value.x = xPos;
            //         translation.Value.z = yPos;
            //     }).Schedule(Dependency);

            Entities
                .WithSharedComponentFilter(lane)
                .ForEach((ref Translation translation, ref Rotation rotation, in LocalTranslation localTranslation, in LocalRotation localRotation, in SegmentAssignment segmentAssignment) =>
                {
                    float sin = math.sin(-localRotation.Value);
                    float cos = math.cos(-localRotation.Value);

                    float rotatedX  = localTranslation.Value.x * cos - localTranslation.Value.y * sin;
                    float rotatedZ = localTranslation.Value.x * sin + localTranslation.Value.y * cos;

                    // update translation
                    var segmentInfo = segmentInfoElements[segmentAssignment.Value].Value;
                    float2 segmentStartXZ = segmentInfo.StartXZ;
                    translation.Value.x = rotatedX + segmentStartXZ.x;
                    translation.Value.y = rotatedZ + segmentStartXZ.y;
                    
                    // update rotation
                    float newRotationValue = localRotation.Value + segmentInfo.StartRotation;
                    rotation.Value = quaternion.EulerXYZ(0, newRotationValue, 0);
                }).Schedule();

            // TODO each lane is small enough to be done in single job, but try profile with ScheduleParallel 
            //combined = JobHandle.CombineDependencies(combined, temp);
        }

        //Dependency = combined;
    }
}