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

        Entities
            .WithReadOnly(segmentInfos)
            .ForEach((ref Translation translation, ref Rotation rotation, in LocalTranslation localTranslation, in LocalRotation localRotation,
                in SegmentAssignment segmentAssignment) =>
            {
                var segmentInfo = segmentInfos[segmentAssignment.Value].SegmentInfo;
                float sin = math.sin(-segmentInfo.StartRotation);
                float cos = math.cos(-segmentInfo.StartRotation);

                float rotatedX = localTranslation.Value.x * cos - localTranslation.Value.y * sin;
                float rotatedZ = localTranslation.Value.x * sin + localTranslation.Value.y * cos;

                // update translation
                translation.Value.x = rotatedX + segmentInfo.StartXZ.x;
                translation.Value.z = rotatedZ + segmentInfo.StartXZ.y;

                // update rotation
                float newRotationValue = localRotation.Value + segmentInfo.StartRotation;
                rotation.Value = quaternion.EulerXYZ(0, newRotationValue, 0);
            }).Schedule();
    }
}