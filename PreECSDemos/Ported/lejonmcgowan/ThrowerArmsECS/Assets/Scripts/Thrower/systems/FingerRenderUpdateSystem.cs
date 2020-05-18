using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


// TODO: confirm this is updating early enough. If not, move to late in the SimulationSystemGroup.
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class FingerRenderUpdateSystem : SystemBase
{
    

    protected override void OnUpdate()
    {
        // TODO: SystemBase has a way to eliminate the need for this GetBufferFromEntity
        var FingerJointsFromEntity = GetBufferFromEntity<FingerJointElementData>(true);
        var UpBases = GetComponentDataFromEntity<ArmBasesUp>(true);
        var FingerParents = GetComponentDataFromEntity<FingerParent>(true);

        Entities.WithReadOnly(FingerJointsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(FingerParents)
            .ForEach((ref Translation translation, ref Rotation rotation, ref NonUniformScale scale,
                in FingerRenderComponentData fingerRef,
                in FingerThickness thickness) =>
            {
                var armRef = FingerParents[fingerRef.fingerEntity];
                var joints = FingerJointsFromEntity[fingerRef.fingerEntity];
                var jointPos = joints[fingerRef.jointIndex];
                var delta = joints[fingerRef.jointIndex + 1].value - jointPos.value; // from joint end to joint begin
                var upBasis = UpBases[armRef].value;

                var debugDelta = jointPos + 0.5f * delta;

                translation = new Translation()
                {
                    Value = debugDelta
                };
                rotation = new Rotation()
                {
                    Value = quaternion.LookRotation(math.normalize(delta), upBasis)
                };

                scale = new NonUniformScale()
                {
                    Value = new float3(thickness, thickness, math.length(delta))
                };
            }).ScheduleParallel();

    }
}